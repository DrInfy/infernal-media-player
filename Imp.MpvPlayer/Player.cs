#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Imp.Base.Interfaces;
using Imp.MpvPlayer.Containers;
using Mpv.NET;
using Newtonsoft.Json;
using SEdge;

#endregion

namespace Imp.MpvPlayer
{
    public class Player : UserControl, IMediaUriPlayer
    {
        #region Static Fields and Constants

        private const int timePosUserData = 10;

        #endregion

        #region  Public Fields and Properties

        public bool IsPlaying { get; private set; }

        public double Duration { get; private set; }

        public double Position
        {
            get
            {
                if (this.IsPlaying)
                {
                    if (this.seekTarget.HasValue)
                    {
                        return this.seekTarget.Value;
                    }

                    return this.cachedPosition;
                }

                return 0;
            }
            set
            {
                if (this.IsPlaying)
                {
                    var totalSecondsString = value.ToString(CultureInfo.InvariantCulture);

                    lock (this.mpv)
                    {
                        this.mpv.Command("seek", totalSecondsString, "absolute+exact");
                    }

                    this.seekTarget = value;
                }
            }
        }

        public double Volume
        {
            get
            {
                lock (this.mpv)
                {
                    return this.mpv.GetPropertyDouble("volume") * 0.01;
                }
            }
            set
            {
                lock (this.mpv)
                {
                    this.mpv.SetPropertyDouble("volume", value * 100);
                }
            }
        }

        public bool HasVideo { get; private set; }

        public List<BaseTrack> Tracks { get; set; } = new List<BaseTrack>();
        public List<BaseTrack> VideoTracks { get; set; } = new List<BaseTrack>();
        public List<BaseTrack> AudioTracks { get; set; } = new List<BaseTrack>();
        public List<BaseTrack> SubtitleTracks { get; set; } = new List<BaseTrack>();

        #endregion

        #region Local Fields

        private Mpv.NET.Mpv mpv;
        private double? seekTarget;
        private double cachedPosition;

        private MpvPlayerHwndHost playerHwndHost;

        #endregion

        #region Common

        public Player()
        {
            Init();
        }

        public void Play()
        {
            lock (this.mpv)
            {
                this.mpv.SetPropertyString("pause", "no");
            }
        }

        public void Pause()
        {
            lock (this.mpv)
            {
                this.mpv.SetPropertyString("pause", "yes");
            }
        }

        public void Stop()
        {
            Pause();
            this.Position = 0;
        }

        public void Clear()
        {
            Close();
        }

        public event Action MediaPlayerEnded;

        public LoadingResult StartLoad([NotNull] string path)
        {
            LoadingResult result = new LoadingResult();

            lock (this.mpv)
            {
                this.mpv.FileLoaded += result.MpvOnFileLoaded;
                this.mpv.EndFile += result.MpvOnEndFile;
                this.mpv.SetPropertyString("pause", "yes");
                this.mpv.Command("loadfile", path, "replace");
            }

            return result;
        }

        public void FinalizeLoad(LoadingResult result)
        {
            if (result.Success)
            {
                this.IsPlaying = true;
                ReadTracks();

                lock (this.mpv)
                {
                    var durationSeconds = this.mpv.GetPropertyDouble("duration");
                    this.Duration = durationSeconds;
                    this.mpv.FileLoaded -= result.MpvOnFileLoaded;
                    this.mpv.EndFile -= result.MpvOnEndFile;

                    this.HasVideo = this.VideoTracks.Any();
                }
            }
            else
            {
                this.HasVideo = false;
            }
        }

        public void Close()
        {
            Stop();
            this.Tracks.Clear();
        }

        public void Init()
        {
            this.mpv = new Mpv.NET.Mpv("lib\\mpv-1.dll");

            SetMpvHost();

            this.Background = Brushes.Black;

            this.mpv.SetPropertyString("keep-open", "always");

            this.mpv.PlaybackRestart += MpvOnPlaybackRestart;
            this.mpv.Seek += MpvOnSeek;

            this.mpv.EndFile += MpvOnEndFile;

            this.mpv.PropertyChange += MpvOnPropertyChange;

            this.mpv.ObserveProperty("time-pos", MpvFormat.Double, timePosUserData);
        }

        private void MpvOnSeek(object sender, EventArgs e)
        {
            // TODO: Confirm: do nothing?
        }

        private void MpvOnPlaybackRestart(object sender, EventArgs e)
        {
            // Started playing again, i.e. no longer seeking.
            this.seekTarget = null;
        }

        private void MpvOnPropertyChange(object sender, MpvPropertyChangeEventArgs e)
        {
            var eventProperty = e.EventProperty;

            switch (e.ReplyUserData)
            {
                case timePosUserData:
                    var newPosition = PointerReader.ReadDouble(eventProperty.Data);

                    PlayerOnPositionChanged(newPosition);
                    break;
            }
        }

        private void MpvOnEndFile(object sender, MpvEndFileEventArgs e)
        {
            bool shouldInvoke = !this.IsPlaying;

            // File died
            this.IsPlaying = false;
            this.Position = 0;

            if (shouldInvoke)
            {
                this.Dispatcher.Invoke(() => MediaPlayerEnded?.Invoke());
            }
        }

        private void SetMpvHost()
        {
            // Create the HwndHost and add it to the user control.
            this.playerHwndHost = new MpvPlayerHwndHost(this.mpv);
            AddChild(this.playerHwndHost);
        }

        private void PlayerOnPositionChanged(double position)
        {
            if (this.IsPlaying)
            {
                this.cachedPosition = position;
                string eof;

                lock (this.mpv)
                {
                    try
                    {
                        eof = this.mpv.GetPropertyString("eof-reached");
                    }
                    catch
                    {
                        eof = null;
                    }
                }

                if (eof == "yes")
                {
                    this.Dispatcher.Invoke(() => MediaPlayerEnded?.Invoke());
                }
            }
            else
            {
                this.cachedPosition = 0;
            }
        }

        public void NoSubtitle()
        {
            lock (this.mpv)
            {
                this.mpv.SetPropertyString("sid", "no");
            }
        }

        public void AutoSubtitle()
        {
            lock (this.mpv)
            {
                this.mpv.SetPropertyString("sid", "auto");
            }
        }

        public void SetSubtitle(int srcId)
        {
            lock (this.mpv)
            {
                this.mpv.SetPropertyString("sid", srcId.ToString());
            }
        }

        /// <summary>
        ///     Rotates subtitle tracks and no subtitle
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
                lock (this.mpv)
                {
                    this.mpv.SetPropertyString("sid", id.ToString());
                }
            }

            return id;
        }

        public void SetAudioTrack(int srcId)
        {
            lock (this.mpv)
            {
                this.mpv.SetPropertyString("aid", srcId.ToString());
            }
        }

        /// <summary>
        ///     Rotates audio tracks
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

            lock (this.mpv)
            {
                this.mpv.SetPropertyString("aid", id.ToString());
            }

            return id;
        }

        private void ReadTracks()
        {
            string tracksJson;

            lock (this.mpv)
            {
                tracksJson = this.mpv.GetPropertyString("track-list");
            }

            if (tracksJson != null)
            {
                this.Tracks = JsonConvert.DeserializeObject<List<BaseTrack>>(tracksJson);
            }

            this.VideoTracks = this.Tracks.Where(x => x.Type == "video").ToList();
            this.AudioTracks = this.Tracks.Where(x => x.Type == "audio").ToList();
            this.SubtitleTracks = this.Tracks.Where(x => x.Type == "sub").ToList();
        }

        #endregion
    }
}