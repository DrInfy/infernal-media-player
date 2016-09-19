using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Base.Libraries;
using Base.Subtitles;
using Imp.Libraries;
using Nikse.SubtitleEdit.Core;

namespace MediaPlayer.Element
{
    public class SubtitleElement : Grid
    {
        private static double fontScale = 72.0 / 120.0;
        private readonly Brush defaultBrush = new SolidColorBrush {Color = Colors.White};
        private readonly Pen defaultOutlinePen = new Pen(new SolidColorBrush(Colors.Black), 3);
        private readonly Typeface defaultTypeface = new Typeface("Verdana");
        //private readonly BlurEffect blurEffect = new BlurEffect();
        //private readonly DropShadowEffect dropShadowEffect= new DropShadowEffect();

        private readonly List<SubtitleChildElement> controls = new List<SubtitleChildElement>();

        private List<EnhancedParagraph> paragraphs { get; set; } = new List<EnhancedParagraph>();
        //public double FontSize { get; set; }

        private Dictionary<string, FontFamily> fontTypefaces { get; set; } = new Dictionary<string, FontFamily>();

        private double imageWidth;
        private double imageHeight;
        private double bottomReserved;
        private double topReserved;
        
        /// <summary> 
        /// Store added texts so that duplicate entries will properly collide with each other.
        /// Contains used reserved value.
        /// </summary>
        private Dictionary<string,double> addedTexts = new Dictionary<string, double>();
        public Func<double> ImageWidthFunc { get; set;
            //get { return _imageWidth; }
            //set
            //{
            //    if (_imageWidth != value)
            //    {
            //        InvalidateVisual();
            //    }
            //    _imageWidth = value;
            //}
        }

        public Func<double> ImageHeightFunc { get; set;
            //get { return _imageHeight; }
            //set
            //{
            //    if (_imageHeight != value)
            //    {
            //        InvalidateVisual();
            //    }
            //    _imageHeight = value;
            //}
        }

        public SsaStyle defaultStyle { get; private set; }


        private readonly DrawingVisual drawingVisual;
        private double _imageWidth;
        private double _imageHeight;

        public SubtitleElement()
        {
            //drawingVisual = new DrawingVisual();
            //RenderOptions.SetBitmapScalingMode(drawingVisual, BitmapScalingMode.HighQuality);
            //RenderOptions.SetEdgeMode(drawingVisual, EdgeMode.Aliased);
            //TextOptions.SetTextRenderingMode(drawingVisual, TextRenderingMode.Aliased);

            for (int i = 0; i < 100; i++)
            {
                var child = new SubtitleChildElement
                {

                    Name = "childSubs" + i
                };
                //this.AddChild(child);
                this.Children.Add(child);
                controls.Add(child);
            }

            defaultStyle = new SsaStyle()
            {
                Alignment = "2",
                Background = System.Drawing.Color.Black,
                Bold = false,
                BorderStyle = "1",
                FontName = "Arial",
                FontSize = 42,
                Italic = false,
                MarginVertical = 60,
                MarginRight = 60,
                MarginLeft = 60,
                Name = "SystemDefault",
                Primary = System.Drawing.Color.White,
                Outline = System.Drawing.Color.Black,
                OutlineWidth = 2,
                Tertiary = System.Drawing.Color.Black
            };
        }

        public void Clear()
        {
            ClearContent();
            fontTypefaces.Clear();
        }

        public void ClearContent()
        {
            foreach (var control in controls)
            {
                control.ClearContent();
            }

            
            this.Effect = null;
            this.paragraphs.Clear();
            //this.InvalidateMeasure();
            //this.InvalidateArrange();
            this.InvalidateVisual();
            //this.UpdateLayout();
        }

        public void Add(EnhancedParagraph p)
        {
            paragraphs.Add(p);
            this.InvalidateVisual();
        }

        public void AddFont(string fontFaceName, FontFamily fontFamily)
        {
            if (!fontTypefaces.ContainsKey(fontFaceName.ToLower()))
            {
                //TODO: remove this check
                fontTypefaces.Add(fontFaceName.ToLower(), fontFamily);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            imageWidth = ImageWidthFunc();
            imageHeight = ImageHeightFunc();

            if (paragraphs != null && imageWidth > 0 && imageHeight > 0)
            {
                addedTexts.Clear();
                var controlIndex = 0;
                topReserved = 0;
                bottomReserved = 0;

                for (int i = paragraphs.Count - 1; i >= 0; i--)
                {
                    var p = paragraphs[i];
                    var mainControl = controls[controlIndex * 2 + 1];
                    var outLineControl = controls[controlIndex * 2];

                    controlIndex++;
                    if (controlIndex > controls.Count)
                    {
                        // No can do.
                        Debugger.Break();
                        base.OnRender(drawingContext);
                        return;
                    }

                    FormattedText fText;
                    Pen outlinePen;
                    var scale = new Size(imageWidth / p.Header.PlayResX ?? 1, imageHeight / p.Header.PlayResY ?? 1);
                    SsaStyle style;
                    Point finalPoint;

                    if (p.Header.UseStyles && p.Header.SubtitleStyles.TryGetValue(p.Paragraph.Extra, out style))
                    {
                    }
                    else
                    {
                        style = defaultStyle;
                    }

                    Typeface typeface = null;
                    FontFamily fontFamily;

                    if (fontTypefaces.TryGetValue(style.FontName, out fontFamily))
                    {
                        //typeface = fontFamily.GetTypefaces().FirstOrDefault();
                        typeface = new Typeface(fontFamily,
                            style.Italic ? FontStyles.Italic : FontStyles.Normal,
                            style.Bold ? FontWeights.Bold : FontWeights.Normal,
                            FontStretches.SemiCondensed);

                        //drawingContext.
                        //fontFamily.
                        //GlyphTypeface g;
                        //typeface.TryGetGlyphTypeface(out g);
                        //g.Stretch
                    }

                    if (typeface == null)
                    {
                        typeface = new Typeface(style.FontName);
                    }

                    outlinePen = outLineControl.GeometryPen;
                    outlinePen.Brush = new SolidColorBrush(style.Outline.ColorConvert());
                    outlinePen.Thickness = style.OutlineWidth * scale.Width * 2;
                    //var textt = p.Text.Replace("\r\n\r\n\r\n", "      ");
                    fText = new FormattedText(p.Text, CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight, typeface,
                        style.FontSize * scale.Width * fontScale,
                        new SolidColorBrush() {Color = style.Primary.ColorConvert()});
                    fText.Trimming = TextTrimming.None;

                    finalPoint = Process(fText, p.Text, p, style, scale, outlinePen, mainControl, outLineControl);


                    //var lines = paragraph.Text.Split('\n').Length;


                    mainControl.FormattedText = fText; // drawingContext.DrawText(fText, finalPoint);
                    mainControl.FormattedTextPos = finalPoint;
                    mainControl.InvalidateVisual();
                    mainControl.Visibility = Visibility.Visible;
                    //mainControl.Draw();
                    if (outlinePen.Thickness > 0)
                    {
                        outLineControl.Geometry = fText.BuildGeometry(finalPoint);
                        outLineControl.InvalidateVisual();
                        outLineControl.Visibility = Visibility.Visible;
                        //outLineControl.Draw();
                    }
                    //Geometry textGeometry = fText.BuildGeometry(finalPoint);
                    //
                    //drawingContext.DrawGeometry(null, outlinePen, textGeometry);

                    //using (var draw = drawingVisual.RenderOpen())
                    //{
                    //    draw.DrawText(fText, finalPoint);
                    //    Geometry textGeometry = fText.BuildGeometry(finalPoint);
                    //    draw.DrawGeometry(null, outlinePen, textGeometry);
                    //}
                    //var screenPoint = this.PointToScreen(new Point());
                    //var renderTargetBitmap = new RenderTargetBitmap(
                    //    (int)this.ActualWidth, (int)this.ActualHeight,
                    //    screenPoint.X, screenPoint.Y,
                    //    PixelFormats.Pbgra32);


                    //renderTargetBitmap.Render(drawingVisual);
                    //drawingContext.DrawImage(renderTargetBitmap, new Rect(new Size(this.ActualWidth, ActualHeight)));
                }
            }
            base.OnRender(drawingContext);
        }


        private Point SetStylePosition(Point? point, SsaStyle style, Size scale, FormattedText fText, string alignment, Point leftTopCorner, EnhancedParagraph paragraph)
        {
            var lMargin = style.MarginLeft * scale.Width;
            var rMargin = style.MarginRight * scale.Width;
            var vMargin = style.MarginVertical * scale.Width;


            fText.MaxTextWidth = imageWidth - lMargin - rMargin;

            var h = imageHeight - fText.Height;
            Point finalPoint;

            if (point != null)
            {
                //finalPoint = leftTopCorner + 
                //             new Vector(style.MarginLeft * scale.Width, vMargin);
                finalPoint = leftTopCorner + new Vector(scale.Width * point.Value.X, scale.Height * point.Value.Y);

                switch (alignment ?? style.Alignment)
                {
                    case "9":
                        fText.MaxTextWidth = point.Value.X - finalPoint.X - lMargin;
                        finalPoint.X -= fText.MaxTextWidth;
                        fText.TextAlignment = TextAlignment.Right;
                        break;
                    case "6":
                        fText.MaxTextWidth = finalPoint.X - leftTopCorner.X - lMargin;
                        finalPoint.Y -= fText.Height / 2;
                        fText.TextAlignment = TextAlignment.Right;
                        finalPoint.X -= fText.MaxTextWidth;
                        break;
                    case "3":
                        fText.MaxTextWidth = point.Value.X - finalPoint.X - lMargin;
                        finalPoint.Y = finalPoint.Y - fText.Height - vMargin * 2;
                        finalPoint.X -= fText.MaxTextWidth;
                        fText.TextAlignment = TextAlignment.Right;
                        break;
                    case "8":
                        fText.MaxTextWidth = Math.Min(imageWidth - finalPoint.X - rMargin + leftTopCorner.X,
                                                 finalPoint.X - lMargin - leftTopCorner.X) * 2;
                        finalPoint.X -= fText.MaxTextWidth / 2;
                        fText.TextAlignment = TextAlignment.Center;
                        break;
                    case "5":
                        fText.MaxTextWidth = Math.Min(imageWidth - finalPoint.X - rMargin + leftTopCorner.X,
                                                 finalPoint.X - lMargin - leftTopCorner.X) * 2;
                        finalPoint.X -= fText.MaxTextWidth / 2;
                        finalPoint.Y = finalPoint.Y - fText.Height / 2 - vMargin;
                        fText.TextAlignment = TextAlignment.Center;
                        break;
                    case "7":
                        fText.MaxTextWidth = imageWidth - finalPoint.X - rMargin;
                        fText.TextAlignment = TextAlignment.Left;
                        break;
                    case "4":
                        fText.MaxTextWidth = imageWidth - finalPoint.X - rMargin;
                        finalPoint.Y = finalPoint.Y - fText.Height / 2;
                        fText.TextAlignment = TextAlignment.Left;
                        break;
                    case "1":
                        fText.MaxTextWidth = imageWidth - finalPoint.X - rMargin;
                        finalPoint.Y = finalPoint.Y - fText.Height;
                        fText.TextAlignment = TextAlignment.Left;
                        break;
                    case "2":
                    default:
                        fText.MaxTextWidth = Math.Min(imageWidth - finalPoint.X - rMargin + leftTopCorner.X,
                                                 finalPoint.X - lMargin - leftTopCorner.X) * 2;
                        finalPoint.X -= fText.MaxTextWidth / 2;
                        finalPoint.Y = finalPoint.Y - fText.Height;
                        fText.TextAlignment = TextAlignment.Center;
                        break;
                }
            }
            else
            {
                finalPoint = leftTopCorner +
                             new Vector(lMargin, vMargin);

                switch (alignment ?? style.Alignment)
                {
                    case "9":
                        fText.TextAlignment = TextAlignment.Right;
                        PreventCollisionTop(fText, paragraph, ref finalPoint);
                        break;
                    case "6":
                        fText.TextAlignment = TextAlignment.Right;
                        finalPoint.Y += h / 2 - vMargin;
                        break;
                    case "3":
                        fText.TextAlignment = TextAlignment.Right;
                        PreventCollision(fText, paragraph, ref finalPoint, h, vMargin);
                        break;
                    case "8":
                        fText.TextAlignment = TextAlignment.Center;
                        PreventCollisionTop(fText, paragraph, ref finalPoint);
                        break;
                    case "5":
                        finalPoint.Y += h / 2 - vMargin;
                        fText.TextAlignment = TextAlignment.Center;
                        break;
                    case "2":
                        fText.TextAlignment = TextAlignment.Center;
                        PreventCollision(fText, paragraph, ref finalPoint, h, vMargin);
                        break;
                    case "7":
                        fText.TextAlignment = TextAlignment.Left;
                        PreventCollisionTop(fText, paragraph, ref finalPoint);
                        break;
                    case "4":
                        fText.TextAlignment = TextAlignment.Left;
                        finalPoint.Y += h / 2 - vMargin;
                        break;
                    case "1":
                        fText.TextAlignment = TextAlignment.Left;
                        finalPoint.Y += h - vMargin * 2;
                        PreventCollision(fText, paragraph, ref finalPoint, h, vMargin);
                        break;
                    default:
                        fText.TextAlignment = TextAlignment.Left;
                        finalPoint.Y += h - vMargin * 2;
                        break;
                }
            }

            return finalPoint;
        }

        private void PreventCollision(FormattedText fText, EnhancedParagraph paragraph, ref Point finalPoint, double h,
            double vMargin)
        {
            if (addedTexts.ContainsKey(paragraph.Text))
            {
                finalPoint.Y += h - vMargin * 2 - addedTexts[paragraph.Text];
            }
            else
            {
                finalPoint.Y += h - vMargin * 2 - bottomReserved;
                addedTexts.Add(paragraph.Text, topReserved);
                bottomReserved += fText.Height;
            }
        }
        private void PreventCollisionTop(FormattedText fText, EnhancedParagraph paragraph, ref Point finalPoint)
        {
            if (addedTexts.ContainsKey(paragraph.Text))
            {
                finalPoint.Y += addedTexts[paragraph.Text];
            }
            else
            {
                finalPoint.Y += topReserved;
                addedTexts.Add(paragraph.Text, topReserved);
                topReserved += fText.Height;
            }
        }

        public Point Process(FormattedText fText, string text, EnhancedParagraph paragraph, SsaStyle style, Size scale,
            Pen outlinePen, SubtitleChildElement mainControl, SubtitleChildElement outlineControl)
        {
            var top = (ActualHeight - imageHeight) / 2;
            var left = (ActualWidth - imageWidth) / 2;
            var leftTopCorner = new Point(left, top);
            var lt = (Vector) leftTopCorner;
            var shadowColor = style.Background.ColorConvert();

            Point? pos = null;
            string alignment = null;
            double? blur = null;
            double? shadow = null;
            double? bord = null;

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
                        if (tag.AdditionalContent.Contains("color="))
                        {
                            var index = tag.AdditionalContent.IndexOf("color=");
                            index = tag.AdditionalContent.IndexOf("\"", index + 1);
                            var endIndex = tag.AdditionalContent.IndexOf("\"", index + 1);

                            var colorHex = tag.AdditionalContent.Substring(index + 1, endIndex - index - 1);
                            var color = (Color) ColorConverter.ConvertFromString(colorHex);
                            fText.SetForegroundBrush(
                                new SolidColorBrush(color),
                                s, e);
                        }
                        if (tag.AdditionalContent.Contains("size="))
                        {
                            var content = LibImp.GetBetween(tag.AdditionalContent, "size=\"", "\"");
                            double val;
                            if (double.TryParse(content, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                                out val))
                            {
                                fText.SetFontSize(val * scale.Height * fontScale, s, e);
                            }
                        }
                        if (tag.AdditionalContent.Contains("face="))
                        {
                            var content = LibImp.GetBetween(tag.AdditionalContent, "face=\"", "\"");
                            FontFamily fontFamily;
                            if (content == null || !this.fontTypefaces.TryGetValue(content.ToLower(), out fontFamily))
                            {
                                fontFamily = new FontFamily(content);
                            }

                            var typeface = new Typeface(fontFamily,
                                style.Italic ? FontStyles.Italic : FontStyles.Normal,
                                style.Bold ? FontWeights.Bold : FontWeights.Normal,
                                FontStretches.SemiCondensed);

                            fText.SetFontTypeface(typeface, s, e);
                        }
                    }
                }
                else if (tag.Type == ParenthesisType.Braces)
                {
                    if (tag.Tag.StartsWith("pos"))
                    {
                        var content = LibImp.GetBetween(tag.Tag, "(", ")");
                        if (content != null)
                        {
                            var parts = content.Split(',');
                            int x, y;
                            if (parts.Length == 2 && int.TryParse(parts[0], out x) && int.TryParse(parts[1], out y))
                            {
                                pos = new Point(x, y);
                            }
                        }
                    }
                    else if (tag.Tag.StartsWith("an"))
                    {
                        var content = LibImp.GetAfter(tag.Tag, "an");
                        if (content != null)
                        {
                            alignment = content;
                        }
                    }
                    else if (tag.Tag.StartsWith("bord"))
                    {
                        var content = LibImp.GetAfter(tag.Tag, "bord");
                        if (content != null)
                        {
                            double val;
                            if (double.TryParse(content, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                                out val))
                            {
                                bord = val * scale.Height * 1.5;
                                outlinePen.Thickness = bord.Value;
                            }
                        }
                    }
                    else if (tag.Tag.StartsWith("shad"))
                    {
                        var content = LibImp.GetAfter(tag.Tag, "shad");
                        if (content != null)
                        {
                            double val;
                            if (double.TryParse(content, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                                out val))
                            {
                                shadow = val;
                            }
                        }
                    }
                    else if (tag.Tag.StartsWith("clip"))
                    {
                        var content = LibImp.GetBetween(tag.Tag, "(", ")");
                        if (content != null)
                        {
                            var parts = content.Split(',', ' ');
                            if (parts.Length == 4)
                            {
                                // Rectangle clip
                                var pathFigure = new PathFigure() {IsClosed = true};
                                var pG = new PathGeometry();
                                var point = LibImp.ToPoint(parts[0], parts[1], scale, lt);
                                var point2 = LibImp.ToPoint(parts[2], parts[3], scale, lt);
                                pathFigure.StartPoint = point;

                                pathFigure.Segments.Add(new LineSegment() {Point = new Point(point.X, point2.Y)});
                                pathFigure.Segments.Add(new LineSegment() {Point = new Point(point2.X, point2.Y)});
                                pathFigure.Segments.Add(new LineSegment() {Point = new Point(point2.X, point.Y)});
                                pathFigure.Segments.Add(new LineSegment() {Point = new Point(point.X, point.Y)});

                                pG.Figures.Add(pathFigure);
                                mainControl.Clip = pG;

                                mainControl.Clip = new RectangleGeometry()
                                {
                                    Rect = new Rect(point, point2)
                                };
                            }
                            else if (parts.FirstOrDefault() == "m" && parts.Length > 3)
                            {
                                //var scales = new Size(1,1);
                                var point = LibImp.ToPoint(parts[1], parts[2], scale, lt);

                                if (parts[3] == "l")
                                {
                                    // Line geometry is simple enough.
                                    var pathFigure = new PathFigure() {IsClosed = true};
                                    var pG = new PathGeometry();
                                    var index = 4;

                                    pathFigure.StartPoint = point;

                                    while (index + 1 < parts.Length)
                                    {
                                        var nextPoint = LibImp.ToPoint(parts[index], parts[index + 1], scale, lt);
                                        pathFigure.Segments.Add(new LineSegment() {Point = nextPoint});
                                        index += 2;
                                    }

                                    //pathFigure.StartPoint = new Point(0,0);
                                    //pathFigure.Segments.Add(new LineSegment() { Point = new Point(500,0)});
                                    //pathFigure.Segments.Add(new LineSegment() { Point = new Point(500, 2500) });
                                    //pathFigure.Segments.Add(new LineSegment() { Point = new Point(0, 2500) });

                                    pG.Figures.Add(pathFigure);
                                    mainControl.Clip = pG;
                                }
                            }

                            outlineControl.Clip = mainControl.Clip;
                        }
                    }
                    else if (tag.Tag.StartsWith("blur"))
                    {
                        var content = LibImp.GetAfter(tag.Tag, "blur");
                        if (content != null)
                        {
                            double val;
                            if (double.TryParse(content, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                                out val))
                            {
                                blur = val;
                            }
                        }
                    }
                    else if (tag.Tag.StartsWith("fs"))
                    {
                        var content = LibImp.GetAfter(tag.Tag, "fs");
                        if (content != null)
                        {
                            int val;
                            if (int.TryParse(content, NumberStyles.None, CultureInfo.InvariantCulture, out val))
                            {
                                fText.SetFontSize(val * scale.Height * fontScale, s, e);
                            }
                        }
                    }
                    else if (tag.Tag.StartsWith("fn"))
                    {
                        var content = LibImp.GetAfter(tag.Tag, "fn");
                        FontFamily fontFamily;
                        if (content == null || !this.fontTypefaces.TryGetValue(content.ToLower(), out fontFamily))
                        {
                            fontFamily = new FontFamily(content);
                        }

                        var typeface = new Typeface(fontFamily,
                            style.Italic ? FontStyles.Italic : FontStyles.Normal,
                            style.Bold ? FontWeights.Bold : FontWeights.Normal,
                            FontStretches.SemiCondensed);

                        fText.SetFontTypeface(typeface, s, e);
                    }
                    else if (tag.Tag == "i1")
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
                    else if (tag.Tag.StartsWith("c&H") || tag.Tag.StartsWith("1c&H"))
                    {
                        var color = ReadAssColor(tag);

                        fText.SetForegroundBrush(
                            new SolidColorBrush(color),
                            s, e);
                    }
                    else if (tag.Tag.StartsWith("3c&H"))
                    {
                        var color = ReadAssColor(tag);
                        outlinePen.Brush = new SolidColorBrush() {Color = color};
                    }
                    else if (tag.Tag.StartsWith("3c&H"))
                    {
                        shadowColor = ReadAssColor(tag);
                    }
                    else if (tag.Tag.StartsWith("alpha"))
                    {
                        mainControl.Opacity = 1 - ReadAssHex(tag) / 255.0;
                        mainControl.DropShadowEffect.Opacity = outlineControl.Opacity = mainControl.Opacity;
                    }
                    else if (tag.Tag.StartsWith("1a"))
                    {
                        mainControl.Opacity = 1 - ReadAssHex(tag) / 255.0;
                    }
                    else if (tag.Tag.StartsWith("2a"))
                    {
                        // Sets the secondary fill alpha. This is only used for pre-highlight in standard karaoke.
                        // Not supported.
                    }
                    else if (tag.Tag.StartsWith("3a"))
                    {
                        outlineControl.Opacity = 1 - ReadAssHex(tag) / 255.0;
                    }
                    else if (tag.Tag.StartsWith("4a"))
                    {
                        // Set Shadow Alpha
                        mainControl.DropShadowEffect.Opacity = 1 - ReadAssHex(tag) / 255.0;
                    }
                    else if (tag.Tag.StartsWith("fr"))
                    {
                        double val;
                        string textval;
                        int rotate;
                        if (tag.Tag.StartsWith("frx"))
                        {
                            textval = LibImp.GetAfter(tag.Tag, "frx");
                            rotate = 1;
                        }
                        else if (tag.Tag.StartsWith("fry"))
                        {
                            textval = LibImp.GetAfter(tag.Tag, "fry");
                            rotate = 2;
                        }
                        else if (tag.Tag.StartsWith("frz"))
                        {
                            textval = LibImp.GetAfter(tag.Tag, "frz");
                            rotate = 3;
                        }
                        else
                        {
                            textval = LibImp.GetAfter(tag.Tag, "fr");
                            rotate = 3;
                        }

                        if (double.TryParse(textval, out val))
                        {
                            if (rotate == 3)
                            {
                                var rotation = new RotateTransform() {Angle = val};
                                mainControl.RenderTransform = rotation;
                                outlineControl.RenderTransform = mainControl.RenderTransform;
                            }
                        }
                    }
                    else if (tag.Tag.StartsWith("org"))
                    {
                        var content = LibImp.GetBetween(tag.Tag, "(", ")");
                        if (content != null)
                        {
                            var parts = content.Split(',');
                            int x, y;
                            if (parts.Length == 2 && int.TryParse(parts[0], out x) && int.TryParse(parts[1], out y))
                            {
                                mainControl.TranslatePoint(new Point(x, y), mainControl);
                                outlineControl.TranslatePoint(new Point(x, y), mainControl);
                            }
                        }
                    }
                }
            }


            var finalPoint = SetStylePosition(pos, style, scale, fText, alignment, leftTopCorner, paragraph);
            shadow = shadow ?? style.ShadowWidth;
            var shadowSet = false;
            //if (shadow >= 0)
            if (!blur.HasValue || outlinePen.Thickness > 0)
            {

                mainControl.DropShadowEffect.Color = shadowColor;
                mainControl.DropShadowEffect.ShadowDepth = shadow.Value * scale.Height;

                mainControl.DropShadowEffect.BlurRadius = scale.Height * 3;

                mainControl.Effect = mainControl.DropShadowEffect;
                shadowSet = true;
            }

            blur = blur ?? (style.BorderStyle == "1" ? 1 : 0);
            if (blur > 0)
            {
                var r = blur.Value * scale.Height * 2;

                // TODO: Implement blur and drop shadow when no border defined
                if (outlinePen.Thickness > 0) //|| shadowSet)
                {
                    outlineControl.BlurEffect.Radius = r;
                    outlineControl.BlurEffect.RenderingBias = RenderingBias.Quality;
                    outlineControl.Effect = outlineControl.BlurEffect;
                }
                else
                {
                    mainControl.BlurEffect.Radius = r;
                    outlineControl.BlurEffect.RenderingBias = RenderingBias.Quality;
                    mainControl.Effect = mainControl.BlurEffect;
                }
            }
            //outlinePen.Thickness *= 2;

            return finalPoint;
        }

        private static Color ReadAssColor(SubtitleTag tag)
        {
            var index = tag.Tag.IndexOf("&H");
            var endIndex = tag.Tag.IndexOf("&", index + 1);

            if (endIndex < index)
            {
                endIndex = tag.Tag.Length;
            }

            var colorHex = "#" + tag.Tag.Substring(index + 2, endIndex - index - 2);
            try
            {
                var color = (Color) ColorConverter.ConvertFromString(colorHex);
                return Color.FromArgb(color.A, color.B, color.G, color.R);
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            return Colors.Black;
        }

        private static int ReadAssHex(SubtitleTag tag)
        {
            var index = tag.Tag.IndexOf("&H");
            var endIndex = tag.Tag.IndexOf("&", index + 1);

            var hex = tag.Tag.Substring(index + 2, endIndex - index - 2);
            
            return Convert.ToInt32(hex, 16);
        }

    }
}
