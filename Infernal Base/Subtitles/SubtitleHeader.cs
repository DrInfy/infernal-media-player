﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Nikse.SubtitleEdit.Core;

namespace Base.Subtitles
{
    public class SubtitleHeader
    {
        public bool IsAss { get; set; }
        public bool UseStyles { get; set; }
        public int? PlayResY { get; set; }
        public int? PlayResX { get; set; }
        public int? AspectRatio { get; set; }
        public double? FramesPerSecond{ get; set; }
        public Dictionary<string, SsaStyle> SubtitleStyles = new Dictionary<string, SsaStyle>();
    }
}
