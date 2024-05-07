using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;

namespace Streamlines.ShapeFunction
{
    public class GH_MembraneBilinearIsoPara : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_BilinearRectangle class.
        /// </summary>
        public GH_MembraneBilinearIsoPara()
          : base("4 Node Isoparametric Membrane", "4IsoparaMem",
              "Transforms Mesh and results into 4-node isoparametric (i.e. any quadrilateral shape) membrane elements for principal stress line analysis using the theory of in-plane loaded plates and Bilinear shape functions",
              "Streamlines", "ShapeFunction")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddVectorParameter("Displacments", "U", "Displacement vectors for each mesh vertex in a list sorted in the same order as the mesh vertices", GH_ParamAccess.list);
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
            List<Vector3d> iU = new List<Vector3d>();
            double iV = 0.0;

            DA.GetData(0, ref iMesh);
            DA.GetDataList(1, iU);
            DA.GetData(2, ref iV);

            //________________________________________________________________________________________________________________________

            //For each face create a bilinear rectangular element
            List<Element> sigma1 = new List<Element>();
            List<Element> sigma2 = new List<Element>();

            for (int i = 0; i < iMesh.Faces.Count; i++)
            {
                MeshFace face = iMesh.Faces[i];
                int p1 = face[0];
                int p2 = face[1];
                int p3 = face[3];
                int p4 = face[2];

                //Create and analyse elements
                BilinearIsoPara bilinearIsoPara1 = new BilinearIsoPara(iMesh.Vertices[p1], iMesh.Vertices[p2], iMesh.Vertices[p3], iMesh.Vertices[p4], iU[p1], iU[p2], iU[p3], iU[p4], iV);

                //output data
                bilinearIsoPara1.ChangeDirection(1);
                sigma1.Add( new Element(bilinearIsoPara1));

                BilinearIsoPara bilinearIsoPara2 = new BilinearIsoPara(iMesh.Vertices[p1], iMesh.Vertices[p2], iMesh.Vertices[p3], iMesh.Vertices[p4], iU[p1], iU[p2], iU[p3], iU[p4], iV);

                bilinearIsoPara2.ChangeDirection(2);
                sigma2.Add( new Element(bilinearIsoPara2));
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
            get { return new Guid("68ddf900-f69b-42a1-9255-298eea3ebfcb"); }
        }
    }
}