using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imp.DirectShow.Helpers;
using Nikse.SubtitleEdit.Core;
using SEdge.Core;

namespace Imp.DirectShow.Subtitles
{
    public class SubtitleStyle
    {
        public string Name { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
        public bool Italic { get; set; }
        public bool Bold { get; set; }
        public bool Underline { get; set; }
        public CustomColor Primary { get; set; }
        public CustomColor Secondary { get; set; }
        public CustomColor Tertiary { get; set; }
        public CustomColor Outline { get; set; }
        public CustomColor Background { get; set; }
        public int ShadowWidth { get; set; }
        public double OutlineWidth { get; set; }
        public string Alignment { get; set; }
        public int MarginLeft { get; set; }
        public int MarginRight { get; set; }
        public int MarginVertical { get; set; }
        public string BorderStyle { get; set; }
        public string RawLine { get; set; }
        public bool LoadedFromHeader { get; set; }

        public SubtitleStyle()
        {
            this.FontName = "Arial";
            this.FontSize = 32;
            this.Primary = CustomColor.White;
            this.Secondary = CustomColor.Yellow;
            this.Outline = CustomColor.Black;
            this.Background = CustomColor.Black;
            this.Alignment = "2";
            this.OutlineWidth = 1;
            this.ShadowWidth = 1;
            this.MarginLeft = 10;
            this.MarginRight = 10;
            this.MarginVertical = 10;
            this.BorderStyle = "1";
            this.RawLine = string.Empty;
            this.LoadedFromHeader = false;
        }

        public SubtitleStyle(SsaStyle ssaStyle)
        {
            this.Name = ssaStyle.Name;
            this.FontName = ssaStyle.FontName;
            this.FontSize = ssaStyle.FontSize;

            this.Italic = ssaStyle.Italic;
            this.Bold = ssaStyle.Bold;
            this.Underline = ssaStyle.Underline;

            this.Primary = ssaStyle.Primary;
            this.Secondary = ssaStyle.Secondary;
            this.Tertiary = ssaStyle.Tertiary;
            this.Outline = ssaStyle.Outline;
            this.Background = ssaStyle.Background;

            this.ShadowWidth = ssaStyle.ShadowWidth;
            this.OutlineWidth = ssaStyle.OutlineWidth;

            this.Alignment = ssaStyle.Alignment;
            this.MarginLeft = ssaStyle.MarginLeft;
            this.MarginRight = ssaStyle.MarginRight;
            this.MarginVertical = ssaStyle.MarginVertical;

            this.BorderStyle = ssaStyle.BorderStyle;
            this.RawLine = ssaStyle.RawLine;
            this.LoadedFromHeader = ssaStyle.LoadedFromHeader;
        }
    }
}
