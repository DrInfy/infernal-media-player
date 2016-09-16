using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Base.Subtitles
{
    public class SubtitleHeader
    {
        public bool IsAss { get; set; }
        public Size VideoSize { get; set; }
        public double? FramesPerSecond{ get; set; }
        public Dictionary<string, SubtitleStyle> SubtitleStyles;
    }
}
