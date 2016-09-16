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

        public EnhancedParagraph(Paragraph p)
        {
            this.Paragraph = p;
            SubtitleFormatReader.GetAssTags(p.Text, this);
        }
    }
}
