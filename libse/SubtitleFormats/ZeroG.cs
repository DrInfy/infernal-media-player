using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class ZeroG : SubtitleFormat
    {
        //E 1 0:50:05.42 0:50:10.06 Default NTP
        private static readonly Regex RegexTimeCodes = new Regex(@"^E 1 \d:\d\d:\d\d.\d\d \d:\d\d:\d\d.\d\d Default NTP ", RegexOptions.Compiled);

        public override string Extension
        {
            get { return ".zeg"; }
        }

        public override string Name
        {
            get { return "Zero G"; }
        }

        public override bool IsTimeBased
        {
            get { return true; }
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            var subtitle = new Subtitle();
            LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > _errorCount;
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            _errorCount = 0;
            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                var s = line.Trim();
                if (s.Length > 35 && RegexTimeCodes.IsMatch(s))
                {
                    try
                    {
                        string timePart = s.Substring(4, 10).TrimEnd();
                        var start = DecodeTimeCode(timePart);
                        timePart = s.Substring(15, 10).Trim();
                        var end = DecodeTimeCode(timePart);
                        var paragraph = new Paragraph { StartTime = start, EndTime = end };
                        paragraph.Text = s.Substring(38).Replace(" \\n ", Environment.NewLine).Replace("\\n", Environment.NewLine);
                        subtitle.Paragraphs.Add(paragraph);
                    }
                    catch
                    {
                        _errorCount++;
                    }
                }
            }
            subtitle.Renumber();
        }

        private static string EncodeTimeCode(TimeCode time)
        {
            //0:50:05.42
            return string.Format("{0:0}:{1:00}:{2:00}.{3:00}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds / 10);
        }

        private static TimeCode DecodeTimeCode(string timePart)
        {
            string[] parts = timePart.Split(new[] { ':', '.' }, StringSplitOptions.RemoveEmptyEntries);
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            int milliseconds = int.Parse(parts[3]) * 10;
            return new TimeCode(hours, minutes, seconds, milliseconds);
        }

    }
}
