using System;
using System.Collections.Generic;

namespace Base
{
    [Serializable]
    public class Settings
    {
        public List<string> CustomPaths = new List<string>();
        public double Volume = 0.75;
        public FileTypes LastFileTypes = FileTypes.Music | FileTypes.Videos; 
        public LoopMode LastLoopMode = LoopMode.NoLoop;
    }
}
