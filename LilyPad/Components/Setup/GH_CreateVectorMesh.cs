using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;
using Rhino.Collections;
using LilyPad.Objects;
using LilyPad.Objects.ShapeFunction;
using LilyPad.Objects.NthOrder;

namespace LilyPad.Components.Setup
{
    public class GH_CreateVectorMesh : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public GH_CreateVectorMesh()
          : base("Create Vector Mesh", "VectorMesh",
              "Creates a data type with a stored mesh and the vector values at the centre representing a principle direction",
              "LilyPad", " Setup")
        {
        }

        /// Register all input parameters.
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh of principal domain", GH_ParamAccess.item);
            pManager.AddVectorParameter("Vectors 1st", "V1", "Principle direction at the centre of each face - In the FIRST principal direction", GH_ParamAccess.list);
            pManager.AddVectorParameter("Vectors 2nd", "V2", "Principle direction at the centre of each face - In the SECOND principal direction", GH_ParamAccess.list);
            pManager.AddNumberParameter("Radius", "R", "Radius used for vector smoothing", GH_ParamAccess.item);
        }

        /// Register all output parameters.
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Principal Direction 1", "P1", "A vector field showing the first principle direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Principal Direction 2", "P2", "A vector field showing the second principle direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// Uses the provided mesh to create a series of quad 8 elements
        /// Assembles list of elements into two principal meshes one for each direction
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh iMesh = new Mesh();
            List<Vector3d> iVectors1 = new List<Vector3d>();
            List<Vector3d> iVectors2 = new List<Vector3d>();
            double iRadius = 0.0;

            DA.GetData(0, ref iMesh);
            DA.GetDataList(1, iVectors1);
            DA.GetDataList(2, iVectors2);
            DA.GetData(3, ref iRadius);

            //_________________________________________________________________________________

            VectorMesh vectorMesh1 = new VectorMesh(iMesh, iVectors1, iRadius);
            VectorMesh vectorMesh2 = new VectorMesh(iMesh, iVectors2, iRadius);
            PrincipalMesh oSigma1 = new PrincipalMesh(vectorMesh1);
            PrincipalMesh oSigma2 = new PrincipalMesh(vectorMesh2);

            //___________________________________________________________________________________

            DA.SetData(0, oSigma1);
            DA.SetData(1, oSigma2);
        }


        /// Assign component icon
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.VectorMesh;
            }
        }

        /// Gets the unique ID for the component
        public override Guid ComponentGuid
        {
            get { return new Guid("48dde900-f69b-41b1-9255-298eea3ebfcb"); }
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