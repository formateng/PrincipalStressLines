﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;
using LilyPad.Objects;
using LilyPad.Objects.ShapeFunction;

namespace LilyPad.Components.Setup
{
    public class GH_MindlinReissnerQuad4 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_MindlinReissnerQuad4 class.
        /// base(Name, Nickname, Description, Folder, SubFolder)
        /// </summary>
        public GH_MindlinReissnerQuad4()
          : base("4 Node Quad Slab", "Quad4Slab",
              "Transforms Mesh and results into 4-node quad slab elements for principal bending stress line analysis using the Mindlin-Reissner Plate Theory and Bilinear shape functions. ***NOTE THAT***: the rotational axes are defined by the right hand rule",
              "LilyPad", " Setup")
        {
        }

        /// Register all input parameters.
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddVectorParameter("Rotations", "φ", "Rotational vectors for each mesh vertex in a list sorted in the same order as the mesh vertices ***NOTE THAT***: the rotational axes are defined by the right hand rule", GH_ParamAccess.list);
            pManager.AddNumberParameter("Poisson's ratio", "v", "Poisson's ratio", GH_ParamAccess.item);
        }

        /// Register all output parameters.
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Principle Direction 1", "P1", "A vector field showing the first principle direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Principle Direction 2", "P2", "A vector field showing the second principle direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// Uses the provided mesh to create a series of quad 4 elements
        /// Assembles list of elements into two principal meshes one for each direction
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh iMesh = new Mesh();
            List<Vector3d> iφ = new List<Vector3d>();
            double iV = 0.0;

            DA.GetData(0, ref iMesh);
            DA.GetDataList(1, iφ);
            DA.GetData(2, ref iV);

            //________________________________________________________________________________________________________________________

            //For each face create a bilinear rectangular element
            List<Element> sigma1 = new List<Element>();
            List<Element> sigma2 = new List<Element>();

            for (int i = 0; i < iMesh.Faces.Count; i++)
            {
                MeshFace face = iMesh.Faces[i];

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

                //alter vectors so they are in the "mathematical form" rather than the right-hand rule
                Vector3d U1 = iφ[p1];
                Vector3d U2 = iφ[p2];
                Vector3d U3 = iφ[p3];
                Vector3d U4 = iφ[p4];

                //Create and analyse elements
                Quad4Element bilinearIsoPara1 = new Quad4Element(point1, point2, point3, point4, U1, U2, U3, U4, iV, false);

                //output data
                bilinearIsoPara1.ChangeDirection(1);
                sigma1.Add(new Element(bilinearIsoPara1));

                Quad4Element bilinearIsoPara2 = new Quad4Element(point1, point2, point3, point4, U1, U2, U3, U4, iV, false);

                bilinearIsoPara2.ChangeDirection(2);
                sigma2.Add(new Element(bilinearIsoPara2));
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
                return Properties.Resources.IconSlabQuad4;
            }
        }

        /// Gets the unique ID for the component
        public override Guid ComponentGuid
        {
            get { return new Guid("78ddf910-f69b-42a1-9255-298eea3ebfcb"); }
        }

        ///Set component to be in the SECOND group of the sub-category
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }
    }
}