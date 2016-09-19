#region Usings

using System;
using Imp.Base.Interfaces;
using Imp.Base.Libraries;
using Imp.DirectShow.Player;

#endregion

namespace Imp.DirectShow.Element
{
    public class MediaUriPlayer : D3DRenderer, IMediaUriPlayer
    {
        #region Fields

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

                    controller.NewAllocatorFrame -= OnMediaPlayerNewAllocatorFramePrivate;
                    controller.NewAllocatorSurface -= OnMediaPlayerNewAllocatorSurfacePrivate;
                    controller = null;
                }
                controller = value;
                InitializeMediaPlayer();
            }
        }

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
            /* Hook into the normal .NET events */
            controller.MediaEnded += OnMediaPlayerEnded;

            /* These events fire when we get new D3Dsurfaces or frames */
            controller.NewAllocatorFrame += OnMediaPlayerNewAllocatorFramePrivate;
            controller.NewAllocatorSurface += OnMediaPlayerNewAllocatorSurfacePrivate;

            controller.Volume = Volume;
            controller.Activate();
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
            MediaPlayerEnded?.Invoke();
        }
    }
}