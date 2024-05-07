using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Streamlines.NthOrder
{
    public class GH_VectorCorrection : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_VectorCorrection class.
        /// </summary>
        public GH_VectorCorrection()
          : base("Vector Correction", "Vec Correct",
              "Flips the principal vectors 180° when they are not aligned with adjacent vectors",
              "Streamlines", "NthOrder")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Vector Mesh", "M", "Vector mesh to correct", GH_ParamAccess.item);
            pManager.AddNumberParameter("Priority List", "P", "List of face indices priority which flips vectors based only on the directions of vectors earlier in the list", GH_ParamAccess.list);
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
            List<double> iPriority = new List<double>();
            PrincipalMesh iPrincipalMesh = new PrincipalMesh();

            DA.GetData(0, ref iPrincipalMesh);
            DA.GetDataList(1, iPriority);

            //_________________________________________________________________________________
            //unwrap vectormesh
            VectorMesh vectorMesh = iPrincipalMesh.VectorMesh;

            //copy data to prevent changes to input param
            VectorMesh copyVectorMesh = new VectorMesh(vectorMesh);

            List<int> priority = iPriority.ConvertAll(new Converter<double, int>(DoubleToInt));
            copyVectorMesh.VectorCorrection(priority);
            PrincipalMesh oMesh = new PrincipalMesh(copyVectorMesh);

            //___________________________________________________________________________________

            DA.SetData(0, oMesh);

        }

        public static int DoubleToInt(double d)
        {
            return Convert.ToInt32(d);
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
            get { return new Guid("591242f6-9a3a-458f-83a0-0388f8edec9a"); }
        }
    }
}