using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;
using LilyPad.Objects;
using LilyPad.Objects.ShapeFunction;

namespace LilyPad.Components.Setup
{
    public class GH_CreateDeformationMesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_MembraneQuad4 class.
        /// base(Name, Nickname, Description, Folder, SubFolder)
        /// </summary>
        public GH_CreateDeformationMesh()
          : base("Create Deformation Mesh", "DefMesh",
              "Transforms Mesh and results into elements for principal stress line analysis using the theory of FEA shape functions",
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

                //If the face is a quad then process it into a Quad4 element
                if (face.IsQuad)
                {
                    int p1 = face[0];
                    int p2 = face[1];
                    int p3 = face[3];
                    int p4 = face[2];

                    Point3d point1 = iMesh.Vertices[p1];
                    Point3d point2 = iMesh.Vertices[p2];
                    Point3d point3 = iMesh.Vertices[p3];
                    Point3d point4 = iMesh.Vertices[p4];

                    // Fit a plane to the points and find the maximum distance from the plane to the points
                    Plane fitPlane;
                    double maxDeviation;
                    if (Plane.FitPlaneToPoints(new List<Point3d> { point1, point2, point3, point4 }, out fitPlane, out maxDeviation) != 0)
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
                    Quad4Element quad4ElementSigma1 = new Quad4Element(point1, point2, point3, point4, def[p1], def[p2], def[p3], def[p4], iV, !iBending);

                    //output data
                    quad4ElementSigma1.ChangeDirection(1);
                    sigma1.Add(new Element(quad4ElementSigma1));

                    Quad4Element quad4ElementSigma2 = new Quad4Element(point1, point2, point3, point4, def[p1], def[p2], def[p3], def[p4], iV, !iBending);

                    quad4ElementSigma2.ChangeDirection(2);
                    sigma2.Add(new Element(quad4ElementSigma2));
                }

                //IF the face is a triangle then process it into a Tri3 element
                if (face.IsTriangle)
                {
                    int p1 = face[0];
                    int p2 = face[1];
                    int p3 = face[2];

                    Point3d point1 = iMesh.Vertices[p1];
                    Point3d point2 = iMesh.Vertices[p2];
                    Point3d point3 = iMesh.Vertices[p3];

                    //Create and analyse elements
                    Tri3Element tri3ElementSigma1 = new Tri3Element(point1, point2, point3, def[p1], def[p2], def[p3], iV, !iBending);

                    //output data
                    tri3ElementSigma1.ChangeDirection(1);
                    sigma1.Add(new Element(tri3ElementSigma1));

                    Tri3Element tri3ElementSigma2 = new Tri3Element(point1, point2, point3, def[p1], def[p2], def[p3], iV, !iBending);

                    tri3ElementSigma2.ChangeDirection(2);
                    sigma2.Add(new Element(tri3ElementSigma2));
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

        /// Assign component icon
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.IconMembraneQuad4;
            }
        }

        /// Gets the unique ID for the component
        public override Guid ComponentGuid
        {
            get { return new Guid("68ddf900-f69b-42a1-9255-298eea3ebfcb"); }
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