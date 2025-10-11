using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;
using LilyPad.Objects;
using LilyPad.Objects.ShapeFunction;
using Rhino.FileIO;
using Rhino.Collections;

namespace LilyPad.Components.Setup
{
    public class GH_CreateDeformationMeshHigher : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_MembraneQuad4 class.
        /// base(Name, Nickname, Description, Folder, SubFolder)
        /// </summary>
        public GH_CreateDeformationMeshHigher()
          : base("Create Deformation Mesh - Higher", "DefMeshHigh",
              "Transforms Mesh and results into elements for principal stress line analysis using the theory of FEA shape functions - This is uses Tri6 and Quad8 Elements",
              "LilyPad", " Setup")
        {
        }

        /// Register all input parameters.
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddVectorParameter("Translations", "U", "Displacement vectors for each mesh vertex in a list sorted in the same order as the mesh vertices", GH_ParamAccess.list,new Vector3d());
            pManager.AddVectorParameter("Rotations", "φ", "Rotational vectors for each mesh vertex *NOTE THAT*: the rotational axes are defined by the right hand rule", GH_ParamAccess.list,new Vector3d());
            pManager.AddNumberParameter("Poisson's ratio", "v", "Poisson's ratio", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Bending Stress Lines", "BSL", "If True then bending stress lines are created, if false in-plane stresslines are created", GH_ParamAccess.item, false);
        }

        /// Register all output parameters.
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Principal Direction 1", "P1", "A vector field showing the first principle direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Principal Direction 2", "P2", "A vector field showing the second principle direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// Uses the provided mesh to create a series of quad 4 elements
        /// Assembles list of elements into two principal meshes one for each direction
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh iMesh = new Mesh();
            List<Vector3d> iU = new List<Vector3d>();
            List<Vector3d> iφ = new List<Vector3d>();
            double iV = 0.0;
            bool iBending = true;

            DA.GetData(0, ref iMesh);
            DA.GetDataList(1, iU);
            DA.GetDataList(2, iφ);
            DA.GetData(3, ref iV);
            DA.GetData(4, ref iBending);

            //________________________________________________________________________________________________________________________

            //Check that mid nodes have been included
            List<int> UnusedVertexIndices = GetUnusedVertexIndices(iMesh);
            if (UnusedVertexIndices.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The mesh has does not included unused vertices. Please add additional vertices to the mesh which represent the mid edge nodes.");
            }


            //Create a list of vertices
            Point3dList vertices = new Point3dList(iMesh.Vertices.ToPoint3dArray());

            //For each face create a bilinear rectangular element
            List<Element> sigma1 = new List<Element>();
            List<Element> sigma2 = new List<Element>();

            //Set which type of deflections are inputted based on whether bending stress lines are requested or not
            List<Vector3d> def;
            if (iBending)
            {
                //If bending stress lines are requested then the rotational vectors are used
                if (iφ.Count != iMesh.Vertices.Count)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Rotational vectors count does not match mesh vertices count.");
                    return;
                }
                def = iφ;
            }
            else
            {
                if (iU.Count != iMesh.Vertices.Count)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Displacement vectors count does not match mesh vertices count.");
                    return;
                }
                def = iU;
            }

            for (int i = 0; i < iMesh.Faces.Count; i++)
            {
                MeshFace face = iMesh.Faces[i];

                //If the face is a quad then process it into a Quad8 element
                if (face.IsQuad)
                {
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

                    int p2 = vertices.ClosestIndex(point2);
                    int p4 = vertices.ClosestIndex(point4);
                    int p5 = vertices.ClosestIndex(point5);
                    int p7 = vertices.ClosestIndex(point7);

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

                    //Create and analyse elements
                    Quad8Element quad8ElementSigma1 = new Quad8Element(point1, point2, point3, point4, point5, point6, point7, point8, def[p1], def[p2], def[p3], def[p4], def[p5], def[p6], def[p7], def[p8], iV, !iBending);

                    //output data
                    quad8ElementSigma1.ChangeDirection(1);
                    sigma1.Add(new Element(quad8ElementSigma1));

                    Quad8Element quad8ElementSigma2 = new Quad8Element(point1, point2, point3, point4, point5, point6, point7, point8, def[p1], def[p2], def[p3], def[p4], def[p5], def[p6], def[p7], def[p8], iV, !iBending);

                    quad8ElementSigma2.ChangeDirection(2);
                    sigma2.Add(new Element(quad8ElementSigma2));
                }

                //IF the face is a triangle then process it into a Tri3 element
                if (face.IsTriangle)
                {
                    int p1 = face[0];
                    int p3 = face[1];
                    int p6 = face[2];

                    Point3d point1 = iMesh.Vertices[p1];
                    Point3d point3 = iMesh.Vertices[p3];
                    Point3d point6 = iMesh.Vertices[p6];

                    Point3d point2 = (point1 + point3) / 2;
                    Point3d point4 = (point1 + point6) / 2;
                    Point3d point5 = (point3 + point6) / 2;

                    int p2 = vertices.ClosestIndex(point2);
                    int p4 = vertices.ClosestIndex(point4);
                    int p5 = vertices.ClosestIndex(point5);

                    //Create and analyse elements
                    Tri6Element tri6ElementSigma1 = new Tri6Element(point1, point2, point3, point4, point5, point6, def[p1], def[p2], def[p3], def[p4], def[p5], def[p6], iV, !iBending);

                    //output data
                    tri6ElementSigma1.ChangeDirection(1);
                    sigma1.Add(new Element(tri6ElementSigma1));

                    Tri6Element tri6ElementSigma2 = new Tri6Element(point1, point2, point3, point4, point5, point6, def[p1], def[p2], def[p3], def[p4], def[p5], def[p6], iV, !iBending);

                    tri6ElementSigma2.ChangeDirection(2);
                    sigma2.Add(new Element(tri6ElementSigma2));
                }


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
        /// Finds any vertices in the mesh that are not used by any face and returns their indices
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        List<int> GetUnusedVertexIndices(Mesh mesh)
        {
            var used = new bool[mesh.Vertices.Count];

            // Mark all vertices referenced by faces
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                var f = mesh.Faces[i];
                used[f.A] = true;
                used[f.B] = true;
                used[f.C] = true;
                if (f.IsQuad) used[f.D] = true;
            }

            // Collect indices not used by any face
            var unused = new List<int>();
            for (int i = 0; i < used.Length; i++)
            {
                if (!used[i])
                    unused.Add(i);
            }

            return unused;
        }

        /// Assign component icon
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.IconCreateDeformationMeshHigher;
            }
        }

        /// Gets the unique ID for the component
        public override Guid ComponentGuid
        {
            get { return new Guid("68ddf900-f69b-42a1-9255-298aaa3ebfcb"); }
        }

        ///Set component to be in the FIRST group of the sub-category
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }
    }
}