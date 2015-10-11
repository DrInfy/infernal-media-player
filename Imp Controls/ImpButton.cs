#region Usings

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Base.Interfaces;

#endregion

namespace ImpControls
{
    public class ImpButton : ImpBaseControl, IStateButton
    {
        #region Fields

        protected object[] sContent;
        protected int sCheckStates = 1;
        protected int sCurrentState = 0;

        #endregion

        #region Properties

        public int? GeometryMargin { get; set; } = null;

        public virtual int CheckStates
        {
            get { return sCheckStates; }
            set
            {
                if (value > 0 & value != CheckStates)
                {
                    sCheckStates = value;
                    CurrentState = CurrentState;
                    sContent = new object[sCheckStates];
                }
            }
        }

        public int CurrentState
        {
            get { return sCurrentState; }
            set
            {
                var temp = sCurrentState;
                if (sCurrentState != value)
                {
                    sCurrentState = value;
                }
                if (sCurrentState > CheckStates - 1)
                    sCurrentState = 0;
                if (sCurrentState < 0)
                    sCurrentState = CheckStates - 1;
                if (sCurrentState != temp)
                {
                    InvalidateVisual();
                }
            }
        }

        #endregion

        public ImpButton()
        {
            sContent = new object[sCheckStates];
        }

        public void SetContent(object content, int index = 0)
        {
            if (index >= 0 & index < CheckStates)
            {
                sContent[index] = content;
            }
        }

        protected override void DrawContent(DrawingContext drawingContext)
        {
            int margin;
            if (GeometryMargin.HasValue)
            {
                margin = GeometryMargin.Value;
            }
            else
            {
                margin = (int) ((ActualHeight - 5) / 5);
            }
            // Margin
            const double bottom = 100;
            const double right = 100;

            double x = 0;
            double y = 0;
            var content = GetCurrentContent();

            x = SetupTranslation(x, ref y);


            if (content == null | ActualHeight < 1 | ActualWidth < 1)
                return;

            if (content is List<BitmapSource>)
            {
                BitmapSource bitmap;
                var bitmapSources = (List<BitmapSource>) content;
                //= GetCurrentContent()
                if (Pressed & MouseOver)
                    bitmap = bitmapSources[2];
                else if (Pressed | MouseOver)
                    bitmap = bitmapSources[1];
                else
                    bitmap = bitmapSources[0];

                // adjust properly to pixel perfection
                x += 0.5;
                y += 0.5;
                var r = new Rect(Math.Floor(ActualWidth / 2 - bitmap.PixelWidth / 2) + x,
                    Math.Floor(ActualHeight / 2 - bitmap.PixelHeight / 2) + y,
                    bitmap.PixelWidth,
                    bitmap.PixelHeight);


                drawingContext.DrawImage(bitmap, r);
            }
            else if (content is PathGeometry)
            {
                var geometry = (Geometry) content;


                if (right / ActualWidth > bottom / ActualHeight)
                {
                    geometry.Transform = new MatrixTransform((ActualWidth - margin * 2) / right,
                        0,
                        0,
                        (ActualWidth - margin * 2) / right, margin + x,
                        (ActualHeight - bottom * (ActualWidth - margin * 2) / right) / 2 + y);
                }
                else
                {
                    geometry.Transform = new MatrixTransform((ActualHeight - margin * 2) / bottom,
                        0,
                        0,
                        (ActualHeight - margin * 2) / bottom,
                        (ActualWidth - right * (ActualHeight - margin * 2) / bottom) / 2 + x,
                        margin + y);
                }
                drawingContext.DrawGeometry(null, new Pen(renderData.FrontBrush, 1), geometry);
            }
            else if (content is string)
            {
                var text = new FormattedText((string) content, System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, sStyle.FontFace, sStyle.DefaultFontSize, renderData.FrontBrush);

                text.MaxTextWidth = Math.Max(0, ActualWidth - 6);
                //text.MaxTextHeight = Me.ActualHeight
                text.MaxLineCount = 1;
                text.TextAlignment = TextAlignment.Center;
                text.Trimming = TextTrimming.CharacterEllipsis;
                drawingContext.DrawText(text, new Point(3 + x, ActualHeight / 2 - 8 + y));
            }
        }

        protected virtual double SetupTranslation(double x, ref double y)
        {
            if (Pressed && MouseOver)
            {
                x = sStyle.PressedTranslation.X;
                y = sStyle.PressedTranslation.Y;
            }
            return x;
        }

        protected virtual object GetCurrentContent()
        {
            return sContent[CurrentState];
        }
    }
}