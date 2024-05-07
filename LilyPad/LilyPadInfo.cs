using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace LilyPad
{
    public class LilyPadInfo : GH_AssemblyInfo
    {
        public override string Name => "LilyPad";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("b03fd707-57b0-4a02-8d9b-1cd305b79e07");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}