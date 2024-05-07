using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Streamlines.NthOrder
{
    public class GH_RadiusChange : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_RadiusChange class.
        /// </summary>
        public GH_RadiusChange()
          : base("Change Radius", "RadiusChange",
              "Alters the input radius of a vector mesh.",
              "Streamlines", "NthOrder")

        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Vector Mesh", "M", "Vector mesh to change", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "R", "Radius used for vector interpolation", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Vector Mesh", "M", "Corrected Vector mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            PrincipalMesh iPrincipalMesh = new PrincipalMesh();
            double iRadius = 0.0;

            DA.GetData(0, ref iPrincipalMesh);
            DA.GetData(1, ref iRadius);

            //_________________________________________________________________________________
            //unwrap vectormesh
            VectorMesh vectorMesh = iPrincipalMesh.VectorMesh;

            //copy data to prevent changes to input param
            VectorMesh copyVectorMesh = new VectorMesh(vectorMesh);

            copyVectorMesh.ChangeRadius(iRadius);
            PrincipalMesh oMesh = new PrincipalMesh(copyVectorMesh);

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
            get { return new Guid("4ff6d5df-cb02-4a83-9beb-e7d31da62c2a"); }
        }
    }
}