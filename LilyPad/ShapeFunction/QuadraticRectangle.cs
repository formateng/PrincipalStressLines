using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;

namespace Streamlines.ShapeFunction
{
    class QuadraticRectangle
    {
        //Properties___________________________________________________________________________________________________________________________________________________
        private Point3d Centre;
        private double A;
        private double B;
        private BoundingBox BoundryBox;

        public PolylineCurve Bounds;
        public Vector3d U1;
        public Vector3d U2;
        public Vector3d U3;
        public Vector3d U4;
        public Vector3d U5;
        public Vector3d U6;
        public Vector3d U7;
        public Vector3d U8;
        public double v;

        public int Direction;

        //Functions
        private double N1x;
        private double N1y;
        private double N2x;
        private double N2y;
        private double N3x;
        private double N3y;
        private double N4x;
        private double N4y;
        private double N5x;
        private double N5y;
        private double N6x;
        private double N6y;
        private double N7x;
        private double N7y;
        private double N8x;
        private double N8y;

        private double EpsilonX;
        private double EpsilonY;
        private double GammaXY;

        private double SigmaX;
        private double SigmaY;
        private double TauXY;

        private double Theta;

        //Constructors___________________________________________________________________________________________________________________________________________________
        public QuadraticRectangle()
        {
            Centre = new Point3d();
            A = 0.0;
            B = 0.0;
            BoundryBox = new BoundingBox();
            Bounds = new PolylineCurve();
            U1 = new Vector3d();
            U2 = new Vector3d();
            U3 = new Vector3d();
            U4 = new Vector3d();
            U5 = new Vector3d();
            U6 = new Vector3d();
            U7 = new Vector3d();
            U8 = new Vector3d();
            v = 0.0;
            Direction = 0;
        }

        public QuadraticRectangle(PolylineCurve bounds, Vector3d u1, Vector3d u2, Vector3d u3, Vector3d u4, Vector3d u5, Vector3d u6, Vector3d u7, Vector3d u8, double poissons)
        {
            Bounds = bounds;
            U1 = u1;
            U2 = u2;
            U3 = u3;
            U4 = u4;
            U5 = u5;
            U6 = u6;
            U7 = u7;
            U8 = u8;
            v = poissons;

            BoundryBox = Bounds.GetBoundingBox(true);
            Centre = BoundryBox.Center;
            A = BoundryBox.Diagonal.X;
            B = BoundryBox.Diagonal.Y;

            Direction = 0;
        }

        //Methods___________________________________________________________________________________________________________________________________________________
        public void ChangeDirection(int dir)
        {
            Direction = dir;
        }

        public Vector3d Evaluate(Point3d location)
        {
            //This method runs the whole calculation process
            //calculate the shape function values
            CalculateShapeFunctionValues(location.X, location.Y);

            //calculate the strain values
            CalculateStrains();

            //calculate the stress values
            CalculateStresses();

            //Calculate theta
            CalculateTheta();

            //calculate the vector
            if (Direction == 1) return new Vector3d(Math.Cos(Theta), Math.Sin(Theta), 0);
            else if (Direction == 2) return new Vector3d(-Math.Sin(Theta), Math.Cos(Theta), 0);
            else return new Vector3d();
        }

        private void CalculateShapeFunctionValues(double x, double y)
        {
            N1x = 2 / (Math.Pow(A, 2) * B) * 2 * (x - Centre.X) * (y - Centre.Y) - 2 / (A * Math.Pow(B, 2)) * Math.Pow(y - Centre.Y, 2) + 1 / Math.Pow(A, 2) * 2 * (x - Centre.X) - 1 / (A * B) * (y - Centre.Y);
            N1y = 2 / (Math.Pow(A, 2) * B) * Math.Pow(x - Centre.X, 2) - 2 / (A * Math.Pow(B, 2)) * (x - Centre.X) * (y - Centre.Y) * 2 + 1 / Math.Pow(B, 2) * 2 * (y - Centre.Y) - 1 / (A * B) * (x - Centre.X);
            N2x = -4 / (Math.Pow(A, 2) * B) * 2 * (x - Centre.X) * (y - Centre.Y) - 2 / Math.Pow(A, 2) * 2 * (x - Centre.X);
            N2y = -4 / (Math.Pow(A, 2) * B) * Math.Pow(x - Centre.X, 2) + 1 / B * 1;
            N3x = 2 / (Math.Pow(A, 2) * B) * 2 * (x - Centre.X) * (y - Centre.Y) + 2 / (A * Math.Pow(B, 2)) * Math.Pow(y - Centre.Y, 2) + 1 / Math.Pow(A, 2) * 2 * (x - Centre.X) + 1 / (A * B) * (y - Centre.Y);
            N3y = 2 / (Math.Pow(A, 2) * B) * Math.Pow(x - Centre.X, 2) + 2 / (A * Math.Pow(B, 2)) * (x - Centre.X) * (y - Centre.Y) * 2 + 1 / Math.Pow(B, 2) * 2 * (y - Centre.Y) + 1 / (A * B) * (x - Centre.X);
            N4x = 4 / (A * Math.Pow(B, 2)) * Math.Pow(y - Centre.Y, 2) - 1 / A * 1;
            N4y = 4 / (A * Math.Pow(B, 2)) * (x - Centre.X) * (y - Centre.Y) * 2 - 2 / Math.Pow(B, 2) * 2 * (y - Centre.Y);
            N5x = -4 / (A * Math.Pow(B, 2)) * Math.Pow(y - Centre.Y, 2) + 1 / A * 1;
            N5y = -4 / (A * Math.Pow(B, 2)) * (x - Centre.X) * (y - Centre.Y) * 2 - 2 / Math.Pow(B, 2) * 2 * (y - Centre.Y);
            N6x = -2 / (Math.Pow(A, 2) * B) * 2 * (x - Centre.X) * (y - Centre.Y) - 2 / (A * Math.Pow(B, 2)) * Math.Pow(y - Centre.Y, 2) + 1 / Math.Pow(A, 2) * 2 * (x - Centre.X) + 1 / (A * B) * (y - Centre.Y);
            N6y = -2 / (Math.Pow(A, 2) * B) * Math.Pow(x - Centre.X, 2) - 2 / (A * Math.Pow(B, 2)) * (x - Centre.X) * (y - Centre.Y) * 2 + 1 / Math.Pow(B, 2) * 2 * (y - Centre.Y) + 1 / (A * B) * (x - Centre.X);
            N7x = 4 / (Math.Pow(A, 2) * B) * 2 * (x - Centre.X) * (y - Centre.Y) - 2 / Math.Pow(A, 2) * 2 * (x - Centre.X);
            N7y = 4 / (Math.Pow(A, 2) * B) * Math.Pow(x - Centre.X, 2) - 1 / B * 1;
            N8x = -2 / (Math.Pow(A, 2) * B) * 2 * (x - Centre.X) * (y - Centre.Y) + 2 / (A * Math.Pow(B, 2)) * Math.Pow(y - Centre.Y, 2) + 1 / Math.Pow(A, 2) * 2 * (x - Centre.X) - 1 / (A * B) * (y - Centre.Y);
            N8y = -2 / (Math.Pow(A, 2) * B) * Math.Pow(x - Centre.X, 2) + 2 / (A * Math.Pow(B, 2)) * (x - Centre.X) * (y - Centre.Y) * 2 + 1 / Math.Pow(B, 2) * 2 * (y - Centre.Y) - 1 / (A * B) * (x - Centre.X);


            /*
            //Creates the differentiated shape funcions where __x denotes a partial differetial to x and __y denotes a partial differetial to y
            N1x = "(2/(a^2*b)*2*(x-Xc)*(y-Yc)-2/(a*b^2)*(y-Yc)^2+1/(a^2)*2*(x-Xc)-1/(a*b)*(y-Yc))";
            N1y = "(2/(a^2*b)*(x-Xc)^2-2/(a*b^2)*(x-Xc)*(y-Yc)*2+1/(b^2)*2*(y-Yc)-1/(a*b)*(x-Xc))";
            N2x = "(-4/(a^2*b)*2*(x-Xc)*(y-Yc) -2/a^2*2*(x-Xc))";
            N2y = "(-4/(a^2*b)*(x-Xc)^2+1/b*1)";
            N3x = "(2/(a^2*b)*2*(x-Xc)*(y-Yc)+2/(a*b^2)*(y-Yc)^2+1/a^2*2*(x-Xc)+1/(a*b)*(y-Yc))";
            N3y = "(2/(a^2*b)*(x-Xc)^2+2/(a*b^2)*(x-Xc)*(y-Yc)*2+1/b^2*2*(y-Yc)+1/(a*b)*(x-Xc))";
            N4x = "(4/(a*b^2)*(y-Yc)^2-1/a*1)";
            N4y = "(4/(a*b^2)*(x-Xc)*(y-Yc)*2-2/b^2*2*(y-Yc))";
            N5x = "(-4/(a*b^2)*(y-Yc)^2+1/a*1)";
            N5y = "(-4/(a*b^2)*(x-Xc)*(y-Yc)*2-2/b^2*2*(y-Yc))";
            N6x = "(-2/(a^2*b)*2*(x-Xc)*(y-Yc)-2/(a*b^2)*(y-Yc)^2+1/a^2*2*(x-Xc)+1/(a*b)*(y-Yc))";
            N6y = "(-2/(a^2*b)*(x-Xc)^2-2/(a*b^2)*(x-Xc)*(y-Yc)*2+1/(b^2)*2*(y-Yc)+1/(a*b)*(x-Xc))";
            N7x = "(4/(a^2*b)*2*(x-Xc)*(y-Yc) -2/a^2*2*(x-Xc))";
            N7y = "(4/(a^2*b)*(x-Xc)^2-1/b*1)";
            N8x = "(-2/(a^2*b)*2*(x-Xc)*(y-Yc)+2/(a*b^2)*(y-Yc)^2+1/(a^2)*2*(x-Xc)-1/(a*b)*(y-Yc))";
            N8y = "(-2/(a^2*b)*(x-Xc)^2+2/(a*b^2)*(x-Xc)*(y-Yc)*2+1/b^2*2*(y-Yc)-1/(a*b)*(x-Xc))";
            */
        }

        private void CalculateStrains()
        {
            EpsilonX = N1x * U1.X + N2x * U2.X + N3x * U3.X + N4x * U4.X + N5x * U5.X + N6x * U6.X + N7x * U7.X + N8x * U8.X;
            EpsilonY = N1y * U1.Y + N2y * U2.Y + N3y * U3.Y + N4y * U4.Y + N5y * U5.Y + N6y * U6.Y + N7y * U7.Y + N8y * U8.Y;
            GammaXY = N1y * U1.X + N2y * U2.X + N3y * U3.X + N4y * U4.X + N5y * U5.X + N6y * U6.X + N7y * U7.X + N8y * U8.X + N1x * U1.Y + N2x * U2.Y + N3x * U3.Y + N4x * U4.Y + N5x * U5.Y + N6x * U6.Y + N7x * U7.Y + N8x * U8.Y;
        }

        private void CalculateStresses()
        {
            //Creates a string for each stress function
            SigmaX = (EpsilonX + v * EpsilonY);
            SigmaY = (EpsilonY + v * EpsilonX);
            TauXY = (((1 - v) / 2) * GammaXY);
        }

        private void CalculateTheta()
        {
            Theta = Math.Atan(2 * TauXY / (SigmaX - SigmaY)) / 2;

            if (SigmaY > (SigmaX + SigmaY) / 2)
            {
                if (TauXY > 0) Theta -= Math.PI / 2;
                else Theta += Math.PI / 2;
            }
        }
    }

}
