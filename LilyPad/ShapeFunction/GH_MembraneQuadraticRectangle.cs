using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;

namespace Streamlines.ShapeFunction
{
    public class GH_MembraneQuadraticRectangle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_QuadraticRectangle class.
        /// </summary>
        public GH_MembraneQuadraticRectangle()
          : base("8 Node Rectangle Membrane", "8RecMem",
              "Transforms Mesh and results into 8-node retangular membrane elements for principal stress line analysis using the theory of in-plane loaded plates and Bilinear shape functions",
              "Streamlines", "ShapeFunction")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Rectangular Mesh", GH_ParamAccess.item);
            pManager.AddVectorParameter("Displacment U1", "U1", "Displacement vector at the top side left point of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacment U2", "U2", "Displacement vector at the top side middle point of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacment U3", "U3", "Displacement vector at the top side right point of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacment U4", "U4", "Displacement vector at the left side middle of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacment U5", "U5", "Displacement vector at the right side middle of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacment U6", "U6", "Displacement vector at the bottom side left of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacment U7", "U7", "Displacement vector at the bottom side middle of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacment U8", "U8", "Displacement vector at the bottom side right of mesh faces", GH_ParamAccess.list);
            pManager.AddNumberParameter("Poisson's ratio", "v", "Poisson's ratio", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Principle Direction 1", "P1", "A vector field showing the first principle direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Principle Direction 2", "P2", "A vector field showing the second principle direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh iMesh = new Mesh();
            List<Vector3d> iU1 = new List<Vector3d>();
            List<Vector3d> iU2 = new List<Vector3d>();
            List<Vector3d> iU3 = new List<Vector3d>();
            List<Vector3d> iU4 = new List<Vector3d>();
            List<Vector3d> iU5 = new List<Vector3d>();
            List<Vector3d> iU6 = new List<Vector3d>();
            List<Vector3d> iU7 = new List<Vector3d>();
            List<Vector3d> iU8 = new List<Vector3d>();
            double iV = 0.0;

            DA.GetData(0, ref iMesh);
            DA.GetDataList(1, iU1);
            DA.GetDataList(2, iU2);
            DA.GetDataList(3, iU3);
            DA.GetDataList(4, iU4);
            DA.GetDataList(5, iU5);
            DA.GetDataList(6, iU6);
            DA.GetDataList(7, iU7);
            DA.GetDataList(8, iU8);
            DA.GetData(9, ref iV);

            //________________________________________________________________________________________________________________________

            //For each face create a bilinear rectangular element
            List<Element> sigma1 = new List<Element>();
            List<Element> sigma2 = new List<Element>();

            for (int i = 0; i < iMesh.Faces.Count; i++)
            {
                MeshFace face = iMesh.Faces[i];

                //create polyline of the face edges
                List<Point3d> points = new List<Point3d>();
                for (int j = 0; j < 4; j++) points.Add(iMesh.Vertices[face[j]]);
                points.Add(iMesh.Vertices[face[0]]);
                Polyline bounds = new Polyline(points);

                //Create and analyse elements
                QuadraticRectangle quadraticRectangle1 = new QuadraticRectangle(bounds.ToPolylineCurve(), iU1[i], iU2[i], iU3[i], iU4[i], iU5[i], iU6[i], iU7[i], iU8[i], iV);

                //output data
                quadraticRectangle1.ChangeDirection(1);
                sigma1.Add(new Element(quadraticRectangle1));

                QuadraticRectangle quadraticRectangle2 = new QuadraticRectangle(bounds.ToPolylineCurve(), iU1[i], iU2[i], iU3[i], iU4[i], iU5[i], iU6[i], iU7[i], iU8[i], iV);

                quadraticRectangle2.ChangeDirection(2);
                sigma2.Add(new Element(quadraticRectangle2));
            }

            //Creates FieldMesh data for output
            FieldMesh Sigma1 = new FieldMesh(sigma1, iMesh);
            FieldMesh Sigma2 = new FieldMesh(sigma2, iMesh);

            //Turn the field meshes into principalMeshes
            PrincipalMesh oSigma1 = new PrincipalMesh(Sigma1);
            PrincipalMesh oSigma2 = new PrincipalMesh(Sigma2);


            //________________________________________________________________________________________________________________________

            DA.SetData(0, oSigma1);
            DA.SetData(1, oSigma2);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d7673591-87d1-4355-9746-e79e12ec4e2e"); }
        }
    }
}