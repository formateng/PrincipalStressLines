using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilyPad.Objects.ShapeFunction
{
    /// <summary>
    /// Contains any element type and can be used to evaluate principal stress directions within the element
    /// </summary>
    class Element
    {
        //Properties

        int Type;
        Quad4Element Quad4Element;
        Quad8Element Quad8Element;

        //Constructors
        public Element()
        {
            Type = 0;
        }

        public Element(Quad4Element quad4Element)
        {
            Type = 1;
            Quad4Element = quad4Element;
        }
        public Element(Quad8Element quad8Element)
        {
            Type = 2;
            Quad8Element = quad8Element;
        }

        //Methods

        public Vector3d Evaluate(Point3d location)
        {
            if (Type == 1) return Quad4Element.Evaluate(location);
            else if (Type == 2) return Quad8Element.Evaluate(location);
            else return new Vector3d();
        }
    }
}
