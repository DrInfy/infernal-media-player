using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class Tmx14 : SubtitleFormat
    {
        public override string Extension
        {
            get { return ".tmx"; }
        }

        public override string Name
        {
            get { return "Translation Memory xml"; }
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
            if (!xmlString.Contains("<tmx") || !xmlString.Contains("<seg>"))
                return;

            var xml = new XmlDocument();
            try
            {
                xml.XmlResolver = null; // skip any DTD
                xml.LoadXml(xmlString);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                _errorCount = 1;
                return;
            }

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("//tu"))
            {
                try
                {
                    XmlNode start = node.SelectSingleNode("prop[@type='start']");
                    XmlNode end = node.SelectSingleNode("prop[@type='end']");
                    XmlNode seg = node.SelectSingleNode("tuv/seg");

                    if (seg != null)
                    {
                        string text = seg.InnerText.Replace("<br />", Environment.NewLine);
                        text = text.Replace("<br/>", Environment.NewLine);
                        text = text.Replace("<br>", Environment.NewLine);
                        text = text.Replace("<BR />", Environment.NewLine);
                        text = text.Replace("<BR/>", Environment.NewLine);
                        text = text.Replace("<BR>", Environment.NewLine);
                        subtitle.Paragraphs.Add(new Paragraph(DecodeTimeCode(start), DecodeTimeCode(end), text));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    _errorCount++;
                }
            }
            subtitle.Renumber();
        }

        private static TimeCode DecodeTimeCode(XmlNode node)
        {
            var tc = new TimeCode(0, 0, 0, 0);
            if (node != null)
            {
                string[] arr = node.InnerText.Split(new[] { ':', '.', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                tc = new TimeCode(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), int.Parse(arr[3]));
            }
            return tc;
        }

    }
}
