using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
