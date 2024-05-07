using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;
using Rhino.Collections;

namespace Streamlines.ShapeFunction
{
    public class GH_MindlinReissnerQuadraticIsoPara : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_BilinearRectangle class.
        /// </summary>
        public GH_MindlinReissnerQuadraticIsoPara()
          : base("8 Node Isoparametric Slab", "8IsoparaSlab",
              "Transforms Mesh and results into 8-node isoparametric (i.e. any quadrilateral shape) slab elements for principal bending stress line analysis using the Mindlin-Reissner Plate Theory and Bilinear shape functions. ***NOTE THAT***: the rotational axes are defined by the right hand rule",
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
            pManager.AddVectorParameter("Rotations Corners", "φ-c", "Rotational vectors for each mesh vertex in a list sorted in the same order as the mesh vertices", GH_ParamAccess.list);
            pManager.AddVectorParameter("Rotations Mid-Nodes", "φ-md", "Rotational vectors for each mid-node in a list sorted in the same order as the mid-node points", GH_ParamAccess.list);
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
            List<Vector3d> iφc = new List<Vector3d>();
            List<Vector3d> iφmd = new List<Vector3d>();
            double iV = 0.0;

            DA.GetData(0, ref iMesh);
            DA.GetDataList(1, iMd);
            DA.GetDataList(2, iφc);
            DA.GetDataList(3, iφmd);
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

                Vector3d U1 = new Vector3d(iφc[p1].Y, -iφc[p1].X, 0.0);
                Vector3d U2 = new Vector3d(iφmd[p2].Y, -iφmd[p2].X, 0.0);
                Vector3d U3 = new Vector3d(iφc[p3].Y, -iφc[p3].X, 0.0);
                Vector3d U4 = new Vector3d(iφmd[p4].Y, -iφmd[p4].X, 0.0);
                Vector3d U5 = new Vector3d(iφmd[p5].Y, -iφmd[p5].X, 0.0);
                Vector3d U6 = new Vector3d(iφc[p6].Y, -iφc[p6].X, 0.0);
                Vector3d U7 = new Vector3d(iφmd[p7].Y, -iφmd[p7].X, 0.0);
                Vector3d U8 = new Vector3d(iφc[p8].Y, -iφc[p8].X, 0.0);

                //Create and analyse elements
                QuadraticIsoPara quadraticIsoPara1 = new QuadraticIsoPara(point1, point2, point3, point4, point5, point6, point7, point8, U1, U2, U3, U4, U5, U6, U7, U8, iV);

                //output data
                quadraticIsoPara1.ChangeDirection(1);
                sigma1.Add( new Element(quadraticIsoPara1));

                QuadraticIsoPara quadraticIsoPara2 = new QuadraticIsoPara(point1, point2, point3, point4, point5, point6, point7, point8, U1, U2, U3, U4, U5, U6, U7, U8, iV);

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
            get { return new Guid("48dde900-f69b-42a1-9255-298eea3ebfcb"); }
        }
    }
}