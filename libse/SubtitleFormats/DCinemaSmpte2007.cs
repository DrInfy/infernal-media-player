using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class DCinemaSmpte2007 : SubtitleFormat
    {
        //<?xml version="1.0" encoding="UTF-8"?>
        //<dcst:SubtitleReel xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:dcst="http://www.smpte-ra.org/schemas/428-7/2007/DCST">
        //  <Id>urn:uuid:7be835a3-cfb4-43d0-bb4b-f0b4c95e962e</Id>
        //  <ContentTitleText>2001, A Space Odissey</ContentTitleText>
        //  <AnnotationText>This is a subtitle file</AnnotationText>
        //  <IssueDate>2012-06-26T12:33:59.000-00:00</IssueDate>
        //  <ReelNumber>1</ReelNumber>
        //  <Language>fr</Language>
        //  <EditRate>25 1</EditRate>
        //  <TimeCodeRate>25</TimeCodeRate>
        //  <StartTime>00:00:00:00</StartTime>
        //  <LoadFont ID="theFontId">urn:uuid:3dec6dc0-39d0-498d-97d0-928d2eb78391</LoadFont>
        //  <SubtitleList
        //      <Font ID="theFontId" Size="39" Weight="normal" Color="FFFFFFFF">
        //      <Subtitle FadeDownTime="00:00:00:00" FadeUpTime="00:00:00:00" TimeOut="00:00:00:01" TimeIn="00:00:00:00" SpotNumber="1">
        //          <Text Vposition="10.0" Valign="bottom">Hallo</Text>
        //      </Subtitle>
        //  </SubtitleList
        //</dcst:SubtitleReel>

        public string Errors { get; private set; }

        private double _frameRate = 24;

        public int Version { get; set; }

        public override string Extension
        {
            get { return ".xml"; }
        }

        public override string Name
        {
            get { return "D-Cinema SMPTE 2007"; }
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

            if (xmlAsString.Contains("http://www.smpte-ra.org/schemas/428-7/2010/DCST"))
                return false;

            if (xmlAsString.Contains("<dcst:SubtitleReel") || xmlAsString.Contains("<SubtitleReel"))
            {
                var xml = new XmlDocument();
                try
                {
                    xmlAsString = xmlAsString.Replace("<dcst:", "<").Replace("</dcst:", "</");
                    xmlAsString = xmlAsString.Replace("xmlns=\"http://www.smpte-ra.org/schemas/428-7/2007/DCST\"", string.Empty);
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

        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            throw new Exception(e.Message);
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            _errorCount = 0;

            var sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            var xml = new XmlDocument { XmlResolver = null };
            xml.LoadXml(sb.ToString().Replace("<dcst:", "<").Replace("</dcst:", "</").Replace("xmlns=\"http://www.smpte-ra.org/schemas/428-7/2007/DCST\"", string.Empty)); // tags might be prefixed with namespace (or not)... so we just remove them

            var ss = Configuration.Settings.SubtitleSettings;
            try
            {
                ss.InitializeDCinameSettings(true);
                XmlNode node = xml.DocumentElement.SelectSingleNode("Id");
                if (node != null)
                    ss.CurrentDCinemaSubtitleId = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("ReelNumber");
                if (node != null)
                    ss.CurrentDCinemaReelNumber = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("EditRate");
                if (node != null)
                    ss.CurrentDCinemaEditRate = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("TimeCodeRate");
                if (node != null)
                {
                    ss.CurrentDCinemaTimeCodeRate = node.InnerText;
                    if (ss.CurrentDCinemaEditRate == "24")
                        Configuration.Settings.General.CurrentFrameRate = 24;
                    else if (ss.CurrentDCinemaEditRate == "25")
                        Configuration.Settings.General.CurrentFrameRate = 24;

                    if (BatchSourceFrameRate.HasValue)
                    {
                        Configuration.Settings.General.CurrentFrameRate = BatchSourceFrameRate.Value;
                    }
                }

                node = xml.DocumentElement.SelectSingleNode("StartTime");
                if (node != null)
                    ss.CurrentDCinemaStartTime = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("Language");
                if (node != null)
                    ss.CurrentDCinemaLanguage = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("ContentTitleText");
                if (node != null)
                    ss.CurrentDCinemaMovieTitle = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("IssueDate");
                if (node != null)
                    ss.CurrentDCinemaIssueDate = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("LoadFont");
                if (node != null)
                    ss.CurrentDCinemaFontUri = node.InnerText;

                node = xml.DocumentElement.SelectSingleNode("SubtitleList/Font");
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
                    StringBuilder pText = new StringBuilder();
                    string lastVPosition = string.Empty;
                    foreach (XmlNode innerNode in node.ChildNodes)
                    {
                        switch (innerNode.Name)
                        {
                            case "Text":
                                if (innerNode.Attributes["Vposition"] != null)
                                {
                                    string vPosition = innerNode.Attributes["Vposition"].InnerText;
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
                                if (innerNode.Attributes["Halign"] != null)
                                {
                                    string hAlign = innerNode.Attributes["Halign"].InnerText;
                                    if (hAlign == "left")
                                        alignLeft = true;
                                    else if (hAlign == "right")
                                        alignRight = true;
                                }
                                if (innerNode.Attributes["Valign"] != null)
                                {
                                    string hAlign = innerNode.Attributes["Valign"].InnerText;
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
                                        pText = new StringBuilder();
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

        private TimeCode GetTimeCode(string s)
        {
            var parts = s.Split(new[] { ':', '.', ',' });

            int milliseconds = (int)Math.Round(int.Parse(parts[3]) * (TimeCode.BaseUnit / _frameRate));
            if (milliseconds > 999)
                milliseconds = 999;

            return new TimeCode(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), milliseconds);
        }

        private string ConvertToTimeString(TimeCode time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", time.Hours, time.Minutes, time.Seconds, DCinemaSmpte2010.MsToFramesMaxFrameRate(time.Milliseconds, _frameRate));
        }

    }
}
