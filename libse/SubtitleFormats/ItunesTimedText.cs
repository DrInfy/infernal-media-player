using System;
using System.Collections.Generic;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    /// <summary>
    /// Crappy format... should always be saved as UTF-8 without BOM (hacked Main.cs) and <br /> tags should be oldstyle <br/>
    /// </summary>
    public class ItunesTimedText : TimedText10
    {
        public override string Extension
        {
            get { return ".itt"; }
        }

        public new const string NameOfFormat = "iTunes Timed Text";

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
            if (fileName != null && !fileName.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
                return false;

            return base.IsMine(lines, fileName);
        }
    }
}
