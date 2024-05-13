using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;
using Rhino.Collections;
using LilyPad.Objects;
using LilyPad.Objects.ShapeFunction;

namespace LilyPad.Components.Setup
{
    public class GH_MembraneQuad8 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_MembraneQuad8 class.
        /// base(Name, Nickname, Description, Folder, SubFolder)
        /// </summary>
        public GH_MembraneQuad8()
          : base("8 Node Quad Membrane", "Quad8Mem",
              "Transforms Mesh and results into 8-node quad membrane elements for principal stress line analysis using the theory of in-plane loaded plates and quadratic shape functions",
              "LilyPad", "Setup")
        {
        }

        /// Register all input parameters.
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddPointParameter("Mid-Nodes", "md", "Mid-node locations as a list of points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacments Corners", "U-c", "Displacement vectors for each mesh vertex in a list sorted in the same order as the mesh vertices", GH_ParamAccess.list);
            pManager.AddVectorParameter("Displacments Mid-Nodes", "U-md", "Displacement vectors for each mid-node in a list sorted in the same order as the mid-node points", GH_ParamAccess.list);
            pManager.AddNumberParameter("Poisson's ratio", "v", "Poisson's ratio", GH_ParamAccess.item);
        }

        /// Register all output parameters.
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Principle Direction 1", "P1", "A vector field showing the first principle direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Principle Direction 2", "P2", "A vector field showing the second principle direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// Uses the provided mesh to create a series of quad 8 elements
        /// The mid-nodes and mid-node displacments are used for the additional 4 nodes around the perimeter of the element
        /// Assembles list of elements into two principal meshes one for each direction
        /// </summary>
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

                Point3d point2 = (point1 + point3) / 2;
                Point3d point4 = (point1 + point6) / 2;
                Point3d point5 = (point3 + point8) / 2;
                Point3d point7 = (point6 + point8) / 2;

                Point3dList midPoints = new Point3dList(iMd);
                int p2 = midPoints.ClosestIndex(point2);
                int p4 = midPoints.ClosestIndex(point4);
                int p5 = midPoints.ClosestIndex(point5);
                int p7 = midPoints.ClosestIndex(point7);


                //Create and analyse elements
                Quad8Element quadraticIsoPara1 = new Quad8Element(point1, point2, point3, point4, point5, point6, point7, point8, iUc[p1], iUmd[p2], iUc[p3], iUmd[p4], iUmd[p5], iUc[p6], iUmd[p7], iUc[p8], iV);

                //output data
                quadraticIsoPara1.ChangeDirection(1);
                sigma1.Add(new Element(quadraticIsoPara1));

                Quad8Element quadraticIsoPara2 = new Quad8Element(point1, point2, point3, point4, point5, point6, point7, point8, iUc[p1], iUmd[p2], iUc[p3], iUmd[p4], iUmd[p5], iUc[p6], iUmd[p7], iUc[p8], iV);

                quadraticIsoPara2.ChangeDirection(2);
                sigma2.Add(new Element(quadraticIsoPara2));
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

        /// Assign component icon
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return ResourceIcons.IconSlabQuad4;
            }
        }

        /// Gets the unique ID for the component
        public override Guid ComponentGuid
        {
            get { return new Guid("68dde900-f69b-42a1-9255-298eea3ebfcb"); }
        }
    }
}