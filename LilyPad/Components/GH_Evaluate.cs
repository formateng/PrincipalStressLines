using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;


namespace Streamlines.NthOrder
{
    public class GH_EvaluateVectorMesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_EvaluatePrincipalMesh class.
        /// </summary>
        public GH_EvaluateVectorMesh()
          : base("Evaluate principal Mesh", "Eval",
              "Evaluates the principal direction at any point in the mesh",
              "Streamlines", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Principal", "M", "Principal mesh to evaluate", GH_ParamAccess.item);
            pManager.AddPointParameter("Location", "P", "Location at which to evalutate the principal mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Vector", "V", "Weighted average vector", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            PrincipalMesh iPrincipalMesh = new PrincipalMesh();
            Point3d iLocation = new Point3d();

            DA.GetData(0, ref iPrincipalMesh);
            DA.GetData(1, ref iLocation);

            //_________________________________________________________________________________

            Vector3d oVector = new Vector3d();
            bool test = iPrincipalMesh.Evaluate(iLocation, ref oVector);

            if (!test) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "no centres within a distance of the radius around the evaluation point");
            //___________________________________________________________________________________

            DA.SetData(0, oVector);

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
            get { return new Guid("f9e23d6a-368e-4d72-bf6f-6b52fe0220d7"); }
        }
    }
}