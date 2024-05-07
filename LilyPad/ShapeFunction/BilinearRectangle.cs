using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel.Expressions;

namespace Streamlines.ShapeFunction
{
    class BilinearRectangle
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

        private double EpsilonX;
        private double EpsilonY;
        private double GammaXY;

        private double SigmaX;
        private double SigmaY;
        private double TauXY;

        private double Theta;

        //Constructors___________________________________________________________________________________________________________________________________________________
        public BilinearRectangle()
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
            v = 0.0;
            Direction = 0;
        }

        public BilinearRectangle(PolylineCurve bounds, Vector3d u1, Vector3d u2, Vector3d u3, Vector3d u4, double poissons)
        {
            Bounds = bounds;
            U1 = u1;
            U2 = u2;
            U3 = u3;
            U4 = u4;
            v = poissons;

            BoundryBox = Bounds.GetBoundingBox(true);
            Centre = BoundryBox.Center;
            A = BoundryBox.Diagonal.X;
            B = BoundryBox.Diagonal.Y;

            Direction = 0;
        }

        //Methods___________________________________________________________________________________________________________________________________________________

        //sets direction
        public void ChangeDirection(int dir)
        {
            Direction = dir;
        }

        public Vector3d Evaluate(Point3d location)
        {
            //This method runs the whole calculation process
            //calculate the shape function values
            CalculateShapeFunctionValues(location.X,location.Y);

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
            //Creates the differentiated shape funcions where __x denotes a partial differetial to x and __y denotes a partial differetial to y
            N1x = -1 / (A * B) * (y - Centre.Y) - 1 / (2 * A);
            N1y = -1 / (A * B) * (x - Centre.X) + 1 / (2 * B);
            N2x = +1 / (A * B) * (y - Centre.Y) + 1 / (2 * A);
            N2y = +1 / (A * B) * (x - Centre.X) + 1 / (2 * B);
            N3x = +1 / (A * B) * (y - Centre.Y) - 1 / (2 * A);
            N3y = +1 / (A * B) * (x - Centre.X) - 1 / (2 * B);
            N4x = -1 / (A * B) * (y - Centre.Y) + 1 / (2 * A);
            N4y = -1 / (A * B) * (x - Centre.X) - 1 / (2 * B);
        }

        private void CalculateStrains()
        {
            EpsilonX = N1x * U1.X + N2x * U2.X + N3x * U3.X + N4x * U4.X;
            EpsilonY = N1y * U1.Y + N2y * U2.Y + N3y * U3.Y + N4y * U4.Y;
            GammaXY = N1y * U1.X + N2y * U2.X + N3y * U3.X + N4y * U4.X + N1x * U1.Y + N2x * U2.Y + N3x * U3.Y + N4x * U4.Y;
        }

        private void CalculateStresses()
        {
            //Creates a string for each stress function
            SigmaX = (EpsilonX + v * EpsilonY);
            SigmaY = (EpsilonY+ v*EpsilonX);
            TauXY = (((1-v)/2)*GammaXY);
        }

        private void CalculateTheta()
        {
            Theta = Math.Atan(2*TauXY/(SigmaX - SigmaY))/2;

            if (SigmaY > (SigmaX + SigmaY) / 2)
            {
                if (TauXY > 0) Theta -= Math.PI / 2;
                else Theta += Math.PI / 2;
            } 
        }
    }
}
