using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Base.Subtitles;
using Nikse.SubtitleEdit.Core;

namespace Imp.Controls
{
    public class Subtitles : Control
    {
        private readonly Brush defaultBrush = new SolidColorBrush {Color = Colors.White};
        private readonly Pen defaultOutlinePen= new Pen(new SolidColorBrush(Colors.Black), 3);
        private readonly Typeface defaultTypeface = new Typeface("Verdana");

        private List<EnhancedParagraph> paragraphs { get; set; } = new List<EnhancedParagraph>();
        //public double FontSize { get; set; }

        public double ImageWidth { get; set; }
        public double ImageHeight { get; set; }

        public void Clear()
        {
            this.paragraphs.Clear();
            this.InvalidateVisual();
        }

        public void Add(EnhancedParagraph p)
        {
            paragraphs.Add(p);
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (paragraphs != null)
            {
                var top = (ActualHeight - ImageHeight) /2;
                var left = (ActualWidth - ImageWidth) / 2;
                var point = new Point(left, top);

                foreach (var paragraph in paragraphs)
                {
                    var fText = new FormattedText(paragraph.Text, CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight, defaultTypeface,
                        FontSize,
                        defaultBrush);
                    fText.MaxLineCount = 3;
                    defaultOutlinePen.Thickness = FontSize/30;
                    //var lines = paragraph.Text.Split('\n').Length;
                    fText.TextAlignment = TextAlignment.Center;
                    fText.MaxTextWidth = ImageWidth;
                    
                    Process(fText, paragraph.Text, paragraph);

                    //fText.SetForegroundBrush(
                    //        new SolidColorBrush(Colors.Aqua),
                    //        3, paragraph.Text.Length- 6);

                    var finalPoint = point + new Vector(0, ImageHeight*0.75f - fText.Height);
                    drawingContext.DrawText(fText, finalPoint);

                    Geometry textGeometry = fText.BuildGeometry(finalPoint);
                    drawingContext.DrawGeometry(null, defaultOutlinePen, textGeometry);
                }
            }
        }

        public void Process(FormattedText fText, string text, EnhancedParagraph paragraph)
        {
            //var color2 = (Color)ColorConverter.ConvertFromString("#a17c51");
            //fText.SetForegroundBrush(
            //                new SolidColorBrush(color2),
            //                0, 1);
            foreach (var tag in paragraph.Tags)
            {
                var s = tag.StartIndex;
                var e = (tag.EndIndex ?? text.Length) - tag.StartIndex;
                if (tag.Type == ParenthesisType.Chevrons)
                {
                    if (tag.Tag == "i")
                    {
                        fText.SetFontStyle(FontStyles.Italic, s, e);
                    }
                    else if (tag.Tag == "b")
                    {
                        fText.SetFontStyle(FontStyles.Oblique, s, e);
                    }
                    else if (tag.Tag == "font")
                    {
                        if (tag.AdditionalContent.Contains("color"))
                        {
                            var index = tag.AdditionalContent.IndexOf("color");
                            index = tag.AdditionalContent.IndexOf("\"", index + 1);
                            var endIndex = tag.AdditionalContent.IndexOf("\"", index + 1);

                            var colorHex = tag.AdditionalContent.Substring(index + 1, endIndex - index - 1);
                            var color = (Color)ColorConverter.ConvertFromString(colorHex);
                            fText.SetForegroundBrush(
                                new SolidColorBrush(color),
                                s, e);
                        }
                    }
                }
                else if (tag.Type == ParenthesisType.Braces)
                {
                    if (tag.Tag == "i1")
                    {
                        fText.SetFontStyle(FontStyles.Italic, s, e);
                    }
                    else if (tag.Tag == "i0")
                    {
                        fText.SetFontStyle(FontStyles.Normal, s, e);
                    }
                    else if (tag.Tag == "b1" || tag.Tag == "b700" || tag.Tag == "b800" || tag.Tag == "b900")
                    {
                        fText.SetFontStyle(FontStyles.Oblique, s, e);
                    }
                    else if (tag.Tag == "b0" || tag.Tag == "b400")
                    {
                        fText.SetFontStyle(FontStyles.Normal, s, e);
                    }
                    else if (tag.Tag.StartsWith("c&H"))
                    {
                        var index = tag.Tag.IndexOf("&H");
                        var endIndex = tag.Tag.IndexOf("&", index + 1);

                        var colorHex = "#" + tag.Tag.Substring(index + 2, endIndex - index - 2);
                        var color = (Color)ColorConverter.ConvertFromString(colorHex);
                        fText.SetForegroundBrush(
                            new SolidColorBrush(color),
                            s, e);
                    }
                }
            }
        }
    }
}
