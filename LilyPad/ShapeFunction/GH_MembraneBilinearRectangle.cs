using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;

namespace Streamlines.ShapeFunction
{
    public class GH_MembraneBilinearRectangle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_BilinearRectangle class.
        /// </summary>
        public GH_MembraneBilinearRectangle()
          : base("4 Node Rectangle Membrane", "4RecMem",
              "Transforms Mesh and results into 4-node rectangular membrane elements for principal stress line analysis using the theory of in-plane loaded plates and Bilinear shape functions",
              "Streamlines", "ShapeFunction")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Rectangular Mesh", GH_ParamAccess.item);
            pManager.AddVectorParameter("Displacment U1", "U1", "Displacement vector at the top left point of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacment U2", "U2", "Displacement vector at the top right point of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacment U3", "U3", "Displacement vector at the bottom left point of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacment U4", "U4", "Displacement vector at the bottom right point of mesh faces", GH_ParamAccess.list);
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
            double iV = 0.0;

            DA.GetData(0, ref iMesh);
            DA.GetDataList(1, iU1);
            DA.GetDataList(2, iU2);
            DA.GetDataList(3, iU3);
            DA.GetDataList(4, iU4);
            DA.GetData(5, ref iV);

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
                BilinearRectangle bilinearRectangle1 = new BilinearRectangle(bounds.ToPolylineCurve(), iU1[i], iU2[i], iU3[i], iU4[i], iV);

                //output data
                bilinearRectangle1.ChangeDirection(1);
                sigma1.Add( new Element(bilinearRectangle1));

                BilinearRectangle bilinearRectangle2 = new BilinearRectangle(bounds.ToPolylineCurve(), iU1[i], iU2[i], iU3[i], iU4[i], iV);

                bilinearRectangle2.ChangeDirection(2);
                sigma2.Add( new Element(bilinearRectangle2));
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
            get { return new Guid("68ddf900-f69b-41a1-9255-298eea3ebfcb"); }
        }
    }
}