﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Base.Interfaces;

namespace ImpControls
{

    public class ImpButton : ImpBaseControl, IStateButton
    {
        protected object[] sContent;
        protected int sCheckStates = 1;

        protected int sCurrentState = 0;

        public ImpButton()
        {
            sContent = new object[sCheckStates];
        }

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
                int temp = sCurrentState;
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
                    this.InvalidateVisual();
                }
            }
        }

        public void SetContent(object content, int index = 0)
        {
            if (index >= 0 & index < CheckStates)
            {
                this.sContent[index] = content;
            }
        }


        protected override void DrawContent(DrawingContext drawingContext)
        {
            int Margin = (int) ((ActualHeight - 5) / 5);
            // Margin
            const double bottom = 100;
            const double right = 100;

            double x = 0;
            double y = 0;
            var content = GetCurrentContent();
 
            x = SetupTranslation(x, ref y);


            if (content == null | this.ActualHeight < 1 | this.ActualWidth < 1)
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
                var r = new Rect(Math.Floor(this.ActualWidth / 2 - bitmap.PixelWidth / 2) + x,
                    Math.Floor(this.ActualHeight / 2 - bitmap.PixelHeight / 2) + y, 
                    bitmap.PixelWidth, 
                    bitmap.PixelHeight);


                drawingContext.DrawImage(bitmap, r);

            }
            else if (content is PathGeometry)
            {
                var geometry = (Geometry)content;


                if (right / this.ActualWidth > bottom / this.ActualHeight) 
                {
                    geometry.Transform = new MatrixTransform((this.ActualWidth - Margin * 2) / right, 
                        0,
                        0,
                        (this.ActualWidth - Margin * 2) / right, Margin + x, 
                        (this.ActualHeight - bottom * (this.ActualWidth - Margin * 2) / right) / 2 + y);
                } 
                else 
                {
                    geometry.Transform = new MatrixTransform((this.ActualHeight - Margin * 2) / bottom, 
                        0,
                        0, 
                        (this.ActualHeight - Margin * 2) / bottom,
                        (this.ActualWidth - right * (this.ActualHeight - Margin * 2) / bottom) / 2 + x, 
                        Margin + y);
                }
                drawingContext.DrawGeometry(null, new System.Windows.Media.Pen(renderData.FrontBrush, 1), geometry);
            } 
            else if (content is string) {

                FormattedText text = new FormattedText((string)content, System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, new Typeface("Arial"), 12, renderData.FrontBrush);

                text.MaxTextWidth = this.ActualWidth - 6;
                //text.MaxTextHeight = Me.ActualHeight
                text.MaxLineCount = 1;
                text.TextAlignment = TextAlignment.Center;
                text.Trimming = TextTrimming.CharacterEllipsis;
                drawingContext.DrawText(text, new System.Windows.Point(3 + x, this.ActualHeight / 2 - 6 + y));
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