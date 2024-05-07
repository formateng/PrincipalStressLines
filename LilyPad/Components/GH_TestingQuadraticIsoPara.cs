using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Streamlines.ShapeFunction
{
    public class GH_TestingQuadraticIsoPara : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_BilinearRectangle class.
        /// </summary>
        public GH_TestingQuadraticIsoPara()
          : base("GH_TestingQuadraticIsoPara", "Test",
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

            Point3d p1 = vertices[face[0]];
            Point3d p3 = vertices[face[1]];
            Point3d p6 = vertices[face[3]];
            Point3d p8 = vertices[face[2]];

            Point3d p2 = (p1 + p3) / 2;
            Point3d p4 = (p1 + p6) / 2;
            Point3d p5 = (p3 + p8) / 2;
            Point3d p7 = (p6 + p8) / 2;


            QuadraticIsoPara quadraticIsoPara = new QuadraticIsoPara(p1, p2, p3, p4, p5, p6, p7, p8, new Vector3d(), new Vector3d(), new Vector3d(), new Vector3d(), new Vector3d(), new Vector3d(), new Vector3d(), new Vector3d(), 0.0);

            Point3d oPoint = quadraticIsoPara.CalculateNaturalCoordinate(iPoint);

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
            get { return new Guid("68ddf900-f69b-41a1-9355-298eea6ebfcb"); }
        }
    }
}