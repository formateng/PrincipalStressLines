using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using LilyPad.Objects.ShapeFunction;

namespace LilyPad.Objects
{
    /// <summary>
    /// This class is used to house the type of integration method used.
    /// Previously there was more than one type of integration method but since the vector mesh has been removed this class is some what redundant.
    /// In the future this Class will be merged with the field Mesh class
    /// </summary>
    class PrincipalMesh
    {
        //Properties
        int Type;
        public FieldMesh FieldMesh;
        public Mesh Mesh;
        public Polyline[] NakedEdges;

        //Constructors

        public PrincipalMesh()
        {
            Type = 0;
        }

        //constructor for creating a principal mesh from a FieldMesh - previous version also had a constructor for the vector mesh type
        public PrincipalMesh(FieldMesh fieldMesh)
        {
            Type = 2;
            FieldMesh = fieldMesh;
            Mesh = fieldMesh.Mesh;
            NakedEdges = fieldMesh.NakedEdges;
        }

        //Methods
        public bool Evaluate(Point3d location, ref Vector3d vector)
        {
            if (Type == 2) return FieldMesh.Evaluate(location, ref vector);
            else return false;
        }
    }
}
