using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nikse.SubtitleEdit.Core;

namespace Base.Subtitles
{
    public class EnhancedParagraph
    {
        public List<SubtitleTag> Tags = null;
        public Paragraph Paragraph { get; private set; }

        public string Text { get; set; }
        public SubtitleHeader Header { get; set; }
        public EnhancedParagraph(SubtitleHeader header, Paragraph p)
        {
            this.Paragraph = p;
            Header = header;
            if (header.IsAss)
            {
                SubtitleFormatReader.GetAssTags(p.Text, this);
            }
        }
    }
}
