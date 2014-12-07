using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Base.Interfaces;
using Base.Libraries;
using MediaPlayer.Player;

namespace MediaPlayer.Element
{
    public class MediaUriPlayer : D3DRenderer, IMediaUriPlayer
    {
        public event Action MediaPlayerEnded;

        private PlayerController controller;
        private double volume = 1;

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
                    
                    //while (!controller.Released)
                    //{
                    //    // wait for media player closure
                    //    Thread.Sleep(1);
                    //}
                    controller.NewAllocatorFrame -= OnMediaPlayerNewAllocatorFramePrivate;
                    controller.NewAllocatorSurface -= OnMediaPlayerNewAllocatorSurfacePrivate;
                    controller = null;
                }
                controller = value;
                InitializeMediaPlayer();
            }
        }


        public void Play()
        {
            if (controller != null)
            controller.Command(MediaCommand.Play);
        }

        public void Pause()
        {
            if (controller != null)
                controller.Command(MediaCommand.Pause);
        }


        public void Stop()
        {
            if (controller != null)
                controller.Command(MediaCommand.Stop);
        }


        protected virtual void InitializeMediaPlayer()
        {


            /* Hook into the normal .NET events */
            //MediaPlayerBase.MediaClosed += OnMediaPlayerClosedPrivate;
            //MediaPlayerBase.MediaFailed += OnMediaPlayerFailedPrivate;
            controller.MediaEnded += OnMediaPlayerEnded;

            /* These events fire when we get new D3Dsurfaces or frames */
            controller.NewAllocatorFrame += OnMediaPlayerNewAllocatorFramePrivate;
            controller.NewAllocatorSurface += OnMediaPlayerNewAllocatorSurfacePrivate;

            controller.Volume = Volume;
            var result = controller.Activate();
            
            // TODO: raise event for error message
        }

        public void Clear()
        {
            if (controller != null)
                controller.Command(MediaCommand.Close);
            controller = null;
        }


        public bool IsPlaying { get { return controller != null; } }


        public double Duration
        {
            get
            {
                return controller != null ? LibImp.TicksToSeconds(controller.Duration) : 0;
            }
        }

        public double Position
        {
            get
            {
                return controller != null ? LibImp.TicksToSeconds(controller.Position) : 0;
            }
            set
            {
                if (controller != null)
                    controller.MoveTo(LibImp.SecondsToTicks(value));
            }
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

        public bool HasVideo
        {
            get
            {
                if (controller == null) return false;
                return controller.HasVideo;
            }
        }


        /// <summary>
        /// Is executes when a new D3D surfaces has been allocated
        /// </summary>
        /// <param name="pSurface">The pointer to the D3D surface</param>
        private void OnMediaPlayerNewAllocatorSurfacePrivate(object sender, IntPtr pSurface)
        {
            SetBackBuffer(pSurface); 
        }


        private void OnMediaPlayerNewAllocatorFramePrivate()
        {
            InvalidateVideoImage();
        }


        private void OnMediaPlayerEnded()
        {
            if (MediaPlayerEnded != null)
                MediaPlayerEnded.Invoke();
        }
    }
}