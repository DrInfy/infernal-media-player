using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.ListLogic;

namespace ImpControls.SpecialFolder
{
    public class SpecialFolderContent
    {
        public readonly string PathData;
        public readonly List<string> FilePaths = new List<string>();
        public readonly List<string> FolderPaths = new List<string>();


        public SpecialFolderContent(string pathData)
        {
            PathData = pathData;
        }
    }
}
