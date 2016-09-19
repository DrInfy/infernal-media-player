using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace Imp.DirectShow.Subtitles
{
    public class SubtitleTrack
    {
        public string Name { get; set; }
        public string Language { get; set; }
        public SubtitleStylized Subtitles { get; set; }
        internal SubtitleFormat Format { get; set; }
        internal Subtitle RawSubs { get; set; }

        internal bool Loaded { get; set; }
        //public string Name { get; set; }
    }
}
