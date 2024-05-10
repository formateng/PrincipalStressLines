using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace LilyPad.Objects
{
    class Delaunay
    {
        //Properties_____________________________________________________________________________________________________________________________________________________________________________
        public List<Point3d> pts;

        //Constructors___________________________________________________________________________________________________________________________________________________________________________
        public Delaunay()
        {
            pts = new List<Point3d>();
        }

        public Delaunay(List<Point3d> Points)
        {
            pts = Points;
        }

        //Methods________________________________________________________________________________________________________________________________________________________________________________
        public Mesh mesh()
        {
            //convert point3d to node2
            var nodes = new Grasshopper.Kernel.Geometry.Node2List();
            for (int i = 0; i < pts.Count; i++)
            {
                nodes.Append(new Grasshopper.Kernel.Geometry.Node2(pts[i].X, pts[i].Y));
            }

            //solve Delaunay
            var delMesh = new Mesh();
            var faces = new List<Grasshopper.Kernel.Geometry.Delaunay.Face>();

            faces = Grasshopper.Kernel.Geometry.Delaunay.Solver.Solve_Faces(nodes, 1);

            //output
            delMesh = Grasshopper.Kernel.Geometry.Delaunay.Solver.Solve_Mesh(nodes, 1, ref faces);
            return delMesh;
        }
    }
}
