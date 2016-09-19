#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Imp.Base
{
    [Serializable]
    public class Settings
    {
        #region Fields

        public List<string> CustomPaths = new List<string>();
        public double Volume = 0.75;
        public FileTypes LastFileTypes = FileTypes.Music | FileTypes.Videos;
        public LoopMode LastLoopMode = LoopMode.NoLoop;

        #endregion
    }
}