﻿#region Usings

using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace Imp.Controls.Gui
{
    public class StyleLib : IStyleLib
    {
        #region Static Fields and Constants

        public const int WINDOWED_MARGIN = 6;
        public const int PANWIDTH = 5;
        public const int SNAPTOWIDTH = 15;
        public const int MINPANWIDTH = 60;
        public const int MINFONTSIZE = 9;
        public const int WIDTH_TO_RESIZE = 800;
        public const int WIDTH_CRITICAL = 400;

        #endregion

        #region Fields

        private StyleClass BaseStyle;
        private StyleClass ButtonStyle;
        //private StyleClass BarStyle;

        private StyleClass ListStyle;

        #endregion

        public void LoadStyles()
        {
            BaseStyle = new StyleClass();
            SetDefaultButtonStyle();
            //ListStyle = BaseStyle.Copy();
            //BaseStyle.NormalBrush = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            //BaseStyle.MouseoverBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            BaseStyle.PressedBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BaseStyle.BackPressedBrush = new SolidColorBrush(Color.FromRgb(100, 0, 0));
            BaseStyle.MouseoverBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            //ListStyle.PressedBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        public Brush GetGridBrush(bool bar)
        {
            if (bar)
                return ButtonStyle.BackNormalBrush;
            return BaseStyle.BackNormalBrush;
        }

        public Brush GetForeground()
        {
            return BaseStyle.NormalBrush;
        }

        public void SetStyle(ImpBaseControl nonButton)
        {
            nonButton.SetStyle(BaseStyle);
        }

        public void SetStyle(ImpButton button, string content)
        {
            button.SetContent(content);
            button.SetStyle(ButtonStyle);
        }

        public void SetStyle(ImpButton button, BtnNumber btnNumber)
        {
            for (var i = 0; i < button.CheckStates; i++)
            {
                button.SetContent(GeometryCreator.GetGeometry(btnNumber, i), i);
            }

            button.SetStyle(ButtonStyle);
        }

        public void SetStyle(TextBox textBox)
        {
            textBox.Background = BaseStyle.PanelUpperBrush;
            textBox.BorderBrush = BaseStyle.BorderBrush;
            textBox.Foreground = BaseStyle.NormalBrush;
            textBox.FontFamily = BaseStyle.DefaultFont;
        }

        private void SetDefaultButtonStyle()
        {
            var brushT = new LinearGradientBrush();
            ButtonStyle = new StyleClass();
            ButtonStyle.BackNormalBrush = BaseStyle.PanelUpperBrush;

            brushT = new LinearGradientBrush();
            brushT.StartPoint = new System.Windows.Point(0, 0);
            brushT.EndPoint = new System.Windows.Point(0, 1);
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0));
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(200, 0, 0, 0), 0.75));
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1));


            ButtonStyle.BorderBrush = brushT;

            brushT = new LinearGradientBrush();
            brushT.StartPoint = new System.Windows.Point(0, 0);
            brushT.EndPoint = new System.Windows.Point(0, 1);
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0));
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(200, 0, 0, 0), 0.75));
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1));

            ButtonStyle.BorderMouseoverBrush = brushT;
            brushT = new LinearGradientBrush();
            brushT.StartPoint = new System.Windows.Point(0, 0);
            brushT.EndPoint = new System.Windows.Point(0, 1);
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0));
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(100, 0, 0, 0), 0.85));
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1));

            ButtonStyle.BorderPressedBrush = brushT;

            brushT = new LinearGradientBrush();
            brushT.StartPoint = new System.Windows.Point(0, 0);
            brushT.EndPoint = new System.Windows.Point(0, 1);
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0));
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(200, 0, 0, 0), 0.85));
            brushT.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1));

            ButtonStyle.BorderDisabledBrush = brushT;


            brushT = new LinearGradientBrush();
            brushT.StartPoint = new System.Windows.Point(0, 0);
            brushT.EndPoint = new System.Windows.Point(0, 1);
            brushT.GradientStops.Add(new GradientStop(Color.FromRgb(65, 65, 65), 0));
            brushT.GradientStops.Add(new GradientStop(Color.FromRgb(150, 150, 150), 0.75));
            brushT.GradientStops.Add(new GradientStop(Color.FromRgb(65, 65, 65), 1));

            ButtonStyle.BackMouseoverBrush = brushT;

            brushT = new LinearGradientBrush();
            brushT.StartPoint = new System.Windows.Point(0, 0);
            brushT.EndPoint = new System.Windows.Point(0, 1);
            brushT.GradientStops.Add(new GradientStop(Color.FromRgb(33, 33, 33), 0));
            brushT.GradientStops.Add(new GradientStop(Color.FromRgb(75, 75, 75), 0.85));
            brushT.GradientStops.Add(new GradientStop(Color.FromRgb(33, 33, 33), 1));

            ButtonStyle.BackPressedBrush = brushT;

            ButtonStyle.BaseBorderThickness.Bottom = 0;
            ButtonStyle.BaseBorderThickness.Top = 0;

            ButtonStyle.PressedTranslation = new System.Windows.Point(0, 2);


            brushT = new LinearGradientBrush();
            brushT.StartPoint = new System.Windows.Point(0, 0);
            brushT.EndPoint = new System.Windows.Point(0, 1);
            brushT.GradientStops.Add(new GradientStop(Color.FromRgb(30, 30, 30), 0));
            brushT.GradientStops.Add(new GradientStop(Color.FromRgb(70, 70, 70), 0.85));
            brushT.GradientStops.Add(new GradientStop(Color.FromRgb(30, 30, 30), 1));

            ButtonStyle.BackDisabledBrush = brushT;
        }

        public void SetStyle(ImpButton button, params string[] content)
        {
            button.CheckStates = content.Length;
            for (var i = 0; i < content.Length; i++)
            {
                button.SetContent(content[i], i);
            }
            button.SetStyle(ButtonStyle);
        }

        public void SetStyle(Label label, bool background = true)
        {
            if (background) label.Background = GetGridBrush(true);
            label.Foreground = GetForeground();
            label.FontFamily = BaseStyle.DefaultFont;
        }
    }
}