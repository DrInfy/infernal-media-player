using System.Collections.Generic;
using Nikse.SubtitleEdit.Core;

namespace Imp.DirectShow.Subtitles
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
            this.Header = header;
            if (header.IsAss)
            {
                SubtitleFormatReader.GetAssTags(p.Text, this);
            }
            else
            {
                Text = p.Text;
            }
        }
    }
}
