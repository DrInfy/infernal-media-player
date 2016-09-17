using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Base.Subtitles;
using Imp.Libraries;
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
        private readonly DrawingVisual drawingVisual;

        public Subtitles()
        {
            drawingVisual = new DrawingVisual();
            RenderOptions.SetBitmapScalingMode(drawingVisual, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(drawingVisual, EdgeMode.Aliased);
            TextOptions.SetTextRenderingMode(drawingVisual, TextRenderingMode.Aliased);
        }

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

                foreach (var p in paragraphs)
                {
                    FormattedText fText;
                    Pen outlinePen;
                    var scale = new Size(ImageWidth / p.Header.PlayResX ?? 1, ImageHeight / p.Header.PlayResY ?? 1);
                    SsaStyle style;
                    Point finalPoint;

                    if (p.Header.UseStyles && p.Header.SubtitleStyles.TryGetValue(p.Paragraph.Extra, out style))
                    {
                        
                        outlinePen = new Pen(new SolidColorBrush(style.Outline.ColorConvert()), style.OutlineWidth * scale.Width);
                        fText = new FormattedText(p.Text, CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight, new Typeface(style.FontName), 
                            style.FontSize * scale.Width,
                            new SolidColorBrush() { Color = style.Primary.ColorConvert()});

                        fText.MaxTextWidth = ImageWidth - style.MarginLeft * scale.Width - style.MarginRight * scale.Width;
                        
                        finalPoint = SetStylePosition(point, style, scale, fText);

                    }
                    else
                    {
                        outlinePen = defaultOutlinePen;
                        outlinePen.Thickness = FontSize/30;

                        fText = new FormattedText(p.Text, CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight, defaultTypeface,
                            FontSize,
                            defaultBrush);

                        finalPoint = point + new Vector(0, ImageHeight * 0.75f - fText.Height);
                        fText.TextAlignment = TextAlignment.Center;
                        fText.MaxTextWidth = ImageWidth;
                    }

                    fText.MaxLineCount = 3;
                    //var lines = paragraph.Text.Split('\n').Length;
                    
                    
                    Process(fText, p.Text, p);


                    drawingContext.DrawText(fText, finalPoint);
                    Geometry textGeometry = fText.BuildGeometry(finalPoint);
                    drawingContext.DrawGeometry(null, outlinePen, textGeometry);

                    //using (var draw = drawingVisual.RenderOpen())
                    //{
                    //    draw.DrawText(fText, finalPoint);
                    //    Geometry textGeometry = fText.BuildGeometry(finalPoint);
                    //    draw.DrawGeometry(null, outlinePen, textGeometry);
                    //}
                    //var screenPoint = this.PointToScreen(new Point());
                    //var renderTargetBitmap = new RenderTargetBitmap(
                    //    (int) this.ActualWidth, (int) this.ActualHeight,
                    //    screenPoint.X, screenPoint.Y,
                    //    PixelFormats.Pbgra32);
                    //renderTargetBitmap.Render(drawingVisual);
                    //drawingContext.DrawImage(renderTargetBitmap, new Rect(new Size(this.ActualWidth, ActualHeight)));
                }
            }
        }

        private Point SetStylePosition(Point point, SsaStyle style, Size scale, FormattedText fText)
        {
            var finalPoint = point + new Vector(style.MarginLeft * scale.Width, style.MarginVertical);
            var h = ImageHeight - fText.Height;

            switch (style.Alignment)
            {
                case "9":
                    fText.TextAlignment = TextAlignment.Right;
                    break;
                case "6":
                    fText.TextAlignment = TextAlignment.Right;
                    finalPoint.Y += h / 2 - style.MarginVertical;
                    break;
                case "3":
                    fText.TextAlignment = TextAlignment.Right;
                    finalPoint.Y += ImageHeight - style.MarginVertical;
                    break;
                case "8":
                    fText.TextAlignment = TextAlignment.Center;
                    finalPoint.Y += h - style.MarginVertical;
                    break;
                case "5":
                    finalPoint.Y += h / 2 - style.MarginVertical;
                    fText.TextAlignment = TextAlignment.Center;
                    break;
                case "2":
                    fText.TextAlignment = TextAlignment.Center;
                    finalPoint.Y += h - style.MarginVertical;
                    break;
                case "7":
                    fText.TextAlignment = TextAlignment.Left;
                    break;
                case "4":
                    fText.TextAlignment = TextAlignment.Left;
                    finalPoint.Y += h / 2 - style.MarginVertical;
                    break;
                case "1":
                    fText.TextAlignment = TextAlignment.Left;
                    finalPoint.Y += h - style.MarginVertical;
                    break;
                default:
                    fText.TextAlignment = TextAlignment.Left;
                    finalPoint.Y += h - style.MarginVertical;
                    break;
            }

            return finalPoint;
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
                var e = Math.Min((tag.EndIndex ?? 10000) - tag.StartIndex, text.Length - tag.StartIndex);

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
