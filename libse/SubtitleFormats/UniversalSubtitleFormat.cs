using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class UniversalSubtitleFormat : SubtitleFormat
    {
        public override string Extension
        {
            get { return ".usf"; }
        }

        public override string Name
        {
            get { return "Universal Subtitle Format"; }
        }

        public override bool IsTimeBased
        {
            get { return true; }
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            var subtitle = new Subtitle();
            LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > 0;
        }

        private static TimeCode DecodeTimeCode(string code)
        {
            string[] parts = code.Split(new[] { ':', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return new TimeCode(0, 0, int.Parse(code), 0); // seconds only
            }
            if (parts.Length == 2)
            {
                return new TimeCode(0, 0, int.Parse(parts[0]), int.Parse(parts[1])); // seconds + ms
            }

            //00:00:07:120
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string ms = parts[3];
            return new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), int.Parse(ms));
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            _errorCount = 0;

            var sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));

            string xmlString = sb.ToString();
            if (!xmlString.Contains("<USFSubtitles") || !xmlString.Contains("<subtitles>"))
                return;

            var xml = new XmlDocument { XmlResolver = null };
            try
            {
                xml.LoadXml(xmlString);
            }
            catch
            {
                _errorCount = 1;
                return;
            }

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("subtitles/subtitle"))
            {
                try
                {
                    string start = node.Attributes["start"].InnerText;
                    string stop = node.Attributes["stop"].InnerText;

                    var text = new StringBuilder();
                    foreach (XmlNode innerNode in node.SelectSingleNode("text").ChildNodes)
                    {
                        switch (innerNode.Name.Replace("tt:", string.Empty))
                        {
                            case "br":
                                text.AppendLine();
                                break;
                            default:
                                text.Append(innerNode.InnerText);
                                break;
                        }
                    }

                    subtitle.Paragraphs.Add(new Paragraph(DecodeTimeCode(start), DecodeTimeCode(stop), text.ToString().Trim()));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    _errorCount++;
                }
            }
            subtitle.Renumber();
        }

    }
}
