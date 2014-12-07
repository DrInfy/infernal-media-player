using System;
using System.Collections.Generic;
using Base;

namespace InfernalWorkOutTracker.Controllers
{
    [Serializable]
    public class WotSettings
    {
        public List<string> CustomPaths = new List<string>();
        public FileTypes LastFileTypes = FileTypes.Music | FileTypes.Videos;
        public LoopMode LastLoopMode = LoopMode.NoLoop;
    }
}