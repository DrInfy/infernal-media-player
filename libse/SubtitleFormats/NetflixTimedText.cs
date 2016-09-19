using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    /// <summary>
    /// Nexflix version of timed text - time code is "00:00:04.000", tag <br /> is converted to <br/>
    /// See https://backlothelp.netflix.com/hc/en-us/articles/215758617-Timed-Text-Style-Guide-General-Requirements
    /// </summary>
    public class NetflixTimedText : TimedText10
    {
        public override string Extension
        {
            get { return ".dfxp"; }
        }

        public new const string NameOfFormat = "Netflix Timed Text";

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
            if (fileName != null && !(fileName.EndsWith(Extension, StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)))
                return false;

            var sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            if (!sb.ToString().Contains("Netflix Subtitle"))
                return false;

            var subtitle = new Subtitle();
            LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > 0;
        }

        public override bool HasStyleSupport
        {
            get
            {
                return false;
            }
        }

    }
}
