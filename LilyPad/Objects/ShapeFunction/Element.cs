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
        Tri3Element Tri3Element;
        Tri6Element Tri6Element;
        Quad4Element Quad4Element;
        Quad8Element Quad8Element;

        //Constructors
        public Element()
        {
            Type = 0;
        }

        public Element(Quad4Element quad4Element)
        {
            Type = 4;
            Quad4Element = quad4Element;
        }
        public Element(Quad8Element quad8Element)
        {
            Type = 8;
            Quad8Element = quad8Element;
        }
        public Element(Tri3Element tri3Element)
        {
            Type = 3;
            Tri3Element = tri3Element;
        }

        public Element(Tri6Element tri6Element)
        {
            Type = 6;
            Tri6Element = tri6Element;
        }

        //Methods

        public Vector3d Evaluate(Point3d location)
        {
            if (Type == 4) return Quad4Element.Evaluate(location);
            else if (Type == 8) return Quad8Element.Evaluate(location);
            else if (Type == 3) return Tri3Element.Evaluate(location);
            else if (Type == 6) return Tri6Element.Evaluate(location);
            else return new Vector3d();
        }
    }
}
