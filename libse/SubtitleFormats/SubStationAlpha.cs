using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class SubStationAlpha : SubtitleFormat
    {
        public string Errors { get; private set; }

        public override string Extension
        {
            get { return ".ssa"; }
        }

        public const string NameOfFormat = "Sub Station Alpha";

        public override string Name
        {
            get { return NameOfFormat; }
        }

        public override bool IsTimeBased
        {
            get { return true; }
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            var subtitle = new Subtitle();
            LoadSubtitle(subtitle, lines, fileName);
            Errors = null;
            return subtitle.Paragraphs.Count > _errorCount;
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            _errorCount = 0;
            Errors = null;
            bool eventsStarted = false;
            subtitle.Paragraphs.Clear();
            // "Marked", " Start", " End", " Style", " Name", " MarginL", " MarginR", " MarginV", " Effect", " Text"
            int indexLayer = 0;
            int indexStart = 1;
            int indexEnd = 2;
            int indexStyle = 3;
            const int indexName = 4;
            int indexMarginL = 5;
            int indexMarginR = 6;
            int indexMarginV = 7;
            int indexEffect = 8;
            int indexText = 9;
            var errors = new StringBuilder();
            int lineNumber = 0;

            var header = new StringBuilder();
            foreach (string line in lines)
            {
                lineNumber++;
                if (!eventsStarted)
                    header.AppendLine(line);

                if (line.Trim().Equals("[events]", StringComparison.OrdinalIgnoreCase))
                {
                    eventsStarted = true;
                }
                else if (!string.IsNullOrEmpty(line) && line.TrimStart().StartsWith(';'))
                {
                    // skip comment lines
                }
                else if (eventsStarted && !string.IsNullOrWhiteSpace(line))
                {
                    string s = line.Trim().ToLower();
                    if (line.Length > 10 && s.StartsWith("format:", StringComparison.Ordinal))
                    {
                        var format = s.Substring(8).Split(',');
                        for (int i = 0; i < format.Length; i++)
                        {
                            var formatTrimmed = format[i].Trim();
                            if (formatTrimmed.Equals("layer", StringComparison.Ordinal))
                                indexLayer = i;
                            else if (formatTrimmed.Equals("start", StringComparison.Ordinal))
                                indexStart = i;
                            else if (formatTrimmed.Equals("end", StringComparison.Ordinal))
                                indexEnd = i;
                            else if (formatTrimmed.Equals("text", StringComparison.Ordinal))
                                indexText = i;
                            else if (formatTrimmed.Equals("effect", StringComparison.Ordinal))
                                indexEffect = i;
                            else if (formatTrimmed.Equals("style", StringComparison.Ordinal))
                                indexStyle = i;
                            else if (formatTrimmed.Equals("marginl", StringComparison.Ordinal))
                                indexMarginL = i;
                            else if (formatTrimmed.Equals("marginr", StringComparison.Ordinal))
                                indexMarginR = i;
                            else if (formatTrimmed.Equals("marginv", StringComparison.Ordinal))
                                indexMarginV = i;
                        }
                    }
                    else if (!string.IsNullOrEmpty(s))
                    {
                        var text = string.Empty;
                        var start = string.Empty;
                        var end = string.Empty;
                        var style = string.Empty;
                        var marginL = string.Empty;
                        var marginR = string.Empty;
                        var marginV = string.Empty;
                        var layer = 0;
                        var effect = string.Empty;
                        var name = string.Empty;

                        string[] splittedLine;
                        if (s.StartsWith("dialog:", StringComparison.Ordinal))
                            splittedLine = line.Remove(0, 7).Split(',');
                        else if (s.StartsWith("dialogue:", StringComparison.Ordinal))
                            splittedLine = line.Remove(0, 9).Split(',');
                        else
                            splittedLine = line.Split(',');

                        for (int i = 0; i < splittedLine.Length; i++)
                        {
                            if (i == indexStart)
                                start = splittedLine[i].Trim();
                            else if (i == indexEnd)
                                end = splittedLine[i].Trim();
                            else if (i == indexLayer)
                                int.TryParse(splittedLine[i], out layer);
                            else if (i == indexEffect)
                                effect = splittedLine[i];
                            else if (i == indexText)
                                text = splittedLine[i];
                            else if (i == indexStyle)
                                style = splittedLine[i];
                            else if (i == indexMarginL)
                                marginL = splittedLine[i].Trim();
                            else if (i == indexMarginR)
                                marginR = splittedLine[i].Trim();
                            else if (i == indexMarginV)
                                marginV = splittedLine[i].Trim();
                            else if (i == indexName)
                                name = splittedLine[i];
                            else if (i > indexText)
                                text += "," + splittedLine[i];
                        }

                        try
                        {
                            var p = new Paragraph
                            {
                                StartTime = GetTimeCodeFromString(start),
                                EndTime = GetTimeCodeFromString(end),
                                Text = AdvancedSubStationAlpha.GetFormattedText(text)
                            };

                            if (!string.IsNullOrEmpty(style))
                                p.Extra = style;
                            if (!string.IsNullOrEmpty(marginL))
                                p.MarginL = marginL;
                            if (!string.IsNullOrEmpty(marginR))
                                p.MarginR = marginR;
                            if (!string.IsNullOrEmpty(marginV))
                                p.MarginV = marginV;
                            if (!string.IsNullOrEmpty(effect))
                                p.Effect = effect;
                            p.Layer = layer;
                            if (!string.IsNullOrEmpty(name))
                                p.Actor = name;
                            p.IsComment = s.StartsWith("comment:", StringComparison.Ordinal);
                            subtitle.Paragraphs.Add(p);
                        }
                        catch
                        {
                            _errorCount++;
                            if (errors.Length < 2000)
                                errors.AppendLine(string.Format(Configuration.Settings.Language.Main.LineNumberXErrorReadingTimeCodeFromSourceLineY, lineNumber, line));
                        }
                    }
                }
            }
            if (header.Length > 0)
                subtitle.Header = header.ToString();
            subtitle.Renumber();
            Errors = errors.ToString();
        }

        private static TimeCode GetTimeCodeFromString(string time)
        {
            // h:mm:ss.cc
            string[] timeCode = time.Split(':', '.');
            return new TimeCode(int.Parse(timeCode[0]),
                                int.Parse(timeCode[1]),
                                int.Parse(timeCode[2]),
                                int.Parse(timeCode[3]) * 10);
        }

        public override void RemoveNativeFormatting(Subtitle subtitle, SubtitleFormat newFormat)
        {
            if (newFormat != null && newFormat.Name == AdvancedSubStationAlpha.NameOfFormat)
            {
                // do we need any conversion?
            }
            else
            {
                foreach (Paragraph p in subtitle.Paragraphs)
                {
                    int indexOfBegin = p.Text.IndexOf('{');
                    string pre = string.Empty;
                    while (indexOfBegin >= 0 && p.Text.IndexOf('}') > indexOfBegin)
                    {
                        string s = p.Text.Substring(indexOfBegin);
                        if (s.StartsWith("{\\an1}", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an2}", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an3}", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an4}", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an5}", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an6}", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an7}", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an8}", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an9}", StringComparison.Ordinal))
                        {
                            pre = s.Substring(0, 6);
                        }
                        else if (s.StartsWith("{\\an1\\", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an2\\", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an3\\", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an4\\", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an5\\", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an6\\", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an7\\", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an8\\", StringComparison.Ordinal) ||
                            s.StartsWith("{\\an9\\", StringComparison.Ordinal))
                        {
                            pre = s.Substring(0, 5) + "}";
                        }
                        else if (s.StartsWith("{\\a1}", StringComparison.Ordinal) || s.StartsWith("{\\a1\\", StringComparison.Ordinal) ||
                                 s.StartsWith("{\\a3}", StringComparison.Ordinal) || s.StartsWith("{\\a3\\", StringComparison.Ordinal))
                        {
                            pre = s.Substring(0, 4) + "}";
                        }
                        else if (s.StartsWith("{\\a9}", StringComparison.Ordinal) || s.StartsWith("{\\a9\\", StringComparison.Ordinal))
                        {
                            pre = "{\\an4}";
                        }
                        else if (s.StartsWith("{\\a10}", StringComparison.Ordinal) || s.StartsWith("{\\a10\\", StringComparison.Ordinal))
                        {
                            pre = "{\\an5}";
                        }
                        else if (s.StartsWith("{\\a11}", StringComparison.Ordinal) || s.StartsWith("{\\a11\\", StringComparison.Ordinal))
                        {
                            pre = "{\\an6}";
                        }
                        else if (s.StartsWith("{\\a5}", StringComparison.Ordinal) || s.StartsWith("{\\a5\\", StringComparison.Ordinal))
                        {
                            pre = "{\\an7}";
                        }
                        else if (s.StartsWith("{\\a6}", StringComparison.Ordinal) || s.StartsWith("{\\a6\\", StringComparison.Ordinal))
                        {
                            pre = "{\\an8}";
                        }
                        else if (s.StartsWith("{\\a7}", StringComparison.Ordinal) || s.StartsWith("{\\a7\\", StringComparison.Ordinal))
                        {
                            pre = "{\\an9}";
                        }
                        int indexOfEnd = p.Text.IndexOf('}');
                        p.Text = p.Text.Remove(indexOfBegin, (indexOfEnd - indexOfBegin) + 1);

                        indexOfBegin = p.Text.IndexOf('{');
                    }
                    p.Text = pre + p.Text;
                }
            }
        }

        public override bool HasStyleSupport
        {
            get
            {
                return true;
            }
        }

    }
}
