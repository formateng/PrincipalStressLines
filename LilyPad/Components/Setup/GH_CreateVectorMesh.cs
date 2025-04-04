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
            pManager.AddVectorParameter("Vectors", "V", "Principle direction at the centre of each face - add only one principal direction", GH_ParamAccess.list);
            pManager.AddNumberParameter("Radius", "R", "Radius used for vector interpolation", GH_ParamAccess.item);
        }

        /// Register all output parameters.
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            //pManager.AddParameter(new GH_Param_PrincipalMesh(), "PrincipalMesh", "M", "Mesh with stored pricipal directions information", GH_ParamAccess.item);
            pManager.AddGenericParameter("PrincipalMesh", "M", "Mesh with stored principal directions information", GH_ParamAccess.list);
        }

        /// <summary>
        /// Uses the provided mesh to create a series of quad 8 elements
        /// Assembles list of elements into two principal meshes one for each direction
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh iMesh = new Mesh();
            List<Vector3d> iVectors = new List<Vector3d>();
            double iRadius = 0.0;

            DA.GetData(0, ref iMesh);
            DA.GetDataList(1, iVectors);
            DA.GetData(2, ref iRadius);

            //_________________________________________________________________________________

            VectorMesh vectorMesh = new VectorMesh(iMesh, iVectors, iRadius);
            PrincipalMesh oMesh = new PrincipalMesh(vectorMesh);

            //___________________________________________________________________________________

            DA.SetData(0, oMesh);
        }


        /// Assign component icon
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
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