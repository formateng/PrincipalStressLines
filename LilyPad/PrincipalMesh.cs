using Streamlines.ShapeFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Streamlines
{
    class PrincipalMesh
    {
        //Properties
        int Type;
        public VectorMesh VectorMesh;
        public FieldMesh FieldMesh;
        public Mesh Mesh;
        public Polyline[] NakedEdges;
        public Plane MeshPlane;

        //Constructors

        public PrincipalMesh()
        {
            Type = 0;
        }

        public PrincipalMesh(VectorMesh vectorMesh)
        {
            Type = 1;
            VectorMesh = vectorMesh;
            Mesh = vectorMesh.Mesh;
            NakedEdges = vectorMesh.NakedEdges;
            MeshPlane = vectorMesh.MeshPlane;
        }

        public PrincipalMesh(FieldMesh fieldMesh)
        {
            Type = 2;
            FieldMesh = fieldMesh;
            Mesh = fieldMesh.Mesh;
            NakedEdges = fieldMesh.NakedEdges;
            MeshPlane = fieldMesh.MeshPlane;
        }

        //Methods

        public bool Evaluate(Point3d location, ref Vector3d vector)
        {
            if (Type == 1) return VectorMesh.Evaluate(location, ref vector);
            else if (Type == 2) return FieldMesh.Evaluate(location, ref vector);
            else return false;
        }
    }
}
