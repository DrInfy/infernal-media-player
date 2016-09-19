using Nikse.SubtitleEdit.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class YouTubeAnnotations : SubtitleFormat
    {
        public interface IGetYouTubeAnnotationStyles
        {
            List<string> GetYouTubeAnnotationStyles(Dictionary<string, int> stylesWithCount);
        }

        public static IGetYouTubeAnnotationStyles GetYouTubeAnnotationStyles { get; set; }

        private bool _promtForStyles = true;

        public override string Extension
        {
            get { return ".xml"; }
        }

        public const string NameOfFormat = "YouTube Annotations";

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
            _promtForStyles = false;
            LoadSubtitle(subtitle, lines, fileName);
            _promtForStyles = true;
            return subtitle.Paragraphs.Count > 0;
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            _errorCount = 0;

            var sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            if (!sb.ToString().Contains("</annotations>") || !sb.ToString().Contains("</TEXT>"))
                return;
            var xml = new XmlDocument { XmlResolver = null };
            try
            {
                string xmlText = sb.ToString();
                xml.LoadXml(xmlText);
                var styles = new List<string> { "speech" };

                if (_promtForStyles)
                {
                    var stylesWithCount = new Dictionary<string, int>();
                    foreach (XmlNode node in xml.SelectNodes("//annotation"))
                    {
                        try
                        {
                            if (node.Attributes["style"] != null && node.Attributes["style"].Value != null)
                            {
                                string style = node.Attributes["style"].Value;

                                XmlNode textNode = node.SelectSingleNode("TEXT");
                                XmlNodeList regions = node.SelectNodes("segment/movingRegion/anchoredRegion");

                                if (regions.Count != 2)
                                    regions = node.SelectNodes("segment/movingRegion/rectRegion");

                                if (textNode != null && regions.Count == 2)
                                {
                                    if (stylesWithCount.ContainsKey(style))
                                        stylesWithCount[style]++;
                                    else
                                        stylesWithCount.Add(style, 1);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                    if (stylesWithCount.Count > 1 && GetYouTubeAnnotationStyles != null)
                    {
                        styles = GetYouTubeAnnotationStyles.GetYouTubeAnnotationStyles(stylesWithCount);
                    }
                    else
                    {
                        styles.Clear();
                        foreach (var k in stylesWithCount.Keys)
                            styles.Add(k);
                    }
                }
                else
                {
                    styles.Add("popup");
                    styles.Add("anchored");
                }

                foreach (XmlNode node in xml.SelectNodes("//annotation"))
                {
                    try
                    {
                        if (node.Attributes["style"] != null && styles.Contains(node.Attributes["style"].Value))
                        {
                            XmlNode textNode = node.SelectSingleNode("TEXT");
                            XmlNodeList regions = node.SelectNodes("segment/movingRegion/anchoredRegion");

                            if (regions.Count != 2)
                                regions = node.SelectNodes("segment/movingRegion/rectRegion");

                            if (textNode != null && regions.Count == 2)
                            {
                                string startTime = regions[0].Attributes["t"].Value;
                                string endTime = regions[1].Attributes["t"].Value;
                                var p = new Paragraph();
                                p.StartTime = DecodeTimeCode(startTime);
                                p.EndTime = DecodeTimeCode(endTime);
                                p.Text = textNode.InnerText;
                                subtitle.Paragraphs.Add(p);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        _errorCount++;
                    }
                }
                subtitle.Sort(SubtitleSortCriteria.StartTime); // force order by start time
                subtitle.Renumber();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _errorCount = 1;
                return;
            }
        }

        private static TimeCode DecodeTimeCode(string time)
        {
            string[] arr = time.Split(new[] { '.', ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 3)
                return new TimeCode(0, int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
            return new TimeCode(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), int.Parse(arr[3]));
        }

        private static string EncodeTime(TimeCode timeCode)
        {
            //0:01:08.0
            return string.Format("{0}:{1:00}:{2:00}.{3}", timeCode.Hours, timeCode.Minutes, timeCode.Seconds, timeCode.Milliseconds);
        }

    }
}
