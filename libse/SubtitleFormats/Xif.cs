using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class Xif : SubtitleFormat
    {
        public override string Extension
        {
            get { return ".xif"; }
        }

        public override string Name
        {
            get { return "XIF"; }
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
            var sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            var xmlAsText = sb.ToString().Trim();
            if (!xmlAsText.Contains("<XIF") || !xmlAsText.Contains("<SubtitleText"))
            {
                return;
            }

            try
            {
                var xml = new XmlDocument { XmlResolver = null };
                xml.LoadXml(xmlAsText);
                foreach (XmlNode node in xml.DocumentElement.SelectNodes("FileBody/ContentBlock"))
                {
                    try
                    {
                        var timeCodeIn = DecodeTimeCodeFrames(node.SelectSingleNode("ThreadedObject/TimingObject/TimeIn").Attributes["value"].InnerText, SplitCharColon);
                        var timeCodeOut = DecodeTimeCodeFrames(node.SelectSingleNode("ThreadedObject/TimingObject/TimeOut").Attributes["value"].InnerText, SplitCharColon);
                        sb.Clear();
                        foreach (XmlNode paragraphNode in node.SelectSingleNode("ThreadedObject/Content/SubtitleText/Paragraph").ChildNodes)
                        {
                            if (paragraphNode.Name == "Text")
                                sb.Append(" " + paragraphNode.InnerText);
                            else if (paragraphNode.Name == "HardReturn")
                                sb.AppendLine();
                        }
                        var p = new Paragraph(timeCodeIn, timeCodeOut, sb.ToString().Replace("  ", " ").Trim());
                        subtitle.Paragraphs.Add(p);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        _errorCount++;
                    }
                }
                subtitle.Renumber();
            }
            catch (Exception)
            {
                _errorCount++;
            }
        }

    }
}
