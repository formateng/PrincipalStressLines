using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Streamlines.ShapeFunction
{
    public class GH_TestingBilinearIsoPara : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_BilinearRectangle class.
        /// </summary>
        public GH_TestingBilinearIsoPara()
          : base("GH_TestingBilinearIsoPara", "Test",
              "",
              "Streamlines", "ShapeFunction")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Rectangular Mesh", GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "P", "Point", GH_ParamAccess.item);
            //pManager.AddVectorParameter("Displacment Vectors", "U", "Displacement vectors in a list of the same order as the mesh vertices", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Point", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh iMesh = new Mesh();
            Point3d iPoint = new Point3d();

            DA.GetData(0, ref iMesh);
            DA.GetData(1, ref iPoint);

            //________________________________________________________________________________________________________________________
            MeshFace face = iMesh.Faces[0];
            Point3d[] vertices = iMesh.Vertices.ToPoint3dArray();
            BilinearIsoPara bilinearIsoPara = new BilinearIsoPara(vertices[face[0]], vertices[face[1]], vertices[face[3]], vertices[face[2]], new Vector3d(), new Vector3d(), new Vector3d(), new Vector3d(), 0.0);

            Point3d oPoint = bilinearIsoPara.CalculateNaturalCoordinate(iPoint);

            //________________________________________________________________________________________________________________________

            DA.SetData(0, oPoint);
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
            get { return new Guid("68ddf900-f69b-41a1-9255-298eea6ebfcb"); }
        }
    }
}