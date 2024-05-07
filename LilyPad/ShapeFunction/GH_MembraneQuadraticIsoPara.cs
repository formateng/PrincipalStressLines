using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;
using Rhino.Collections;

namespace Streamlines.ShapeFunction
{
    public class GH_MembraneQuadraticIsoPara : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_BilinearRectangle class.
        /// </summary>
        public GH_MembraneQuadraticIsoPara()
          : base("8 Node Isoparametric Membrane", "8IsoparaMem",
              "Transforms Mesh and results into 8-node retangular membrane elements for principal stress line analysis using the theory of in-plane loaded plates and Bilinear shape functions",
              "Streamlines", "ShapeFunction")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddPointParameter("Mid-Nodes", "md", "Mid-node locations as a list of points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacments Corners", "U-c", "Displacement vectors for each mesh vertex in a list sorted in the same order as the mesh vertices", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacments Mid-Nodes", "U-md", "Displacement vectors for each mid-node in a list sorted in the same order as the mid-node points", GH_ParamAccess.list);
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
            List<Point3d> iMd = new List<Point3d>();
            List<Vector3d> iUc = new List<Vector3d>();
            List<Vector3d> iUmd = new List<Vector3d>();
            double iV = 0.0;

            DA.GetData(0, ref iMesh);
            DA.GetDataList(1, iMd);
            DA.GetDataList(2, iUc);
            DA.GetDataList(3, iUmd);
            DA.GetData(4, ref iV);

            //________________________________________________________________________________________________________________________

            //For each face create a bilinear rectangular element
            List<Element> sigma1 = new List<Element>();
            List<Element> sigma2 = new List<Element>();

            for (int i = 0; i < iMesh.Faces.Count; i++)
            {
                MeshFace face = iMesh.Faces[i];
                int p1 = face[0];
                int p3 = face[1];
                int p6 = face[3];
                int p8 = face[2];

                Point3d point1 = iMesh.Vertices[p1];
                Point3d point3 = iMesh.Vertices[p3];
                Point3d point6 = iMesh.Vertices[p6];
                Point3d point8 = iMesh.Vertices[p8];

                Point3d point2 = (point1 + point3)/2;
                Point3d point4 = (point1 + point6)/2;
                Point3d point5 = (point3 + point8)/2;
                Point3d point7 = (point6 + point8)/2;

                Point3dList midPoints = new Point3dList(iMd);
                int p2 = midPoints.ClosestIndex(point2);
                int p4 = midPoints.ClosestIndex(point4);
                int p5 = midPoints.ClosestIndex(point5);
                int p7 = midPoints.ClosestIndex(point7);


                //Create and analyse elements
                QuadraticIsoPara quadraticIsoPara1 = new QuadraticIsoPara(point1, point2, point3, point4, point5, point6, point7, point8, iUc[p1], iUmd[p2], iUc[p3], iUmd[p4], iUmd[p5], iUc[p6], iUmd[p7], iUc[p8], iV);

                //output data
                quadraticIsoPara1.ChangeDirection(1);
                sigma1.Add( new Element(quadraticIsoPara1));

                QuadraticIsoPara quadraticIsoPara2 = new QuadraticIsoPara(point1, point2, point3, point4, point5, point6, point7, point8, iUc[p1], iUmd[p2], iUc[p3], iUmd[p4], iUmd[p5], iUc[p6], iUmd[p7], iUc[p8], iV);

                quadraticIsoPara2.ChangeDirection(2);
                sigma2.Add( new Element(quadraticIsoPara2));
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
            get { return new Guid("68dde900-f69b-42a1-9255-298eea3ebfcb"); }
        }
    }
}