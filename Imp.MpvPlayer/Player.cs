#region Usings

using System;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Documents;
using Imp.Base.Interfaces;
using Mpv.WPF;

#endregion

namespace Imp.MpvPlayer
{
    public class Player : Grid, IMediaUriPlayer
    {
        public event Action MediaPlayerEnded;

        public Player()
        {
            Init();
        }

        #region  Public Fields and Properties

        public bool IsPlaying => this.player.IsMediaLoaded;
        public double Duration => this.controller.Duration.TotalSeconds;

        public double Position
        {
            get => this.controller.Position.TotalSeconds;
            // TODO: Imp Fader

            set
            {
                if (this.player.IsMediaLoaded)
                {
                    this.player.Position = TimeSpan.FromSeconds(value);
                }
            } 
           
        }

        public double Volume
        {
            get => 0; // TODO: Imp Fader
            set => this.player.Volume = (int)(value * 100);
        }

        public bool HasVideo => this.player.IsMediaLoaded;

        #endregion

        #region Local Fields

        private PlayerController controller = new PlayerController();

        internal Mpv.WPF.MpvPlayer player;

        #endregion

        #region Common

        public void Play()
        {
            this.player.Resume();
        }

        public void Pause()
        {
            this.player.Pause();
        }

        public void Stop()
        {
            this.player.Position = TimeSpan.Zero;
        }

        public void Clear()
        {
            this.player.Stop();
            //this.player.Load(null);
        }

        public void Init()
        {
            this.player = new Mpv.WPF.MpvPlayer("mpv-1.dll")
            {
                AutoPlay = true
            };
            this.Children.Add(this.player);

            this.player.KeepOpen = KeepOpen.Always;
            this.player.MediaLoaded += PlayerOnMediaLoaded;
            this.player.MediaUnloaded += PlayerOnMediaUnloaded;
            this.player.PositionChanged += PlayerOnPositionChanged;
        }
        
        private void PlayerOnMediaLoaded(object sender, EventArgs e)
        {
            this.controller.IsMediaLoaded = true;

            this.controller.Duration = this.player.Duration;
        }

        private void PlayerOnMediaUnloaded(object sender, EventArgs e)
        {
            this.controller.IsMediaLoaded = false;
            this.controller.Position = TimeSpan.Zero;
        }

        private void PlayerOnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            this.controller.Position = TimeSpan.FromSeconds(e.Position);

            if (this.IsPlaying && this.controller.Position == this.controller.Duration)
            {
                this.Dispatcher.Invoke(() => this.MediaPlayerEnded?.Invoke());
            }
        }

        #endregion

        public void Close()
        {
            //throw new NotImplementedException();
        }
    }
}