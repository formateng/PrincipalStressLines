using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;

namespace Streamlines.ShapeFunction
{
    public class GH_MindlinReissnerBilinearRectangle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_BilinearRectangle class.
        /// </summary>
        public GH_MindlinReissnerBilinearRectangle()
          : base("4 Node Rectangular Slab", "4RecSlab",
              "Transforms Mesh and results into 4-node rectangular slab elements for principal bending stress line analysis using the Mindlin-Reissner Plate Theory and Bilinear shape functions. ***NOTE THAT***: the rotational axes are defined by the right hand rule",
              "Streamlines", "ShapeFunction")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Rectangular Mesh", GH_ParamAccess.item);
            pManager.AddVectorParameter("Rotation φ1", "φ1", "Rotation vector for rotations about the x and y axes at the **TOP LEFT** point of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Rotation φ2", "φ2", "Rotation vector for rotations about the x and y axes at the **TOP RIGHT** point of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Rotation φ3", "φ3", "Rotation vector for rotations about the x and y axes at the **BOTTOM LEFT** point of mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("Rotation φ4", "φ4", "Rotation vector for rotations about the x and y axes at the **BOTTOM RIGHT** point of mesh faces", GH_ParamAccess.list);
            pManager.AddNumberParameter("Poisson's ratio", "v", "Poisson's ratio", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Principle Direction 1", "P1", "A vector field showing the first principle direction", GH_ParamAccess.item);
            pManager.AddGenericParameter("Principle Direction 2", "P2", "A vector field showing the second principle direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh iMesh = new Mesh();
            List<Vector3d> iφ1 = new List<Vector3d>();
            List<Vector3d> iφ2 = new List<Vector3d>();
            List<Vector3d> iφ3 = new List<Vector3d>();
            List<Vector3d> iφ4 = new List<Vector3d>();

            double iV = 0.0;

            DA.GetData(0, ref iMesh);
            DA.GetDataList(1, iφ1);
            DA.GetDataList(2, iφ2);
            DA.GetDataList(3, iφ3);
            DA.GetDataList(4, iφ4);
            DA.GetData(5, ref iV);

            //________________________________________________________________________________________________________________________

            //For each face create a bilinear rectangular element
            List<Element> sigma1 = new List<Element>();
            List<Element> sigma2 = new List<Element>();

            for (int i = 0; i < iMesh.Faces.Count; i++)
            {
                MeshFace face = iMesh.Faces[i];

                Vector3d U1 = new Vector3d(iφ1[i].Y, -iφ1[i].X, 0.0);
                Vector3d U2 = new Vector3d(iφ2[i].Y, -iφ2[i].X, 0.0);
                Vector3d U3 = new Vector3d(iφ3[i].Y, -iφ3[i].X, 0.0);
                Vector3d U4 = new Vector3d(iφ4[i].Y, -iφ4[i].X, 0.0);

                //create polyline of the face edges
                List<Point3d> points = new List<Point3d>();
                for (int j = 0; j < 4; j++) points.Add(iMesh.Vertices[face[j]]);
                points.Add(iMesh.Vertices[face[0]]);
                Polyline bounds = new Polyline(points);

                //Create and analyse elements
                BilinearRectangle bilinearRectangle1 = new BilinearRectangle(bounds.ToPolylineCurve(), U1, U2, U3, U4, iV);

                //output data
                bilinearRectangle1.ChangeDirection(1);
                sigma1.Add(new Element(bilinearRectangle1));

                BilinearRectangle bilinearRectangle2 = new BilinearRectangle(bounds.ToPolylineCurve(), U1, U2, U3, U4, iV);

                bilinearRectangle2.ChangeDirection(2);
                sigma2.Add(new Element(bilinearRectangle2));
            }

            //Creates FieldMesh data for output
            FieldMesh Sigma1 = new FieldMesh(sigma1, iMesh);
            FieldMesh Sigma2 = new FieldMesh(sigma2, iMesh);

            //Turn the field meshes into principalMeshes
            PrincipalMesh oSigma1 = new PrincipalMesh(Sigma1);
            PrincipalMesh oSigma2 = new PrincipalMesh(Sigma2);


            //________________________________________________________________________________________________________________________

            DA.SetData(0, oSigma1);
            DA.SetData(1, oSigma2);
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
            get { return new Guid("68ddf900-f69b-41a1-9355-298eea3ebfcb"); }
        }
    }
}