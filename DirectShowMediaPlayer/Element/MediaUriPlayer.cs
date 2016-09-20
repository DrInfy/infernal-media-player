using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Imp.Base.Interfaces;
using Imp.Base.Libraries;
using Imp.DirectShow.Player;

namespace Imp.DirectShow.Element
{
    public class MediaUriPlayer: MediaRenderer, IMediaUriPlayer
    {
        private MediaRenderer renderer ;


        #region Fields

        private List<int> lastSubtitleIndices = new List<int>();
        private List<int> nextSubtitleIndices = new List<int>();
        private PlayerController controller;
        private double volume = 1;

        #endregion

        #region Properties

        public PlayerController Controller
        {
            get { return controller; }
            set
            {
                if (controller != null)
                {
                    if (controller.Equals(value))
                        return;
                    controller.Command(MediaCommand.Close);

                    controller.NewAllocatorFrame -= this.renderer.OnMediaPlayerNewAllocatorFramePrivate;
                    controller.NewAllocatorSurface -= this.renderer.OnMediaPlayerNewAllocatorSurfacePrivate;
                    controller = null;
                }
                controller = value;
                InitializeMediaPlayer();
            }
        }

        public SubtitleElement SubtitleElement { get; set; }
        public bool IsPlaying => controller != null;
        public double Duration => controller != null ? LibImp.TicksToSeconds(controller.Duration) : 0;

        public double Position
        {
            get { return controller != null ? LibImp.TicksToSeconds(controller.Position) : 0; }
            set { controller?.MoveTo(LibImp.SecondsToTicks(value)); }
        }

        public double Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                if (controller != null)
                    controller.Volume = value;
            }
        }

        public bool HasVideo => controller != null && controller.HasVideo;

        #endregion

        #region Events

        public event Action MediaPlayerEnded;

        #endregion

        public MediaUriPlayer()
        {
            this.renderer = this;
            //this.renderer = new MediaRenderer()
            //{
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    Margin = new Thickness(0),
            //    Visibility = Visibility.Visible
            //};
            //<element:MediaUriPlayer x:Name="UriPlayer" Grid.Column="2" Grid.Row="1" HorizontalAlignment="Center"  Margin="0,0,0,0" VerticalAlignment="Center" MouseDown="UriPlayer_MouseDown" MediaPlayerEnded="UriPlayer_MediaPlayerEnded" />
            //this.subtitleElement = new SubtitleElement()
            //{
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    Margin = new Thickness(0),
            //    Visibility = Visibility.Visible
            //};

            //this.Children.Add(this.renderer);
            //this.Children.Add(this.subtitleElement);
        }

        public void Play()
        {
            controller?.Command(MediaCommand.Play);
        }

        public void Pause()
        {
            controller?.Command(MediaCommand.Pause);
        }

        public void Stop()
        {
            controller?.Command(MediaCommand.Stop);
        }

        public void Clear()
        {
            controller?.Command(MediaCommand.Close);
            controller = null;
        }

        protected virtual void InitializeMediaPlayer()
        {
            this.SubtitleElement.Clear();
            this.SubtitleElement.Visibility = this.controller.SelectedSubtitleTrack != null ? Visibility.Visible : Visibility.Hidden;
            this.SubtitleElement.CopyFonts(this.controller.Fonts);
            this.SubtitleElement.ImageWidthFunc = () => this.VideoImage?.ActualWidth ?? 0;
            this.SubtitleElement.ImageHeightFunc = () => this.VideoImage?.ActualHeight ?? 0;

            /* Hook into the normal .NET events */
            controller.MediaEnded += OnMediaPlayerEnded;

            /* These events fire when we get new D3Dsurfaces or frames */
            controller.NewAllocatorFrame += Controller_NewAllocatorFrame;
            controller.NewAllocatorFrame += this.renderer.OnMediaPlayerNewAllocatorFramePrivate;
            controller.NewAllocatorSurface += this.renderer.OnMediaPlayerNewAllocatorSurfacePrivate;

            controller.Volume = Volume;
            controller.Activate();
        }

        private void Controller_NewAllocatorFrame()
        {
            var position = this.Position;

            if (this.controller.SelectedSubtitleTrack != null)
            {

                /* Ensure we run on the correct Dispatcher */
                

                this.nextSubtitleIndices.Clear();
                for (int i = 0; i < this.controller.SelectedSubtitleTrack.Subtitles.Paragraphs.Count; i++)
                {
                    var p = this.controller.SelectedSubtitleTrack.Subtitles.Paragraphs[i].Paragraph;

                    if (p.StartTime.TotalSeconds <= position
                        && p.EndTime.TotalSeconds >= position)
                    {
                        this.nextSubtitleIndices.Add(i);
                    }
                }


#if DEBUG
                if (this.nextSubtitleIndices.Count != this.lastSubtitleIndices.Count || this.nextSubtitleIndices.Any(x => !this.lastSubtitleIndices.Contains(x)))
#else
            if (nextIndices.Count != lastIndices.Count || nextIndices.Any(x => !lastSubtitleIndices.Contains(x)))
#endif
                {
                    if (!this.Dispatcher.CheckAccess())
                    {
                        this.Dispatcher.Invoke(UpdateSubtitles);
                        return;
                    }

                    //UpdateSubtitles();
                }

                
            }
        }

        private void UpdateSubtitles()
        {
            this.SubtitleElement.ClearContent();
            this.lastSubtitleIndices.Clear();

            foreach (var nextIndex in this.nextSubtitleIndices)
            {
                this.lastSubtitleIndices.Add(nextIndex);
                this.SubtitleElement.Add(
                    this.controller.SelectedSubtitleTrack.Subtitles.Paragraphs[nextIndex]);
            }

            this.SubtitleElement.Visibility = this.nextSubtitleIndices.Count > 0 ? Visibility.Visible : Visibility.Hidden;

            this.SubtitleElement.InvalidateVisual();
        }

        private void OnMediaPlayerEnded()
        {
            MediaPlayerEnded?.Invoke();
        }
    }
}
