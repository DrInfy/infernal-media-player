using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class TimeXml : SubtitleFormat
    {
        public override string Extension
        {
            get { return ".xml"; }
        }

        public override string Name
        {
            get { return "Xml"; }
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

            string xmlString = sb.ToString();
            if (!xmlString.Contains("<Paragraph>") || !xmlString.Contains("<Text>"))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("Paragraph"))
            {
                try
                {
                    string start = node.SelectSingleNode("StartMilliseconds").InnerText;
                    string end = node.SelectSingleNode("EndMilliseconds").InnerText;
                    string text = node.SelectSingleNode("Text").InnerText;

                    subtitle.Paragraphs.Add(new Paragraph(text, Convert.ToDouble(start, CultureInfo.InvariantCulture), Convert.ToDouble(end, CultureInfo.InvariantCulture)));
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
