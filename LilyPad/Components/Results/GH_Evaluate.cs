using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using LilyPad.Objects;
using Rhino.Geometry;


namespace LilyPad.Components.Results
{
    public class GH_EvaluatePrincipalMesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_EvaluatePrincipalMesh class.
        /// base(Name, Nickname, Description, Folder, SubFolder)
        /// </summary>
        public GH_EvaluatePrincipalMesh()
          : base("Evaluate principal Mesh", "Eval",
              "Evaluates the principal direction at any point in the mesh",
              "LilyPad", "Results")
        {
        }

        /// Register all input parameters.
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Principal", "M", "Principal mesh to evaluate", GH_ParamAccess.item);
            pManager.AddPointParameter("Location", "P", "Location at which to evalutate the principal mesh", GH_ParamAccess.item);
        }

        /// Register all output parameters.
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Vector", "V", "Weighted average vector", GH_ParamAccess.item);
        }


        /// <summary>
        /// Assign parameter inputs
        /// run evaluate method of principalMesh and check for errors
        /// assign resulting vector to the output parameter
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Grasshopper.Kernel.Types.GH_ObjectWrapper objWrapPrinciMesh = new Grasshopper.Kernel.Types.GH_ObjectWrapper();

            PrincipalMesh iPrincipalMesh = new PrincipalMesh();
            Point3d iLocation = new Point3d();

            DA.GetData(0, ref objWrapPrinciMesh);
            DA.GetData(1, ref iLocation);

            if (objWrapPrinciMesh != null)
                iPrincipalMesh = objWrapPrinciMesh.Value as PrincipalMesh;

            //_________________________________________________________________________________

            Vector3d oVector = new Vector3d();
            bool test = iPrincipalMesh.Evaluate(iLocation, ref oVector);

            if (!test) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "no centres within a distance of the radius around the evaluation point");
            //___________________________________________________________________________________

            DA.SetData(0, oVector);

        }

        /// Assign component icon
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return ResourceIcons.IconEvaluate_1;
            }
        }

        /// Gets the unique ID for the component
        public override Guid ComponentGuid
        {
            get { return new Guid("f9e23d6a-368e-4d72-bf6f-6b52fe0220d7"); }
        }

        /// Set component to be in the FIRST group of the sub-category
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }
    }
}