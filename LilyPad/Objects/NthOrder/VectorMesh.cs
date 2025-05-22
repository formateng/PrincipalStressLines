using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Complex;
using Rhino.Geometry;
using Grasshopper.Kernel;
using System.Windows.Forms;
using MathNet.Numerics.Financial;
using TriangleNet.Data;
using TriangleNet;
using Rhino.Geometry.Collections;
using Rhino.Collections;

namespace LilyPad.Objects.NthOrder
{
    class VectorMesh
    {

        //Propeties___________________________________________________________________________________________
        public Rhino.Geometry.Mesh Mesh;
        public Polyline[] NakedEdges;
        public Plane MeshPlane;
        public List<Vector3d> Principals;
        public List<Point3d> Centres;
        public double Radius;




        //Constructors___________________________________________________________________________________________
        public VectorMesh()
        {
            Mesh = new Rhino.Geometry.Mesh();
            Principals = new List<Vector3d>();
            Centres = new List<Point3d>();
            MeshPlane = new Plane();
        }

        public VectorMesh(Rhino.Geometry.Mesh mesh, List<Vector3d> principals, double radius)
        {

            //copy principal vectors to prevent overwriting original list
            List<Vector3d> copyPrincipals = new List<Vector3d>();
            for (int i = 0; i < principals.Count; i++)
            {
                copyPrincipals.Add(principals[i]);
            }

            //assign properties
            Mesh = mesh;
            Principals = copyPrincipals;

            List<Point3d> centres = new List<Point3d>();
            for (int i = 0; i < Mesh.Faces.Count; i++)
            {
                Point3d centre = Mesh.Faces.GetFaceCenter(i);
                centres.Add(centre);
            }
            Centres = centres;

            Mesh.FaceNormals.ComputeFaceNormals();

            NakedEdges = Mesh.GetNakedEdges();
            Radius = radius;
            MeshPlane = new Plane(Mesh.Vertices[0], Mesh.Vertices[1], Mesh.Vertices[2]);
        }

        public VectorMesh(VectorMesh vectorMesh)
        {
            //copy principal vectors to prevent overwriting original list
            List<Vector3d> copyPrincipals = new List<Vector3d>();
            for (int i = 0; i < vectorMesh.Principals.Count; i++)
            {
                copyPrincipals.Add(vectorMesh.Principals[i]);
            }

            Mesh = vectorMesh.Mesh;
            Principals = copyPrincipals;
            Centres = vectorMesh.Centres;
            NakedEdges = vectorMesh.NakedEdges;
            Radius = vectorMesh.Radius;
            MeshPlane = vectorMesh.MeshPlane;
        }

        //Methods_____________________________________________________________________________________________________

        /// <summary>
        /// Evaluates the vector a specfic direction using the Nth Order method proposed by Tam (2015)
        /// </summary>
        public bool Evaluate(Point3d location, ref Vector3d direction)
        {
            Sphere sphere = new Sphere(location, Radius);
            double dist;
            double angle;

            //Check if points are contain with a box centred on location with edge lengths equal to 2x radius
            BoundingBox box = sphere.BoundingBox;
            List<int> contains = new List<int>();
            for (int i = 0; i < Centres.Count; i++)
            {
                if (box.Contains(Centres[i])) contains.Add(i);
            }

            ////find closest vector
            //double closestDist = Radius;
            //Vector3d closestVec = new Vector3d();
            //for (int i = 0; i < contains.Count; i++)
            //{
            //    dist = location.DistanceTo(Centres[contains[i]]);
            //    if (dist < closestDist)
            //    {
            //        closestDist = dist;
            //        closestVec = Principals[contains[i]];
            //    }
            //}
            Vector3d cumulVector = new Vector3d();
            double cumulWeight = 0.0;
            for (int i = 0; i < contains.Count; i++)
            {
                int j = contains[i];
                dist = location.DistanceTo(Centres[j]);
                if (dist <= Radius)
                {
                    cumulVector += -Principals[j];
                    cumulWeight += (Radius - dist);
                }
            }
            Vector3d closestVec = cumulVector;

            cumulVector = new Vector3d();
            cumulWeight = 0.0;
            for (int i = 0; i < contains.Count; i++)
            {
                int j = contains[i];
                dist = location.DistanceTo(Centres[j]);
                if (dist <= Radius)
                {
                    angle = Math.Abs(Vector3d.VectorAngle(closestVec, Principals[j]));
                    if (angle > Math.PI * 0.75) cumulVector += -Principals[j] * (Radius - dist);
                    else cumulVector += Principals[j] * (Radius - dist);

                    cumulWeight += (Radius - dist);
                }
            }

            if (cumulWeight == 0.0)
            {
                //if no points are found use the values from the closest centre
                Point3dList centres = new Point3dList(Centres);
                direction = Principals[centres.ClosestIndex(location)];
                direction = direction / direction.Length;
                return false;
            }
            else
            {
                direction = cumulVector / cumulWeight;
                direction = direction / direction.Length;
                return true;
            }
        }

        public void VectorCorrection(List<int> priority)
        {
            int current;
            List<int> checkedFaces = new List<int>();
            MeshFaceList faces = Mesh.Faces;
            int[] adjacentFaces;
            Vector3d meanVec = new Vector3d();
            double angle;

            //insert the initial face into the checkedFaces list
            checkedFaces.Add(priority[0]);

            for (int i = 1; i < priority.Count; i++)
            {
                //set current face index
                current = priority[i];

                //find adjacent faces in the whole mesh
                adjacentFaces = faces.AdjacentFaces(current);

                //find adjacent faces which have been checked
                List<int> checkedAdjaFaces = new List<int>();
                foreach (var adjaFace in adjacentFaces) if (checkedFaces.Contains(adjaFace)) checkedAdjaFaces.Add(adjaFace);

                //Calculate the average vector of the principal vectors from faces which have been checked and are adjacent
                int count = 0;
                foreach (var adjaFace in checkedAdjaFaces)
                {
                    count++;
                    meanVec += Principals[adjaFace];
                }
                meanVec = meanVec / count;

                //Measure the angle between the current face and the mean vector
                Vector3d currentVec = Principals[current];
                angle = Math.Abs(Vector3d.VectorAngle(currentVec, meanVec));

                //If the angle between the two vectors is greater than 0.75 times 180 degrees then flip the principal vector of the current face
                if (angle > Math.PI * 0.75)
                {
                    currentVec = -currentVec;
                    Principals.Insert(current, currentVec);
                    Principals.RemoveAt(current + 1);
                }

                //add face to checked faces
                checkedFaces.Add(current);
            }

        }

        public void ChangeRadius(double radius)
        {
            Radius = radius;
        }
    }

}
