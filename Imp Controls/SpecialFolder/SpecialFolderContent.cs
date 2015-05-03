#region Usings

using System.Collections.Generic;

#endregion

namespace ImpControls.SpecialFolder
{
    public class SpecialFolderContent
    {
        #region Fields

        public readonly string PathData;
        public readonly List<string> FilePaths = new List<string>();
        public readonly List<string> FolderPaths = new List<string>();

        #endregion

        public SpecialFolderContent(string pathData)
        {
            PathData = pathData;
        }
    }
}