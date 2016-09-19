using System;
using System.Linq;

namespace Imp.Base.Subtitles
{
    public class SubtitleTag
    {
        public ParenthesisType Type { get; set; }
        public string Tag { get; set; }
        public string AdditionalContent{ get; set; }
        public int StartIndex { get; set; }
        public int? EndIndex { get; set; }

        public SubtitleTag(string content, ParenthesisType type, int startIndex, int? endIndex)
        {
            this.Type = type;
            StartIndex = startIndex;
            EndIndex = endIndex;

            if (type == ParenthesisType.Chevrons)
            {
                var splits = content.Split(new[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries);
                Tag = splits.FirstOrDefault();
                AdditionalContent = splits.Skip(1).FirstOrDefault();
            }
            else
            {
                Tag = content;
            }
        }
    }
}
