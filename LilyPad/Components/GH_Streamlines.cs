using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Streamlines.NthOrder
{
    public class GH_StreamlinesVectorMesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_StreamlinesPrincipalMesh class.
        /// </summary>
        public GH_StreamlinesVectorMesh()
          : base("Streamlines", "Streamlines",
              "Creates a series of streamlines on the principal mesh",
              "Streamlines", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Principal Mesh", "Mesh", "Principal mesh to analyse", GH_ParamAccess.item);
            pManager.AddPointParameter("Seed", "S", "Point which the streamline passes through", GH_ParamAccess.item);
            pManager.AddNumberParameter("Step Tolerance", "T", "Size of the step between successive points", GH_ParamAccess.item);
            pManager.AddNumberParameter("Integration Method", "IM", "Integration method used to generate the streamline: 'Euler=1' 'RK2=2' 'RK3=3' 'RK4=4'", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Max. Error", "Err", "If value > 0 then an adaptive step procedure is used where the step size is decreased if the estimated integration error is more than the specified value", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Seeding Method", "SM", "Seeding strategy used to generate new seeding point: 'Neighbour =1'......", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Seperation", "dSep", "The seperation distance of new seed points", GH_ParamAccess.item);
            pManager.AddNumberParameter("Test Dist.", "dTest", "The distance which a streamline is ended if it is closer than this distance to a existing streamline, this is only applied if the specified value is >0", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Streamlines", "S", "Steamlines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Grasshopper.Kernel.Types.GH_ObjectWrapper objWrapPrinciMesh = new Grasshopper.Kernel.Types.GH_ObjectWrapper();


            PrincipalMesh iPrincipalMesh = new PrincipalMesh();
            Point3d iSeed = new Point3d();
            double iStepSize = 0.0;
            double iMethod = 0;
            double iMaxAngle = 0.0;
            double iStrategy = 0;
            double iDSep = 0.0;
            double iDTest = 0.0;

            DA.GetData(0, ref objWrapPrinciMesh);
            DA.GetData(1, ref iSeed);
            DA.GetData(2, ref iStepSize);
            DA.GetData(3, ref iMethod);
            DA.GetData(4, ref iMaxAngle);
            DA.GetData(5, ref iStrategy);
            DA.GetData(6, ref iDSep);
            DA.GetData(7, ref iDTest);

            if (objWrapPrinciMesh != null)
                iPrincipalMesh = objWrapPrinciMesh.Value as PrincipalMesh;

            //_________________________________________________________________________________
            Streamlines streamlines = new Streamlines(iPrincipalMesh);

            List<Polyline> oStreamlines = streamlines.CreateStreamlines(iSeed, iStepSize, Convert.ToInt32(iMethod), Convert.ToInt32(iStrategy), iDSep, iDTest, iMaxAngle);

            //___________________________________________________________________________________

            DA.SetDataList(0, oStreamlines);

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
            get { return new Guid("31cc7a61-5877-4d3e-8627-3d653e415f54"); }
        }
    }
}