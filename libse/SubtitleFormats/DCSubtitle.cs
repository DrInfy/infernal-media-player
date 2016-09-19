using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{

    // https://github.com/SubtitleEdit/subtitleedit/issues/detail?id=18
    //<?xml version="1.0" encoding="UTF-8"?>
    //<DCSubtitle Version="1.0">
    //  <SubtitleID>4EB245B8-4D3A-4158-9516-95DD20E8322E</SubtitleID>
    //  <MovieTitle>Unknown</MovieTitle>
    //  <ReelNumber>1</ReelNumber>
    //  <Language>Swedish</Language>
    //  <Font Italic="no">
    //    <Subtitle SpotNumber="1" TimeIn="00:00:06:040" TimeOut="00:00:08:040" FadeUpTime="20" FadeDownTime="20">
    //      <Text Direction="horizontal" HAlign="center" HPosition="0.0" VAlign="bottom" VPosition="6.0">DETTA HAR HÄNT...</Text>
    //    </Subtitle>
    //  </Font>
    //</DCSubtitle>

    public class DCSubtitle : SubtitleFormat
    {
        public override string Extension
        {
            get { return ".xml"; }
        }

        public override string Name
        {
            get { return "D-Cinema interop"; }
        }

        public override bool IsTimeBased
        {
            get { return true; }
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            var sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            string xmlAsString = sb.ToString().Trim();
            if (xmlAsString.Contains("<DCSubtitle"))
            {
                var xml = new XmlDocument { XmlResolver = null };
                try
                {
                    xml.LoadXml(xmlAsString);

                    var subtitles = xml.DocumentElement.SelectNodes("//Subtitle");
                    return subtitles != null && subtitles.Count > 0;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
            }
            return false;
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            _errorCount = 0;

            var sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            var xml = new XmlDocument { XmlResolver = null };
            xml.LoadXml(sb.ToString().Trim());

            var ss = Configuration.Settings.SubtitleSettings;
            try
            {
                ss.InitializeDCinameSettings(false);
                XmlNode node = xml.DocumentElement.SelectSingleNode("SubtitleID");
                if (node != null)
                    ss.CurrentDCinemaSubtitleId = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("ReelNumber");
                if (node != null)
                    ss.CurrentDCinemaReelNumber = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("Language");
                if (node != null)
                    ss.CurrentDCinemaLanguage = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("MovieTitle");
                if (node != null)
                    ss.CurrentDCinemaMovieTitle = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("LoadFont");
                if (node != null)
                {
                    if (node.Attributes["URI"] != null)
                        ss.CurrentDCinemaFontUri = node.Attributes["URI"].InnerText;
                }

                node = xml.DocumentElement.SelectSingleNode("Font");
                if (node != null)
                {
                    if (node.Attributes["ID"] != null)
                        ss.CurrentDCinemaFontId = node.Attributes["ID"].InnerText;
                    if (node.Attributes["Size"] != null)
                        ss.CurrentDCinemaFontSize = Convert.ToInt32(node.Attributes["Size"].InnerText);
                    if (node.Attributes["Color"] != null)
                        ss.CurrentDCinemaFontColor = System.Drawing.ColorTranslator.FromHtml("#" + node.Attributes["Color"].InnerText);
                    if (node.Attributes["Effect"] != null)
                        ss.CurrentDCinemaFontEffect = node.Attributes["Effect"].InnerText;
                    if (node.Attributes["EffectColor"] != null)
                        ss.CurrentDCinemaFontEffectColor = System.Drawing.ColorTranslator.FromHtml("#" + node.Attributes["EffectColor"].InnerText);
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
            }

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("//Subtitle"))
            {
                try
                {
                    var pText = new StringBuilder();
                    string lastVPosition = string.Empty;
                    foreach (XmlNode innerNode in node.ChildNodes)
                    {
                        switch (innerNode.Name)
                        {
                            case "Text":
                                if (innerNode.Attributes["VPosition"] != null)
                                {
                                    string vPosition = innerNode.Attributes["VPosition"].InnerText;
                                    if (vPosition != lastVPosition)
                                    {
                                        if (pText.Length > 0 && lastVPosition.Length > 0)
                                            pText.AppendLine();
                                        lastVPosition = vPosition;
                                    }
                                }
                                bool alignLeft = false;
                                bool alignRight = false;
                                bool alignVTop = false;
                                bool alignVCenter = false;
                                if (innerNode.Attributes["HAlign"] != null)
                                {
                                    string hAlign = innerNode.Attributes["HAlign"].InnerText;
                                    if (hAlign == "left")
                                        alignLeft = true;
                                    else if (hAlign == "right")
                                        alignRight = true;
                                }
                                if (innerNode.Attributes["VAlign"] != null)
                                {
                                    string hAlign = innerNode.Attributes["VAlign"].InnerText;
                                    if (hAlign == "top")
                                        alignVTop = true;
                                    else if (hAlign == "center")
                                        alignVCenter = true;
                                }
                                if (alignLeft || alignRight || alignVCenter || alignVTop)
                                {
                                    if (!pText.ToString().StartsWith("{\\an"))
                                    {
                                        string pre = string.Empty;
                                        if (alignVTop)
                                        {
                                            if (alignLeft)
                                                pre = "{\\an7}";
                                            else if (alignRight)
                                                pre = "{\\an9}";
                                            else
                                                pre = "{\\an8}";
                                        }
                                        else if (alignVCenter)
                                        {
                                            if (alignLeft)
                                                pre = "{\\an4}";
                                            else if (alignRight)
                                                pre = "{\\an6}";
                                            else
                                                pre = "{\\an5}";
                                        }
                                        else
                                        {
                                            if (alignLeft)
                                                pre = "{\\an1}";
                                            else if (alignRight)
                                                pre = "{\\an3}";
                                        }
                                        string temp = pre + pText;
                                        pText.Clear();
                                        pText.Append(temp);
                                    }
                                }

                                if (innerNode.ChildNodes.Count == 0)
                                {
                                    pText.Append(innerNode.InnerText);
                                }
                                else
                                {
                                    foreach (XmlNode innerInnerNode in innerNode)
                                    {
                                        if (innerInnerNode.Name == "Font" && innerInnerNode.Attributes["Italic"] != null &&
                                           innerInnerNode.Attributes["Italic"].InnerText.Equals("yes", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (innerInnerNode.Attributes["Color"] != null)
                                                pText.Append("<i><font color=\"" + GetColorStringFromDCinema(innerInnerNode.Attributes["Color"].Value) + "\">" + innerInnerNode.InnerText + "</font><i>");
                                            else
                                                pText.Append("<i>" + innerInnerNode.InnerText + "</i>");
                                        }
                                        else if (innerInnerNode.Name == "Font" && innerInnerNode.Attributes["Color"] != null)
                                        {
                                            if (innerInnerNode.Attributes["Italic"] != null && innerInnerNode.Attributes["Italic"].InnerText.Equals("yes", StringComparison.OrdinalIgnoreCase))
                                                pText.Append("<i><font color=\"" + GetColorStringFromDCinema(innerInnerNode.Attributes["Color"].Value) + "\">" + innerInnerNode.InnerText + "</font><i>");
                                            else
                                                pText.Append("<font color=\"" + GetColorStringFromDCinema(innerInnerNode.Attributes["Color"].Value) + "\">" + innerInnerNode.InnerText + "</font>");
                                        }
                                        else
                                        {
                                            pText.Append(innerInnerNode.InnerText);
                                        }
                                    }
                                }
                                break;
                            default:
                                pText.Append(innerNode.InnerText);
                                break;
                        }
                    }
                    string start = node.Attributes["TimeIn"].InnerText;
                    string end = node.Attributes["TimeOut"].InnerText;

                    if (node.ParentNode.Name == "Font" && node.ParentNode.Attributes["Italic"] != null && node.ParentNode.Attributes["Italic"].InnerText.Equals("yes", StringComparison.OrdinalIgnoreCase) &&
                        !pText.ToString().Contains("<i>"))
                    {
                        string text = pText.ToString();
                        if (text.StartsWith("{\\an") && text.Length > 6)
                            text = text.Insert(6, "<i>") + "</i>";
                        else
                            text = "<i>" + text + "</i>";
                        pText = new StringBuilder(text);
                    }

                    subtitle.Paragraphs.Add(new Paragraph(GetTimeCode(start), GetTimeCode(end), pText.ToString()));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    _errorCount++;
                }
            }

            if (subtitle.Paragraphs.Count > 0)
                subtitle.Header = xml.OuterXml; // save id/language/font for later use

            subtitle.Renumber();
        }

        private static string GetColorStringFromDCinema(string p)
        {
            string s = p.ToLower().Trim();
            if (s.Replace("#", string.Empty).
                Replace("0", string.Empty).
                Replace("1", string.Empty).
                Replace("2", string.Empty).
                Replace("3", string.Empty).
                Replace("4", string.Empty).
                Replace("5", string.Empty).
                Replace("6", string.Empty).
                Replace("7", string.Empty).
                Replace("8", string.Empty).
                Replace("9", string.Empty).
                Replace("a", string.Empty).
                Replace("b", string.Empty).
                Replace("c", string.Empty).
                Replace("d", string.Empty).
                Replace("e", string.Empty).
                Replace("f", string.Empty).Length == 0)
            {
                if (s.StartsWith('#'))
                    return s;
                return "#" + s;
            }
            return p;
        }

        private static TimeCode GetTimeCode(string s)
        {
            var parts = s.Split(':', '.', ',');

            int milliseconds = int.Parse(parts[3]) * 4; // 000 to 249
            if (s.Contains('.'))
                milliseconds = int.Parse(parts[3].PadRight(3, '0'));
            if (milliseconds > 999)
                milliseconds = 999;

            return new TimeCode(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), milliseconds);
        }

        public static string ConvertToTimeString(TimeCode time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}:{3:000}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds / 4);
        }

    }
}
