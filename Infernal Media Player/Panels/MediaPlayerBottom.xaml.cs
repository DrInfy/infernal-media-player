using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Base.Commands;
using Imp.Controllers;
using ImpControls.Gui;
using Ipv;

namespace Imp.Panels
{
    /// <summary>
    /// Interaction logic for MediaPlayerBottom.xaml
    /// </summary>
    public partial class MediaPlayerBottom : UserControl
    {
        private MainController mainC;


        public MediaPlayerBottom()
        {
            InitializeComponent();
        }


        public void SetStyles(StyleLib styleLib, MainController mainController)
        {
            mainC = mainController;

            styleLib.SetStyle(ButtonPlay, BtnNumber.Play);
            styleLib.SetStyle(ButtonLoop, BtnNumber.Loop);
            styleLib.SetStyle(ButtonMute, BtnNumber.Mute);

            LabelPosition.Foreground = styleLib.GetForeground();
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
    }
}
