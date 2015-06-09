#region Usings

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Base;
using DirectShowLib;
using MediaPlayer.Helpers;

#endregion

namespace MediaPlayer.Player
{
    internal class FilterGraphs
    {
        #region Static Fields and Constants

        private const MediaPositionFormat preferedPositionFormat = MediaPositionFormat.MediaTime;

        #endregion

        #region Fields

        internal long Position;
        internal long Duration;
        internal long FrameDuration;

        /// <summary>
        /// DirectShow interface for controlling audio
        /// functions such as volume and balance
        /// </summary>
        internal IBasicAudio Audio;

        /// <summary>
        /// The DirectShow filter graph reference
        /// </summary>
        internal IFilterGraph2 FilterGraph;

        /// <summary>
        /// The DirectShow interface for controlling the
        /// filter graph.  This provides, Play, Pause, Stop, etc
        /// functionality.
        /// </summary>
        internal IMediaControl MediaControl;

        /// <summary>
        /// The DirectShow interface for getting events
        /// that occur in the FilterGraph.
        /// </summary>
        internal IMediaEventEx MediaEvent;

        /// <summary>
        /// The DirectShow media seeking interface
        /// </summary>
        internal IMediaSeeking Seeking;

        internal IVideoFrameStep VideoStep;

        /// <summary>
        /// The DirectShow graph interface.  In this example
        /// We keep reference to this so we can dispose 
        /// of it later.
        /// 
        /// Move this to somewhere else
        /// </summary>
        internal IGraphBuilder GraphBuilder;

        internal volatile bool HasVideo;
        internal int NaturalVideoHeight;
        internal int NaturalVideoWidth;
        internal Vmr9Allocator Allocator;
        private readonly PlayerController controller;
        private MediaPositionFormat currentPositionFormat;
        private IBaseFilter sourceFilter;

        #endregion

        #region Properties

        internal MediaPositionFormat CurrentPositionFormat
        {
            get { return currentPositionFormat; }
        }

        #endregion

        internal FilterGraphs(PlayerController controller)
        {
            this.controller = controller;
        }

        internal virtual void OpenSource(out ImpError result)
        {
            result = null;
            if (string.IsNullOrEmpty(controller.FilePath))
            {
                result = new ImpError(ErrorType.FileNotFound, "Empty filepath");
                return;
            }

            try
            {
                DisableVistaDecoders();

                /* Creates the GraphBuilder COM object */
                GraphBuilder = new FilterGraphNoThread() as IGraphBuilder;
                if (GraphBuilder == null)
                    throw new Exception("Could not create a graph");


                FilterGraph = GraphBuilder as IFilterGraph2;

                if (FilterGraph == null)
                    throw new Exception("Could not QueryInterface for the IFilterGraph2");

                /* Have DirectShow find the correct source filter for the Uri */
                var hr = FilterGraph.AddSourceFilter(controller.FilePath, controller.FilePath, out sourceFilter);
                DsError.ThrowExceptionForHR(hr);


                MediaEvent = GraphBuilder as IMediaEventEx;
                MediaControl = GraphBuilder as IMediaControl;

                if (FilterGraph == null)
                    throw new Exception("Could not QueryInterface for the IMediaControl");
            }
            catch (Exception ex)
            {
#if DEBUG
                throw;
#endif
                /* This exection will happen usually if the media does
                 * not exist or could not open due to not having the
                 * proper filters installed */
                FreeResources();

                result = new ImpError(ErrorType.FailedToOpenFile, ex.Message);
            }
        }

        private static void DisableVistaDecoders()
        {
            var osInfo = Environment.OSVersion;
            if (osInfo.Version.Major >= 6)
            {
                var dspc = new DirectShowPluginControl();
                var ipc = (IAMPluginControl) dspc;
                var aac = MediaSubType.Mpeg2Audio; //new Guid("00000001-0000-0010-8000-00AA00389B71");
                var pref = new Guid();
                var h264Default = new Guid("212690fb-83e5-4526-8fd7-74478b7939cd");
                var MP3Default = new Guid("e1f1a0b8-beee-490d-ba7c-066c40b5e2b9");
                var hr2 = ipc.GetPreferredClsid(aac, out pref);
                hr2 = ipc.SetDisabled(h264Default, true);
                hr2 = ipc.SetDisabled(MP3Default, true);
            }
        }

        internal bool RenderVideoStream(ref ImpError result, string filePath)
        {
            try
            {
                var renderer = CreateVideoMixingRenderer9(GraphBuilder, 2);

                /* We will want to enum all the pins on the source filter */
                IEnumPins pinEnum;

                var hr = sourceFilter.EnumPins(out pinEnum);
                DsError.ThrowExceptionForHR(hr);

                var fetched = IntPtr.Zero;
                IPin[] pins = {null};

                /* Counter for how many pins successfully rendered */
                var pinsRendered = 0;


                var mixer = renderer as IVMRMixerControl9;

                if (mixer != null)
                {
                    VMR9MixerPrefs dwPrefs;
                    mixer.GetMixingPrefs(out dwPrefs);
                    dwPrefs &= ~VMR9MixerPrefs.RenderTargetMask;
                    dwPrefs |= VMR9MixerPrefs.RenderTargetRGB;
                    //mixer.SetMixingPrefs(dwPrefs);
                }

                /* Loop over each pin of the source filter */
                while (pinEnum.Next(pins.Length, pins, fetched) == 0)
                {
                    if (FilterGraph.RenderEx(pins[0],
                        AMRenderExFlags.RenderToExistingRenderers,
                        IntPtr.Zero) >= 0)
                        pinsRendered++;

                    Marshal.ReleaseComObject(pins[0]);
                }

                Marshal.ReleaseComObject(pinEnum);
                Marshal.ReleaseComObject(sourceFilter);

                if (pinsRendered == 0)
                    throw new Exception("Could not render any streams from the source Uri");
                /* Configure the graph in the base class */

                VideoStep = GraphBuilder as IVideoFrameStep;

                //HasVideo = true;
                /* Sets the NaturalVideoWidth/Height */
                SetNativePixelSizes(renderer);
            }
            catch (Exception ex)
            {
                if (result != null && result.Type == ErrorType.CouldNotRenderAudio)
                {
                    // could not render video or audio
                    FreeResources();
                    result.Type = ErrorType.FailedToOpenFile;
                    return true;
                }

                result = new ImpError(ErrorType.CouldNotRenderVideo, ex.Message);
            }
            return false;
        }

        /// <summary>
        ///     Creates a new VMR9 renderer and configures it with an allocator
        /// </summary>
        /// <returns>An initialized DirectShow VMR9 renderer</returns>
        private IBaseFilter CreateVideoMixingRenderer9(IGraphBuilder graph, int streamCount)
        {
            var vmr9 = new VideoMixingRenderer9() as IBaseFilter;

            var filterConfig = vmr9 as IVMRFilterConfig9;

            if (filterConfig == null)
                throw new Exception("Could not query filter configuration.");

            /* We will only have one video stream connected to the filter */
            var hr = filterConfig.SetNumberOfStreams(streamCount);
            DsError.ThrowExceptionForHR(hr);

            /* Setting the renderer to "Renderless" mode
             * sounds counter productive, but its what we
             * need to do for setting up a custom allocator */
            hr = filterConfig.SetRenderingMode(VMR9Mode.Renderless);
            DsError.ThrowExceptionForHR(hr);

            /* Query the allocator interface */
            var vmrSurfAllocNotify = vmr9 as IVMRSurfaceAllocatorNotify9;

            if (vmrSurfAllocNotify == null)
                throw new Exception("Could not query the VMR surface allocator.");

            Allocator = new Vmr9Allocator();

            /* We supply our custom allocator to the renderer */
            hr = vmrSurfAllocNotify.AdviseSurfaceAllocator(Vmr9Allocator.UserId, Allocator);
            DsError.ThrowExceptionForHR(hr);

            hr = Allocator.AdviseNotify(vmrSurfAllocNotify);
            DsError.ThrowExceptionForHR(hr);


            hr = graph.AddFilter(vmr9, string.Format("Renderer: {0}", "0"));

            DsError.ThrowExceptionForHR(hr);

            return vmr9;
        }

        /// <summary>
        ///     Setup the IMediaSeeking interface
        /// </summary>
        protected internal void SetMediaSeekingInterface(IMediaSeeking mediaSeeking)
        {
            Seeking = mediaSeeking;

            if (mediaSeeking == null)
            {
                currentPositionFormat = MediaPositionFormat.None;
                Duration = 0;
                return;
            }

            /* Get our prefered DirectShow TimeFormat */
            var preferedFormat = FormatHelper.ConvertPositionFormat(preferedPositionFormat);

            /* Attempt to set the time format */
            mediaSeeking.SetTimeFormat(preferedFormat);

            Guid currentFormat;

            /* Gets the current time format
             * we may not have been successful
             * setting our prefered format */
            mediaSeeking.GetTimeFormat(out currentFormat);

            /* Set our property up with the right format */
            currentPositionFormat = FormatHelper.ConvertPositionFormat(currentFormat);

            /* Get the duration of the media.  This value will
             * be in whatever format that was set. ie Frame, MediaTime */
            Seeking.GetDuration(out Duration);
        }

        /// <summary>
        ///     Sets the natural pixel resolution the video in the graph
        /// </summary>
        /// <param name="renderer">The video renderer</param>
        protected void SetNativePixelSizes(IBaseFilter renderer)
        {
            var hasVideo = false;

            var size = GetVideoSize(renderer, PinDirection.Input, 0, out hasVideo, out FrameDuration);

            NaturalVideoHeight = (int) size.Height;
            NaturalVideoWidth = (int) size.Width;
            HasVideo = hasVideo;
        }

        /// <summary>
        ///     Gets the video resolution of a pin on a renderer.
        /// </summary>
        /// <param name="renderer">The renderer to inspect</param>
        /// <param name="direction">The direction the pin is</param>
        /// <param name="pinIndex">The zero based index of the pin to inspect</param>
        /// <param name="hasVideo"> Whether the media has video</param>
        /// <param name="frameSpeed"> Speed of a frame</param>
        /// <returns>If successful a video resolution is returned.  If not, a 0x0 size is returned</returns>
        protected static Size GetVideoSize(IBaseFilter renderer, PinDirection direction, int pinIndex, out bool hasVideo,
            out long frameSpeed)
        {
            frameSpeed = 0; // safety check

            hasVideo = false;
            var size = new Size();

            var mediaType = new AMMediaType();
            var pin = DsFindPin.ByDirection(renderer, direction, pinIndex);


            if (pin == null)
            {
                DsUtils.FreeAMMediaType(mediaType);
                return size;
            }

            var hr = pin.ConnectionMediaType(mediaType);

            /* Check to see if its a video media type */
            if (hr == 0 && (mediaType.formatType == FormatType.VideoInfo2 || mediaType.formatType == FormatType.VideoInfo))
            {
                var videoInfo = new VideoInfoHeader();

                /* Read the video info header struct from the native pointer */
                Marshal.PtrToStructure(mediaType.formatPtr, videoInfo);

                var rect = videoInfo.SrcRect.ToRectangle();
                size = new Size(rect.Width, rect.Height);
                hasVideo = true;
                frameSpeed = videoInfo.AvgTimePerFrame;
            }

            DsUtils.FreeAMMediaType(mediaType);
            Marshal.ReleaseComObject(pin);

            return size;
        }

        internal void RenderAudioStream(ref ImpError result)
        {
            try
            {
                if (GraphBuilder == null)
                    return;

                /* Add our prefered audio renderer */
                Audio = GraphBuilder as IBasicAudio;

                AddFilterByName(GraphBuilder, FilterCategory.AudioRendererCategory, DShowHelper.DEFAULT_AUDIO_RENDERER_NAME);
                if (Audio == null)
                    throw new Exception("Could not find audio");
            }
            catch (Exception ex)
            {
                result = new ImpError(ErrorType.CouldNotRenderAudio, ex.Message);
            }
        }

        /// <summary>
        /// Adds a filter to a DirectShow graph based on it's name and filter category
        /// </summary>
        /// <param name="graphBuilder">The graph builder to add the filter to</param>
        /// <param name="deviceCategory">The category the filter belongs to</param>
        /// <param name="friendlyName">The friendly name of the filter</param>
        /// <returns>Reference to the IBaseFilter that was added to the graph or returns null if unsuccessful</returns>
        protected static IBaseFilter AddFilterByName(IGraphBuilder graphBuilder, Guid deviceCategory, string friendlyName)
        {
            var device = DsDevice.GetDevicesOfCat(deviceCategory).FirstOrDefault(x => x.Name == friendlyName);
            return AddFilterByDevice(graphBuilder, device);
        }

        protected static IBaseFilter AddFilterByDevicePath(IGraphBuilder graphBuilder, Guid deviceCategory, string devicePath)
        {
            var device = DsDevice.GetDevicesOfCat(deviceCategory).FirstOrDefault(x => x.DevicePath == devicePath);
            return AddFilterByDevice(graphBuilder, device);
        }

        private static IBaseFilter AddFilterByDevice(IGraphBuilder graphBuilder, DsDevice device)
        {
            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            var filterGraph = graphBuilder as IFilterGraph2;

            if (filterGraph == null)
                return null;

            IBaseFilter filter = null;
            if (device != null)
            {
                var hr = filterGraph.AddSourceFilterForMoniker(device.Mon, null, device.Name, out filter);
                DsError.ThrowExceptionForHR(hr);
            }
            return filter;
        }

        public void FreeResources()
        {
            Position = 0;
            FrameDuration = 0;
            Duration = 0;

            if (MediaControl != null)
            {
                FilterState filterState;
                MediaControl.GetState(0, out filterState);
                if (filterState == FilterState.Running)
                    MediaControl.Pause();

                MediaControl.GetState(0, out filterState);

                while (filterState == FilterState.Running)
                    MediaControl.GetState(0, out filterState);
                Marshal.ReleaseComObject(MediaControl);
            }


            if (Audio != null)
                Marshal.ReleaseComObject(Audio);

            if (MediaEvent != null)
                Marshal.ReleaseComObject(MediaEvent);

            if (Seeking != null)
                Marshal.ReleaseComObject(Seeking);

            if (VideoStep != null)
                Marshal.ReleaseComObject(VideoStep);

            if (FilterGraph != null)
                Marshal.ReleaseComObject(FilterGraph);

            if (GraphBuilder != null)
                Marshal.ReleaseComObject(GraphBuilder);

            Audio = null;
            MediaControl = null;
            MediaEvent = null;
            Seeking = null;
            VideoStep = null;
            FilterGraph = null;
            GraphBuilder = null;
            GC.Collect();
        }
    }
}