using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace Imp.DirectShow.Subtitles
{
    public static class SubtitleFormatReader
    {
        public static SubtitleStylized ParseHeader(Subtitle subtitle, SubtitleFormat format)
        {
            var finalSubs = new SubtitleStylized();
            finalSubs.Header = new SubtitleHeader();
            finalSubs.Header.UseStyles = format.HasStyleSupport;

            var styles = new List<string>();
            if (format.GetType() == typeof(AdvancedSubStationAlpha) || format.GetType() == typeof(SubStationAlpha))
            {
                ReadAss(subtitle, finalSubs);
            }
            else if (format.GetType() == typeof(TimedText10) || format.GetType() == typeof(ItunesTimedText))
                styles = TimedText10.GetStylesFromHeader(subtitle.Header);
            else if (format.GetType() == typeof(Sami) || format.GetType() == typeof(SamiModern))
                styles = Sami.GetStylesFromHeader(subtitle.Header);

            //subtitle.Header
            //header.
            return finalSubs;
        }

        private static void ReadAss(Subtitle subtitle, SubtitleStylized finalSubs)
        {
            finalSubs.Header.IsAss = true;

            //ReadAdvancedSubStationAlpha(subtitle);
            finalSubs.Header.SubtitleStyles = AdvancedSubStationAlpha.GetSsaStyle(subtitle.Header).ToDictionary(x => x.Name);

            finalSubs.Header.PlayResX = ReadDefinitionInt(subtitle.Header, "PlayResX");
            finalSubs.Header.PlayResY = ReadDefinitionInt(subtitle.Header, "PlayResY");
        }

        private static void ReadAdvancedSubStationAlpha(Subtitle subtitle)
        {
            foreach (var VARIABLE in subtitle.Header.Split('\n'))
            {
                throw new NotImplementedException();
            }
        }

        private static int? ReadDefinitionInt(string subtitleHeader, string text)
        {
            var index = subtitleHeader.IndexOf(text, StringComparison.Ordinal);
            if (index > 0)
            {
                var end = subtitleHeader.IndexOf("\n", index, StringComparison.Ordinal);
                if (end > index)
                {
                    var result = subtitleHeader.Substring(index + text.Length+1, end - index - 2- text.Length);
                    int val;

                    if (int.TryParse(result, out val))
                    {
                        return val;
                    }
                }
            }

            return null;
        }

        public static void GetAssTags(string text, EnhancedParagraph enhancedParagraph)
        {
            //var original = text;
            var list = new List<SubtitleTag>();

            for (int i = 0; i < text.Length; i++)
            {
                var tagFound = true;

                while (tagFound)
                {
                    tagFound = false;
                    tagFound |= FindBraces(i, list, ref text);
                    tagFound |= FindChevrons(i, list, ref text);
                }
            }

            enhancedParagraph.Tags = list;
            enhancedParagraph.Text = text;
            //var kissa = original;
        }

        private static bool FindChevrons(int index, List<SubtitleTag> list, ref string text)
        {
            if (isChar(index, text, '<') && !isChar(index - 1, text, '\\'))
            {
                var end = text.IndexOf('>');
                var key = CutOff(index, ref text, end);
                var closingTag = "</" + key.Split(new[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() + ">";
                var startAnother = text.IndexOf(closingTag, StringComparison.Ordinal);

                if (startAnother >= 0)
                {
                    closingTag = CutOff(startAnother, ref text, startAnother + closingTag.Length - 1);
                    list.Add(new SubtitleTag(key, ParenthesisType.Chevrons, index, startAnother));
                    return true;
                }
                else
                {
                    list.Add(new SubtitleTag(key, ParenthesisType.Chevrons, index, null));
                    Debug.WriteLine("Invalid tag found");
                    //Debugger.Break();
                    return true;
                }
            }
            return false;
        }

        private static bool FindBraces(int index, List<SubtitleTag> list, ref string text)
        {
            if (isChar(index, text, '{') && !isChar(index - 1, text, '\\'))
            {
                var end = text.IndexOf('}');
                if (end > 0)
                {
                    var tag = CutOff(index, ref text, end);

                    var tags = tag.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var s in tags)
                    {
                        list.Add(new SubtitleTag(s, ParenthesisType.Braces, index, null));
                    }

                    return true;
                }
            }

            return false;
        }

        private static string CutOff(int index, ref string text, int end)
        {
            var cuttedText = text.Substring(index + 1, end - index - 1);
            if (index == 0)
            {
                if (text.Length > end + 1)
                {
                    text = text.Substring(end + 1);
                }
                else
                {
                    text = string.Empty;
                }
            }
            else
            {
                if (text.Length > end + 1)
                {
                    text = text.Substring(0, index) + text.Substring(end + 1);
                }
                else
                {
                    text = text.Substring(0, index);
                }
            }

            return cuttedText;
        }

        /// <summary>
        /// OIverFlow safe check
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool isChar(int index, string text, char c)
        {
            return index >= 0 && text.Length > index && text[index] == c;
        }

        
    }
}
