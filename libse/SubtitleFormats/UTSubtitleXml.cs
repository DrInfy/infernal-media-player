using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class UTSubtitleXml : SubtitleFormat
    {
        public override string Extension
        {
            get { return ".xml"; }
        }

        public override string Name
        {
            get { return "UT Subtitle xml"; }
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

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            _errorCount = 0;

            var sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            if (!sb.ToString().Contains("<uts") || !sb.ToString().Contains("secOut="))
                return;
            var xml = new XmlDocument { XmlResolver = null };
            try
            {
                string xmlText = sb.ToString();
                xml.LoadXml(xmlText);

                foreach (XmlNode node in xml.SelectNodes("//ut"))
                {
                    try
                    {
                        string endTime = node.Attributes["secOut"].InnerText;
                        string startTime = node.Attributes["secIn"].InnerText;
                        string text = node.InnerText;
                        text = text.Replace("<br>", Environment.NewLine).Replace("<br />", Environment.NewLine);
                        var p = new Paragraph();
                        p.StartTime.TotalSeconds = Convert.ToDouble(startTime, System.Globalization.CultureInfo.InvariantCulture);
                        p.EndTime.TotalSeconds = Convert.ToDouble(endTime, System.Globalization.CultureInfo.InvariantCulture);
                        p.Text = text;
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _errorCount = 1;
                return;
            }
        }

    }
}
