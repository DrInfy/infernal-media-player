using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class VocapiaSplit : SubtitleFormat
    {
        public override string Extension
        {
            get { return ".xml"; }
        }

        public override string Name
        {
            get { return "Vocapia Split"; }
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

        private static string ToTimeCode(double totalMilliseconds)
        {
            return string.Format("{0:0##}", totalMilliseconds / TimeCode.BaseUnit);
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            _errorCount = 0;

            var sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));

            string xmlString = sb.ToString();
            if (!xmlString.Contains("<SpeechSegment"))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("SegmentList/SpeechSegment"))
            {
                try
                {
                    string start = node.Attributes["stime"].InnerText;
                    string end = node.Attributes["etime"].InnerText;
                    string text = node.InnerText;
                    text = text.Replace("<s/>", Environment.NewLine);
                    text = text.Replace("  ", " ");
                    var p = new Paragraph(text, ParseTimeCode(start), ParseTimeCode(end));
                    var spkIdAttr = node.Attributes["spkid"];
                    if (spkIdAttr != null)
                    {
                        p.Extra = spkIdAttr.InnerText;
                        p.Actor = p.Extra;
                    }
                    subtitle.Paragraphs.Add(p);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    _errorCount++;
                }
            }
            subtitle.Renumber();
            if (subtitle.Paragraphs.Count > 0)
                subtitle.Header = xmlString;
        }

        private static double ParseTimeCode(string s)
        {
            return Convert.ToDouble(s) * TimeCode.BaseUnit;
        }

        public override bool HasStyleSupport
        {
            get { return true; }
        }

        public static List<string> GetStylesFromHeader(Subtitle subtitle)
        {
            var list = new List<string>();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (!string.IsNullOrEmpty(p.Actor))
                {
                    if (list.IndexOf(p.Actor) < 0)
                        list.Add(p.Actor);
                }
            }
            return list;
        }

    }
}
