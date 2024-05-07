using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Streamlines.ShapeFunction
{
    class FieldMesh
    {

        //Properties
        public Rhino.Geometry.Mesh Mesh;
        public Polyline[] NakedEdges;
        public Plane MeshPlane;
        private List<Element> Elements;

        //Constructors
        public FieldMesh()
        {
            Rhino.Geometry.Mesh Mesh = new Rhino.Geometry.Mesh();
            NakedEdges = new Polyline[1];
            Elements = new List<Element>();
            MeshPlane = new Plane();
        }

        public FieldMesh(List<Element> elements, Rhino.Geometry.Mesh mesh)
        {
            Mesh = mesh;
            Elements = elements;

            NakedEdges = Mesh.GetNakedEdges();

            MeshPlane = new Plane(Mesh.Vertices[0], Mesh.Vertices[1], Mesh.Vertices[2]);

            //Test if mesh is planar
            double testPlaneDist = 0.0;
            for (int i = 3; i < Mesh.Vertices.Count; i++)
            {
                testPlaneDist += MeshPlane.DistanceTo(Mesh.Vertices[i]);
            }
            if (testPlaneDist > 0.0001) throw new NotImplementedException("Curret functionality only works for planar meshes");
        }

        //Methods
        /// <summary>
        /// Evaluates the vector a specfic point inside the mesh by evaluating field equations for that coordinate.
        /// </summary>
        public bool Evaluate(Point3d location, ref Vector3d direction)
        {
            //Find which face the point is located on
            MeshPoint closesPoint = Mesh.ClosestMeshPoint(location, 0.0);
            location = closesPoint.Point;

            //Using the face which the point is located on, evaluate the expression for that element
            direction = new Vector3d();

            direction = Elements[closesPoint.FaceIndex].Evaluate(location);

            return true;
        }

    }
}
