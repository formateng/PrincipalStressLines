using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Streamlines.NthOrder
{
    public class GH_CreateVectorMesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_CreatePrincipalMesh class.
        /// </summary>
        public GH_CreateVectorMesh()
          : base("Create Vector Mesh", "VectorMesh",
              "Creates a data type with a stored mesh and the vector values at the centre representing a principle direction",
              "Streamlines", "NthOrder")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh of principal domain", GH_ParamAccess.item);
            pManager.AddVectorParameter("Vectors", "V", "Principle direction at the centre of each face - add only one principal direction", GH_ParamAccess.list);
            pManager.AddNumberParameter("Radius", "R", "Radius used for vector interpolation", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddParameter(new GH_Param_PrincipalMesh(), "PrincipalMesh", "M", "Mesh with stored pricipal directions information", GH_ParamAccess.item);
            pManager.AddGenericParameter("PrincipalMesh", "M", "Mesh with stored principal directions information", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
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
            get { return new Guid("ada24819-203d-45bc-a777-f27244a3fd4d"); }
        }
    }
}