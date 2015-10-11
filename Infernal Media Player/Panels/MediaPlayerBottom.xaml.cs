#region Usings

using System.Windows.Controls;
using System.Windows.Input;
using Base.Commands;
using Imp.Controllers;
using ImpControls.Gui;

#endregion

namespace Imp.Panels
{
    /// <summary>
    /// Interaction logic for MediaPlayerBottom.xaml
    /// </summary>
    public partial class MediaPlayerBottom : UserControl
    {
        #region Fields

        private MainController mainC;

        #endregion

        public MediaPlayerBottom()
        {
            InitializeComponent();
        }

        public void SetStyles(StyleLib styleLib, MainController mainController)
        {
            mainC = mainController;

            styleLib.SetStyle(ButtonPlay, BtnNumber.Play);
            styleLib.SetStyle(ButtonLoop, BtnNumber.Loop);
            styleLib.SetStyle(ButtonNext, BtnNumber.Next);
            styleLib.SetStyle(ButtonMute, BtnNumber.Mute);
            styleLib.SetStyle(LabelPosition, false);
        }

        private void ButtonPlay_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.Playpause);
        }

        private void ButtonLoop_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.LoopChange);
        }

        private void sliderTime_MouseMove(object sender, MouseEventArgs e)
        {
            if (SliderTime.IsMouseCaptured)
                mainC.Exec(ImpCommand.SetPosition,
                    e.GetPosition(SliderTime).X / SliderTime.RenderSize.Width * SliderTime.Maximum);
        }

        private void sliderTime_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                SliderTime.ReleaseMouseCapture();
        }

        private void sliderTime_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                SliderTime.CaptureMouse();
        }

        private void sliderVolume_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                SliderVolume.CaptureMouse();
        }

        private void sliderVolume_MouseMove(object sender, MouseEventArgs e)
        {
            if (SliderVolume.IsMouseCaptured)
                mainC.Exec(ImpCommand.SetVolume,
                    e.GetPosition(SliderVolume).X / SliderVolume.RenderSize.Width * SliderVolume.Maximum);
        }

        private void sliderVolume_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                SliderVolume.ReleaseMouseCapture();
        }

        private void ButtonMute_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.VolumeMute);
        }

        private void ButtonNext_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.OpenNext);
        }
    }
}