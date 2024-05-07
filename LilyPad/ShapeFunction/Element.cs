using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamlines.ShapeFunction
{
    class Element
    {
        //Properties

        int Type;
        BilinearRectangle BilinearRectangle;
        QuadraticRectangle QuadraticRectangle;
        BilinearIsoPara BilinearIsoPara;
        QuadraticIsoPara QuadraticIsoPara;

        //Constructors

        public Element()
        {
            Type = 0;
        }

        public Element(BilinearRectangle bilinearRectangle)
        {
            Type = 1;
            BilinearRectangle = bilinearRectangle;
        }

        public Element(QuadraticRectangle quadraticRectangle)
        {
            Type = 2;
            QuadraticRectangle = quadraticRectangle;
        }

        public Element(BilinearIsoPara bilinearIsoPara)
        {
            Type = 3;
            BilinearIsoPara = bilinearIsoPara;
        }
        public Element(QuadraticIsoPara quadraticIsoPara)
        {
            Type = 4;
            QuadraticIsoPara = quadraticIsoPara;
        }

        //Methods

        public Vector3d Evaluate(Point3d location)
        {
            if (Type == 1) return BilinearRectangle.Evaluate(location);
            else if (Type == 2) return QuadraticRectangle.Evaluate(location);
            else if (Type == 3) return BilinearIsoPara.Evaluate(location);
            else if (Type == 4) return QuadraticIsoPara.Evaluate(location);
            else return new Vector3d();
        }
    }
}
