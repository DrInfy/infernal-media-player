#region Usings

using System;
using System.Windows.Controls;
using System.Windows.Input;
using Imp.Base.Commands;
using Imp.Base.ListLogic;
using Imp.Controls.Gui;
using Imp.Controls.Lists;
using Imp.Player.Controllers;

#endregion

namespace Imp.Player.Panels
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
            styleLib.SetStyle(this.ButtonSettings, BtnNumber.Settings);
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

        private void ButtonSettings_OnClicked(object sender)
        {
            this.ButtonSettings.CurrentState++;

            if (this.ButtonSettings.CurrentState == 1)
            {
                this.mainC.ReadTracks();

                // Update track selection menu
                this.audioTracks.ClearList();
                foreach (var track in this.mainC.AudioTracks)
                {
                    var name = track.lang + " " + track.Title;
                    if (track.IsSelected)
                    {
                        name = "(current) " + name;
                    }

                    var trackId = track.Id;
                    this.audioTracks.AddToList(new ImpTextAndCommand(name, ImpCommand.ChangeAudioTrack, () => trackId));
                }


                // Update track selection menu
                this.subTitleTracks.ClearList();
                this.subTitleTracks.AddToList(new ImpTextAndCommand("No subtitles", ImpCommand.ChangeSubtitles, () => -1));

                foreach (var track in this.mainC.SubtitleTracks)
                {
                    var name = track.lang + " " + track.Title;
                    if (track.IsSelected)
                    {
                        name = "(current) " + name;
                    }

                    var trackId = track.Id;
                    this.subTitleTracks.AddToList(new ImpTextAndCommand(name, ImpCommand.ChangeSubtitles, () => trackId));
                }

                var size = this.subTitleTracks.DesiredSize();
                var size2 = this.audioTracks.DesiredSize();

                this.TrackSelectionMenu.Height = Math.Max(size.Height, size2.Height);

                this.TrackSelectionMenu.IsOpen = true;
                this.TrackSelectionMenu.ReleaseMouseCapture();
            }
            else
            {
                this.TrackSelectionMenu.IsOpen = false;
            }
        }

        private void AudioTracks_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetTrack((ContextMenuList)sender);
        }

        private void AudioTracks_OnTouchTap(object sender, TouchEventArgs e)
        {
            SetTrack((ContextMenuList)sender);
        }

        private void SubTitleTracks_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetTrack((ContextMenuList)sender);
        }

        private void SubTitleTracks_OnTouchTap(object sender, TouchEventArgs e)
        {
            SetTrack((ContextMenuList) sender);
        }

        private void SetTrack(ContextMenuList sender)
        {
            CloseMenu();

            var item = sender.GetSelected();

            if (item != null)
            {
                this.mainC.Exec(item.Command, item.Argument);
            }
        }

        public void CloseMenu()
        {
            this.ButtonSettings.CurrentState = 0;
            this.TrackSelectionMenu.IsOpen = this.ButtonSettings.CurrentState == 1;
        }

        private void AudioTracks_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ((ContextMenuList) sender).SelectNone();
        }

        private void MediaPlayerBottom_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var pos = (e.GetPosition(this));
            bool isOutside = pos.X < 0 || pos.Y < 0 || pos.Y > this.ActualHeight || pos.X > this.ActualWidth;

            if (isOutside && !this.TrackSelectionMenu.IsMouseOver)
            {
                CloseMenu();
            }
        }
    }
}