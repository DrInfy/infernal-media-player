﻿using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using Base;
using Base.Controllers;

using DirectShowLib;
using MediaPlayer.Helpers;
using Size = System.Windows.Size;
using Timer = System.Timers.Timer;


namespace MediaPlayer.Player
{
    public class PlayerController
    {
        private bool reallyPlaying = false;
        private ImpFader fader;

        internal readonly string FilePath;
        private readonly Dispatcher dispatcher;
        private volatile bool timerUpdating = false;
        private FilterGraphs graphs;

        private double fadingVolume;
        //private Dispatcher dispatcher;

        private MediaCommand queuedCommand = MediaCommand.Stop;
        private MediaCommand currentCommand = MediaCommand.Stop;

        /// <summary>
        ///     The custom DirectShow allocator
        /// </summary>
        private ICustomAllocator customAllocator;

        /// <summary>
        ///     Our Win32 timer to poll the DirectShow graph
        /// </summary>
        private Timer graphPollTimer;

        private double volume;

        public bool Released { get; private set; }

        /// <summary>
        /// Read-only property for current media duration.
        /// </summary>
        public long Duration
        {
            get { return graphs.Duration; }
        }

        public long Position
        {
            get { return fader.GetPosition(graphs.Position); }
        }

        public double Volume
        {
            get { return volume; }
            set
            {
                fader.SetVolume(value);
                volume = value;
            }
        }

        public bool HasVideo
        {
            get { return graphs.HasVideo; }
        }


        public PlayerController(string filePath, Dispatcher dispatcher)
        {
            graphs = new FilterGraphs(this);
            this.FilePath = filePath;
            this.dispatcher = dispatcher;
            //this.dispatcher = dispatcher;
            fader = new ImpFader();
        }


        public void HandleCommand()
        {
            if (currentCommand != queuedCommand)
            {
                currentCommand = queuedCommand;
                
                switch (currentCommand)
                {
                    case MediaCommand.Play:
                        //graphs.MediaControl.Run();
                        fader.Play();
                        break;
                    case MediaCommand.Stop:
                        fader.Pause();
                        //fader.SetPosition(0);
                        //graphs.MediaControl.Stop();
                        break;
                    case MediaCommand.Close:
                        graphPollTimer.Enabled = false;
                        FreeResources();
                        return;
                    case MediaCommand.Pause:
                        fader.Pause();
                        //graphs.MediaControl.Pause();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (fader.Active)
                graphPollTimer.Interval = 1;
            else
                graphPollTimer.Interval = 15;
        }

        public void Command(MediaCommand command)
        {
            // prevent any commands if the player is determined to be closed
            if (queuedCommand != MediaCommand.Close && currentCommand != MediaCommand.Close)
                queuedCommand = command;
        }


        private void UpdateFade()
        {
         
            long position = graphs.Position;
            long oldPosition = position;
            //int volumeInt;
            //graphs.Audio.get_Volume(out volumeInt);
            //fadingVolume = DShowHelper.DirectShowVolumeToVolumePercentage(volumeInt);
            double oldVolume = fadingVolume;// DShowHelper.DirectShowVolumeToVolumePercentage(volumeInt);

            if (fader.Update(ref position, ref oldVolume, currentCommand == MediaCommand.Stop))
            {
                if (reallyPlaying)
                {
                    graphs.MediaControl.Pause();
                    reallyPlaying = false;   
                }
            }
            else
            {
                if (!reallyPlaying && currentCommand == MediaCommand.Play)
                {
                    graphs.MediaControl.Run();
                    reallyPlaying = true;
                }
            }
                
            
            //DShowHelper.VolumePercentageToDirectShowVolume(volume);
            if (oldPosition != position)
                graphs.Seeking.SetPositions(position, AMSeekingSeekingFlags.AbsolutePositioning, 0,
                    AMSeekingSeekingFlags.NoPositioning);

            if (oldVolume != fadingVolume)
            {
                fadingVolume = oldVolume;
                if (fadingVolume <= 0)
                    fadingVolume = oldVolume;
                graphs.Audio.put_Volume(DShowHelper.VolumePercentageToDirectShowVolume(fadingVolume));
            }
                
        }
        

        /// <summary>
        /// Notifies when the media has completed
        /// </summary>
        public event Action MediaEnded;


        /// <summary>
        ///     Event notifies when there is a new video frame
        ///     to be rendered
        /// </summary>
        public event Action NewAllocatorFrame;

        /// <summary>
        ///     Event notifies when there is a new surface allocated
        /// </summary>
        public event NewAllocatorSurfaceDelegate NewAllocatorSurface;


        public virtual void OpenSource(out ImpError result)
        {
            result = null;
            graphs.OpenSource(out result);

            graphs.RenderAudioStream(ref result);

            graphs.RenderVideoStream(ref result, FilePath);
            graphs.SetMediaSeekingInterface(graphs.GraphBuilder as IMediaSeeking);
        }


        public ImpError Activate()
        {
            ImpError result = null;
            //Dispatcher.DispatcherThread = Thread.CurrentThread;
            if (graphs.HasVideo)
            {
                RegisterCustomAllocator(graphs.Allocator);
                graphs.Allocator.InvokeSurfaceCreation();    
            }
            else
            {
                NewAllocatorSurface.Invoke(this, IntPtr.Zero);
            }

            graphs.Audio.put_Volume(DShowHelper.DSHOW_VOLUME_SILENCE);
            //graphs.MediaControl.Pause();
            queuedCommand = MediaCommand.Play;

            /* This is generally a good place to start
             * our polling timer */
            StartGraphPollTimer();
            return result;
        }

        


        /// <summary>
        /// Starts the graph polling timer to update possibly needed
        /// things like the media position
        /// </summary>
        protected void StartGraphPollTimer()
        {
            if (graphPollTimer == null)
            {
                graphPollTimer = new Timer {Interval = DShowHelper.DSHOW_TIMER_POLL_MS};
                graphPollTimer.Elapsed += TimerElapsed;
            }

            graphPollTimer.Enabled = true;
        }


        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (timerUpdating)
                return;
            timerUpdating = true;

            HandleCommand();
            if (currentCommand == MediaCommand.Close)
                return;

            UpdateFade();                

            //Dispatcher.BeginInvoke((Action)delegate
            //{
                
            ProcessGraphEvents();
                //OnGraphTimerTick();
            //});
            graphs.Seeking.GetCurrentPosition(out graphs.Position);
            timerUpdating = false;
        }



        private void ProcessGraphEvents()
        {
            if (graphs.MediaEvent != null)
            {
                IntPtr param1;
                IntPtr param2;
                EventCode code;

                /* Get all the queued events from the interface */
                while (graphs.MediaEvent.GetEvent(out code, out param1, out param2, 0) == 0)
                {
                    /* Handle anything for this event code */
                    OnMediaEvent(code, param1, param2);

                    /* Free everything..we only need the code */
                    graphs.MediaEvent.FreeEventParams(code, param1, param2);
                }
            }
        }

        /// <summary>
        /// Is called when a new media event code occurs on the graph
        /// </summary>
        /// <param name="code">The event code that occured</param>
        /// <param name="param1">The first parameter sent by the graph</param>
        /// <param name="param2">The second parameter sent by the graph</param>
        protected virtual void OnMediaEvent(EventCode code, IntPtr param1, IntPtr param2)
        {
            switch (code)
            {
                case EventCode.Complete:
                    dispatcher.BeginInvoke((Action) (() => InvokeMediaEnded(null)));
                    //StopGraphPollTimer();
                    break;
                case EventCode.Paused:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Invokes the MediaEnded event, notifying any subscriber that
        /// media has reached the end
        /// </summary>
        protected void InvokeMediaEnded(EventArgs e)
        {
            var mediaEndedHandler = MediaEnded;
            if (mediaEndedHandler != null)
                mediaEndedHandler();
        }


        private void FreeResources()
        {
            reallyPlaying = false;
            //VerifyAccess();
            StopGraphPollTimer();
            FreeCustomAllocator();
            graphs.FreeResources();
            Released = true;
        }


        /// <summary>
        ///     Stops the graph polling timer
        /// </summary>
        protected void StopGraphPollTimer()
        {
            if (graphPollTimer != null)
            {
                graphPollTimer.Stop();
                graphPollTimer.Dispose();
                graphPollTimer = null;
            }
        }


        /// <summary>
        ///     Disposes of the current allocator
        /// </summary>
        protected void FreeCustomAllocator()
        {
            if (customAllocator == null)
                return;

            customAllocator.Dispose();

            customAllocator.NewAllocatorFrame -= CustomAllocatorNewAllocatorFrame;
            customAllocator.NewAllocatorSurface -= CustomAllocatorNewAllocatorSurface;

            if (Marshal.IsComObject(customAllocator))
                Marshal.ReleaseComObject(customAllocator);

            customAllocator = null;
        }


        

        /// <summary>
        ///     Registers the custom allocator and hooks into it's supplied events
        /// </summary>
        protected internal void RegisterCustomAllocator(ICustomAllocator allocator)
        {
            FreeCustomAllocator();

            if (allocator == null)
                return;

            customAllocator = allocator;

            customAllocator.NewAllocatorFrame += CustomAllocatorNewAllocatorFrame;
            customAllocator.NewAllocatorSurface += CustomAllocatorNewAllocatorSurface;
        }



        /// <summary>
        ///     Local event handler for the custom allocator's new surface event
        /// </summary>
        private void CustomAllocatorNewAllocatorSurface(object sender, IntPtr pSurface)
        {
            InvokeNewAllocatorSurface(pSurface);
        }


        /// <summary>
        ///     Local event handler for the custom allocator's new frame event
        /// </summary>
        private void CustomAllocatorNewAllocatorFrame()
        {
            InvokeNewAllocatorFrame();
        }


        /// <summary>
        ///     Invokes the NewAllocatorFrame event, notifying any subscriber that new frame
        ///     is ready to be presented.
        /// </summary>
        protected void InvokeNewAllocatorFrame()
        {
            Action newAllocatorFrameHandler = NewAllocatorFrame;
            if (newAllocatorFrameHandler != null)
                newAllocatorFrameHandler();
        }


        /// <summary>
        ///     Invokes the NewAllocatorSurface event, notifying any subscriber of a new surface
        /// </summary>
        /// <param name="pSurface">The COM pointer to the D3D surface</param>
        protected void InvokeNewAllocatorSurface(IntPtr pSurface)
        {
            NewAllocatorSurfaceDelegate del = NewAllocatorSurface;
            if (del != null)
                del(this, pSurface);
        }


        public void MoveTo(long value)
        {
            fader.SetPosition(value);
        }
    }
}