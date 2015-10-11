#region Usings

using System;
using System.Windows;
using System.Windows.Media;

#endregion

namespace ImpControls
{
    /// <summary>
    /// Specifies styling for a control
    /// </summary>
    [Serializable]
    public class StyleClass
    {
        #region Static Fields and Constants

        /// <summary> The scrollbar width</summary>
        public int ScrollbarWidth = 10;

        #endregion

        #region Fields

        /// <summary> height of one single row in listboxes in pixels</summary>
        public int RowHeight = 18;

        public Brush BorderBrush;
        public Brush BorderMouseoverBrush;
        public Brush BorderPressedBrush;
        public Brush BorderDisabledBrush;
        public Brush BackNormalBrush;
        public Brush BackMouseoverBrush;
        public Brush BackPressedBrush;
        public Brush BackDisabledBrush;
        public Brush NormalBrush;
        public Brush MouseoverBrush;
        public Brush PressedBrush;
        public Brush DisabledBrush;
        public Brush PanelUpperBrush;
        public Brush PanelLowerBrush;
        public Thickness BaseBorderThickness;
        public Point PressedTranslation = new Point(0, 0);
        public Typeface FontFace = new Typeface("Segoe UI");
        public FontFamily DefaultFont = new FontFamily("Segoe UI");

        public int DefaultFontSize = 12;

        #endregion

        public StyleClass()
        {
            BorderBrush = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            BorderMouseoverBrush = new SolidColorBrush(Color.FromArgb(100, 20, 20, 20));
            BorderPressedBrush = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40));
            BorderDisabledBrush = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));

            BackNormalBrush = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            BackMouseoverBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            BackPressedBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100));
            BackDisabledBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            NormalBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            MouseoverBrush = new SolidColorBrush(Color.FromRgb(170, 0, 0));
            PressedBrush = new SolidColorBrush(Color.FromRgb(200, 0, 0));
            DisabledBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            var brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(0, 1);
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(50, 50, 50), 0));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(120, 120, 120), 0.75));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(50, 50, 50), 1));

            PanelUpperBrush = brush;

            PanelLowerBrush = brush;

            BaseBorderThickness = new Thickness(1);
        }

        public StyleClass Copy()
        {
            return (StyleClass) MemberwiseClone();
        }
    }
}