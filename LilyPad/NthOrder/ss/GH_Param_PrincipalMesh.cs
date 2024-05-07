using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;

namespace Streamlines.NthOrder
{



    class GH_Param_PrincipalMesh : GH_Param<IGH_Goo_PrincipalMesh>
    {

        /* We need to supply a constructor without arguments that calls the base class constructor.
        public GH_Param_PrincipalMesh()
          : base("PrincipalMesh", "PrinciMesh", "Represents a rhino mesh with attached vectors to each face, representing the principal stress direction in that face", "Params", "Primitive",GH_ParamAccess.item)
        { }
        */

        public GH_Param_PrincipalMesh() : 
            base(new GH_InstanceDescription("PrincipalMesh", "PrinciMesh", "Represents a rhino mesh with attached vectors to each face, representing the principal stress direction in that face", "Params", "Primitive"))
        { }

        public override System.Guid ComponentGuid
        {
            // Always generate a new Guid, but never change it once 
            // you've released this parameter to the public.
            get { return new Guid("{2FEEF1A2-A754-431d-8C78-9BF92B78BAE1}"); }
        }

    }

}
