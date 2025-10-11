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
    public class GH_MindlinReissnerQuad8 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_MindlinReissnerQuad8 class.
        /// base(Name, Nickname, Description, Folder, SubFolder)
        /// </summary>
        public GH_MindlinReissnerQuad8()
          : base("8 Node Quad Slab", "Quad8Slab",
              "Transforms Mesh and results into 8-node quad slab elements for principal bending stress line analysis using the Mindlin-Reissner Plate Theory and Quadratic shape functions. ***NOTE THAT***: the rotational axes are defined by the right hand rule",
              "LilyPad", " Setup")
        {
        }

        /// Register all input parameters.
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddPointParameter("Mid-Nodes", "md", "Mid-node locations as a list of points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Rotations Corners", "φ-c", "Rotational vectors for each mesh vertex in a list sorted in the same order as the mesh vertices", GH_ParamAccess.list);
            pManager.AddVectorParameter("Rotations Mid-Nodes", "φ-md", "Rotational vectors for each mid-node in a list sorted in the same order as the mid-node points", GH_ParamAccess.list);
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
        /// Assembles list of elements into two principal meshes one for each direction
        /// </summary>
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

                Point3d point2 = (point1 + point3) / 2;
                Point3d point4 = (point1 + point6) / 2;
                Point3d point5 = (point3 + point8) / 2;
                Point3d point7 = (point6 + point8) / 2;

                Point3dList midPoints = new Point3dList(iMd);
                int p2 = midPoints.ClosestIndex(point2);
                int p4 = midPoints.ClosestIndex(point4);
                int p5 = midPoints.ClosestIndex(point5);
                int p7 = midPoints.ClosestIndex(point7);

                // Fit a plane to the points and find the maximum distance from the plane to the points
                Plane fitPlane;
                double maxDeviation;
                if (Plane.FitPlaneToPoints(new List<Point3d> { point1, point3, point6, point8 }, out fitPlane, out maxDeviation) != 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Face {i} could not fit a plane.");
                    return;
                }

                if (maxDeviation > 0.01)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Face {i} is too warped. Plane deviation: {maxDeviation}");
                    return;
                }

                
                Vector3d U1 = iφc[p1];
                Vector3d U2 = iφmd[p2];
                Vector3d U3 = iφc[p3];
                Vector3d U4 = iφmd[p4];
                Vector3d U5 = iφmd[p5];
                Vector3d U6 = iφc[p6];
                Vector3d U7 = iφmd[p7];
                Vector3d U8 = iφc[p8];

                //Create and analyse elements
                Quad8Element quadraticIsoPara1 = new Quad8Element(point1, point2, point3, point4, point5, point6, point7, point8, U1, U2, U3, U4, U5, U6, U7, U8, iV, false);

                //output data
                quadraticIsoPara1.ChangeDirection(1);
                sigma1.Add(new Element(quadraticIsoPara1));

                Quad8Element quadraticIsoPara2 = new Quad8Element(point1, point2, point3, point4, point5, point6, point7, point8, U1, U2, U3, U4, U5, U6, U7, U8, iV, false);

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
                return Properties.Resources.IconSlabQuad8;
            }
        }

        /// Gets the unique ID for the component
        public override Guid ComponentGuid
        {
            get { return new Guid("48dde900-f69b-42a1-9255-298eea3ebfcb"); }
        }

        ///Set component to be in the SECOND group of the sub-category
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }
    }
}