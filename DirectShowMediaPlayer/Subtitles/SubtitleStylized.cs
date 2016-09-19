using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imp.DirectShow.Subtitles
{
    public class SubtitleStylized
    {
        public SubtitleHeader Header { get; set; }
        public List<EnhancedParagraph> Paragraphs { get; set; }
    }
}
