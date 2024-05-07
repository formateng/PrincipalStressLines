using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace Streamlines.NthOrder
{
    class IGH_Goo_PrincipalMesh : GH_Goo<VectorMesh>
    {

        //Constructors________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________

        // Default Constructor, sets the state to Unknown.
        public IGH_Goo_PrincipalMesh()
        {
            this.Value = new VectorMesh();
        }

        // Constructor with initial value
        public IGH_Goo_PrincipalMesh(VectorMesh principalMesh)
        {
            this.Value = new VectorMesh(principalMesh);
        }

        // Copy Constructor
        public IGH_Goo_PrincipalMesh(IGH_Goo_PrincipalMesh gooPrincipalMesh)
        {
            this.Value = gooPrincipalMesh.Value;
        }

        // Duplication method (technically not a constructor)
        public override IGH_Goo Duplicate()
        {
            return new IGH_Goo_PrincipalMesh(this);
        }

        //Formatters_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________
        // Instances are always valid
        public override bool IsValid
        {
            get { return true; }
        }

        // Return a string with the name of this Type.
        public override string TypeName
        {
            get { return "Principal Mesh"; }
        }

        // Return a string describing what this Type is about.
        public override string TypeDescription
        {
            get { return "A principal mesh represents a rhino mesh with attached vectors to each face, representing the principal stress direction in that face"; }
        }

        // Return a string representation of the state (value) of this instance.
        public override string ToString()
        {
            return "Principal Mesh";
        }

        //Casting_______________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________
         //Return the principalmesh class to use in script.
        public override object ScriptVariable()
        {
            return this.Value;
        }

        /*This function is called when Grasshopper needs to convert this 
        // instance of TriStateType into some other type Q.
        public override bool CastTo<Q>(ref Q target)
        {
            //First, see if Q is similar to the Integer primitive.
            if (typeof(Q).IsAssignableFrom(typeof(int)))
            {
                object ptr = this.Value;
                target = (Q)ptr;
                return true;
            }

            //Then, see if Q is similar to the GH_Integer type.
            if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
            {
                object ptr = new GH_Integer(this.Value);
                target = (Q)ptr;
                return true;
            }

            //We could choose to also handle casts to Boolean, GH_Boolean, 
            //Double and GH_Number, but this is left as an exercise for the reader.
            return false;
        }

        // This function is called when Grasshopper needs to convert other data 
        // into TriStateType.
        public override bool CastFrom(object source)
        {
            //Abort immediately on bogus data.
            if (source == null) { return false; }

            //Use the Grasshopper Integer converter. By specifying GH_Conversion.Both 
            //we will get both exact and fuzzy results. You should always try to use the
            //methods available through GH_Convert as they are extensive and consistent.
            int val;
            if (GH_Convert.ToInt32(source, out val, GH_Conversion.Both))
            {
                this.Value = val;
                return true;
            }

            //If the integer conversion failed, we can still try to parse Strings.
            //If possible, you should ensure that your data type can 'deserialize' itself 
            //from the output of the ToString() method.
            string str = null;
            if (GH_Convert.ToString(source, out str, GH_Conversion.Both))
            {
                switch (str.ToUpperInvariant())
                {
                    case "FALSE":
                    case "F":
                    case "NO":
                    case "N":
                        this.Value = 0;
                        return true;

                    case "TRUE":
                    case "T":
                    case "YES":
                    case "Y":
                        this.Value = +1;
                        return true;

                    case "UNKNOWN":
                    case "UNSET":
                    case "MAYBE":
                    case "DUNNO":
                    case "?":
                        this.Value = -1;
                        return true;
                }
            }

            //We've exhausted the possible conversions, it seems that source
            //cannot be converted into a TriStateType after all.
            return false;
        }
        */
    }
}
