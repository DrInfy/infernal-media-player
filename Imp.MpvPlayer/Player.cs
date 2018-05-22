#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Documents;
using Imp.Base.Interfaces;
using Imp.MpvPlayer.Containers;
using Mpv.WPF;
using Newtonsoft.Json;

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
                    this.controller.SetTargetPosition(this.player, TimeSpan.FromSeconds(value));
                }
            } 
           
        }

        public double Volume
        {
            get => 0; // TODO: Imp Fader
            set => this.player.Volume = (int)(value * 100);
        }

        public bool HasVideo => this.player.IsMediaLoaded;

        public List<BaseTrack> Tracks { get; set; } = new List<BaseTrack>();
        public List<BaseTrack> AudioTracks { get; set; } = new List<BaseTrack>();
        public List<BaseTrack> SubtitleTracks { get; set; } = new List<BaseTrack>();

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
            Close();
        }

        public void Close()
        {
            Tracks = new List<BaseTrack>();
            this.player.Stop();
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
            this.player.MediaEndedSeeking += Player_MediaEndedSeeking;
        }

        private void Player_MediaEndedSeeking(object sender, EventArgs e)
        {
            this.controller.ClearTarget();
        }

        private void PlayerOnMediaLoaded(object sender, EventArgs e)
        {
            this.controller.IsMediaLoaded = true;

            this.controller.Duration = this.player.Duration;
            this.player.API.SetPropertyString("hr-seek", "yes");
            this.player.API.SetPropertyString("hr-seek-framedrop", "yes");
            //this.player.API.SetPropertyDouble("hr-seek-demuxer-offset", -1);
            
            
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

        public void NoSubtitle()
        {
            this.player.API.SetPropertyString("sid", "no");
        }

        public void AutoSubtitle()
        {
            this.player.API.SetPropertyString("sid", "auto");
        }

        public void SetSubtitle(int srcId)
        {
            this.player.API.SetPropertyString("sid", srcId.ToString());
        }

        /// <summary>
        /// Rotates subtitle tracks and no subtitle
        /// </summary>
        public int NextSubtitle()
        {
            var id = 0;
            ReadTracks();

            var selected = this.SubtitleTracks.FirstOrDefault(x => x.IsSelected);

            if (selected != null)
            {
                var nextTrack = this.SubtitleTracks.Where(x => x.Id > selected.Id).OrderBy(x => x.Id).FirstOrDefault();

                if (nextTrack != null)
                {
                    id = nextTrack.Id;
                }
                else
                {
                    id = 0;
                }
            }
            else
            {
                id = 1;
            }

            if (id == 0)
            {
                NoSubtitle();
            }
            else
            {
                this.player.API.SetPropertyString("sid", id.ToString());
            }

            return id;
        }

        public void SetAudioTrack(int srcId)
        {
            this.player.API.SetPropertyString("aid", srcId.ToString());
        }

        /// <summary>
        /// Rotates audio tracks
        /// </summary>
        public int NextAudioTrack()
        {
            var id = 0;
            ReadTracks();

            var selected = this.AudioTracks.FirstOrDefault(x => x.IsSelected);

            if (selected != null)
            {
                var nextTrack = this.AudioTracks.Where(x => x.Id > selected.Id).OrderBy(x => x.Id).FirstOrDefault();

                if (nextTrack != null)
                {
                    id = nextTrack.Id;
                }
                else
                {
                    id = 1;
                }
            }
            else
            {
                id = 1;
            }

            this.player.API.SetPropertyString("aid", id.ToString());

            return id;
        }

        private void ReadTracks()
        {
            var tracksJson = this.player.API.GetPropertyString("track-list");
            if (tracksJson != null)
            {
                this.Tracks = JsonConvert.DeserializeObject<List<BaseTrack>>(tracksJson);
            }

            this.AudioTracks = this.Tracks.Where(x => x.Type == "audio").ToList();
            this.SubtitleTracks = this.Tracks.Where(x => x.Type == "sub").ToList();
        }

        #endregion


    }
}