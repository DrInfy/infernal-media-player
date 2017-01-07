#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using DirectShowLib;
using Imp.Base;
using Imp.Base.Controllers;
using Imp.DirectShow.Helpers;
using Imp.DirectShow.Subtitles;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;
using Nikse.SubtitleEdit.Core.ContainerFormats.Mp4;
using Nikse.SubtitleEdit.Core.ContainerFormats.Mp4.Boxes;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using Imp.Base.Libraries;
using Imp.DirectShow.Element;
using Timer = System.Timers.Timer;

#endregion

namespace Imp.DirectShow.Player
{
    public class PlayerController
    {
        #region Fields

        internal readonly string FilePath;
        private readonly ImpFader fader;
        private readonly Dispatcher dispatcher;
        private readonly FilterGraphs graphs;
        private bool reallyPlaying = false;
        private volatile bool timerUpdating = false;
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

        #endregion

        #region Properties

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

        public Size VideoSize => new Size(graphs.NaturalVideoWidth, graphs.NaturalVideoHeight);

        public SubtitleTrack SelectedSubtitleTrack { get; private set; }

        public int SubtitleTrackIndex
        {
            get { return this.subtitleTrackIndex; }
            set
            {
                this.subtitleTrackIndex = value;

                if (this.SubtitleTracks.Count <= value || value < 0)
                {
                    this.subtitleTrackIndex = -1;
                    this.SelectedSubtitleTrack = null;
                }
                else
                {
                    this.SelectedSubtitleTrack = this.SubtitleTracks[value];
                }

                this.Player.ResetSubtitles();
            }
        }

        #endregion

        public List<SubtitleTrack> SubtitleTracks = new List<SubtitleTrack>();

        public Dictionary<string, FontFamily> Fonts { get; } = new Dictionary<string, FontFamily>();
        public MediaUriPlayer Player { get; set; }

        private readonly bool subtitles;
        private int subtitleTrackIndex;

        #region Events

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

        #endregion

        public PlayerController(string filePath, Dispatcher dispatcher, bool subtitles)
        {
            graphs = new FilterGraphs(this);
            FilePath = filePath;
            this.dispatcher = dispatcher;
            //this.dispatcher = dispatcher;
            fader = new ImpFader();
            this.subtitles = subtitles;
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
                        
                        fader.Pause();
                        break;
                        
                    case MediaCommand.Pause:
                        fader.Pause();
                        //graphs.MediaControl.Pause();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (currentCommand == MediaCommand.Close)
            {
                if (fadingVolume <= 0)
                {
                    graphPollTimer.Enabled = false;
                    FreeResources();
                    return;
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

            if (command == MediaCommand.Close)
            {
                Player?.SubtitleElement?.ClearContent();
            }
        }

        private void UpdateFade()
        {
            var position = graphs.Position;
            var oldPosition = position;
            //int volumeInt;
            //graphs.Audio.get_Volume(out volumeInt);
            //fadingVolume = DShowHelper.DirectShowVolumeToVolumePercentage(volumeInt);
            var oldVolume = fadingVolume; // DShowHelper.DirectShowVolumeToVolumePercentage(volumeInt);

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
                    for (int i = 0; i < 100; i++)
                    {
                        var result = graphs.MediaControl.Run();
                        if (result == 0)
                        {
                            reallyPlaying = true;
                            break;
                        }
                        Thread.Sleep(1);
                    }

                    
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

        public virtual void OpenSource(out ImpError result)
        {
            this.SelectedSubtitleTrack = null;
            if (subtitles)
            {
                ReadTracks();
            }

            graphs.OpenSource(out result);

            graphs.RenderAudioStream(ref result);

            graphs.RenderVideoStream(ref result, FilePath);
            graphs.SetMediaSeekingInterface(graphs.GraphBuilder as IMediaSeeking);

            if (subtitles)
            {
                FinalizeSubtitles();
            }
        }

        #region Subtitles

        private void FinalizeSubtitles()
        {
            foreach (var subtitleTrack in this.SubtitleTracks)
            {
                subtitleTrack.Subtitles = CreateEnhancedSubTitles(subtitleTrack.RawSubs, subtitleTrack.Format, this.VideoSize);
                if (subtitleTrack.Subtitles != null)
                {
                    subtitleTrack.Loaded = true;
                    if (this.SelectedSubtitleTrack == null)
                    {
                        this.SelectedSubtitleTrack = subtitleTrack;
                    }
                }
            }
        }

        private void ReadTracks()
        {
            var extension = Path.GetExtension(this.FilePath);
            if (extension == ".mkv" || extension == ".mks")
            {
                using (var matroska = new MatroskaFile(this.FilePath))
                {
                    var stopwatch = Stopwatch.StartNew();
                    if (matroska.ReadMetadata())
                    {
                        DebugUtil.WriteLine($"Read metadata time: {(int) stopwatch.Elapsed.TotalMilliseconds} ms");

                        stopwatch.Restart();
                        var subTitleTrackInfo = matroska.Subtitles;

                        var mkvSubs = matroska.GetSubtitle(null);
                        DebugUtil.WriteLine($"Read from file time: {(int)stopwatch.Elapsed.TotalMilliseconds} ms");
                        stopwatch.Restart();

                        foreach (var info in subTitleTrackInfo)
                        {
                            var resultingSubs = new Subtitle();
                            var subInfo = new SubtitleTrack();
                            subInfo.Language = info.Language;
                            subInfo.Name = info.Name;
                            this.SubtitleTracks.Add(subInfo);

                            subInfo.Format = Utilities.LoadMatroskaTextSubtitle(info, matroska, mkvSubs[info.TrackNumber], resultingSubs);
                            subInfo.RawSubs = resultingSubs;

                            DebugUtil.WriteLine($"Read from matroska time: {(int)stopwatch.Elapsed.TotalMilliseconds} ms");
                            stopwatch.Restart();

                            //subInfo.Subtitles = CreateEnhancedSubTitles(resultingSubs, format, this.VideoSize);
                        }

                        // Load all subtitles
                        LoadFonts(matroska);
                        Debug.WriteLine($"Read fonts time: {(int)stopwatch.Elapsed.TotalMilliseconds} ms");
                        stopwatch.Restart();
                    }
                }
            }
            else if (extension == ".mp4" || extension == ".m4v" || extension == ".3gp")
            {
                //var mp4Parser = new MP4Parser(this.FilePath);
                //var mp4SubtitleTracks = mp4Parser.GetSubtitleTracks();

                //foreach (var trak in mp4SubtitleTracks)
                //{
                //    var subInfo = new SubtitleTrack();
                //    subInfo.Language = trak.Mdia.Mdhd.LanguageString;
                //    subInfo.Name = trak.Name;
                //    this.SubtitleTracks.Add(subInfo);
                //    subInfo.RawSubs = LoadMp4SubtitleForSync(trak);
                //}
            }
        }

        private void LoadFonts(MatroskaFile matroska)
        {
            try
            {
                this.Fonts.Clear();
                var attachments = matroska.Attachments;
                //Dictionary<string, List<MatroskaAttachment>> fontPackageDictionary = new Dictionary<string, List<MatroskaAttachment>>();
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        string familyName;
                        FontLoader.SaveToDisc(attachment.Data, attachment.Name);
                    }

                    var result = FontLoader.LoadFromDisc();

                    foreach (var fontFamily in result)
                    {
                        AddFont(fontFamily.Key, fontFamily.Value);
                    }
                }
                //return;

                //foreach (var attachment in attachments)
                //{
                //    if (attachment.MimeType == FontLoader.TrueTypeFont
                //        || attachment.MimeType == FontLoader.FontTtf)
                //    {
                //        var familyName = FontLoader.LoadFontFamilyName(attachment.Data);

                //        if (fontPackageDictionary.ContainsKey(familyName))
                //        {
                //            fontPackageDictionary[familyName].Add(attachment);

                //        }
                //        else
                //        {
                //            fontPackageDictionary.Add(familyName, new List<MatroskaAttachment>() { attachment });
                //        }
                //    }
                //}

                //foreach (var valuePair in fontPackageDictionary)
                //{
                //    string familyName = valuePair.Key;
                //    var font = FontLoader.LoadFontFamily(valuePair.Value.Select(x => x.Data), familyName);

                //    if (familyName != null && font != null)
                //    {
                //        subtitleElement.AddFont(familyName, font);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Debugger.Break();

            }

        }

        public void AddFont(string fontFaceName, FontFamily fontFamily)
        {
            if (!this.Fonts.ContainsKey(fontFaceName.ToLower()))
            {
                //TODO: remove this check
                this.Fonts.Add(fontFaceName.ToLower(), fontFamily);
            }
        }

        private static Subtitle LoadMp4SubtitleForSync(Trak mp4SubtitleTrack)
        {
            var subtitle = new Subtitle();
            if (mp4SubtitleTrack.Mdia.IsVobSubSubtitle)
            {
                // Not supported
                return subtitle;
            }
            else
            {
                for (int i = 0; i < mp4SubtitleTrack.Mdia.Minf.Stbl.EndTimeCodes.Count; i++)
                {
                    if (mp4SubtitleTrack.Mdia.Minf.Stbl.Texts.Count > i)
                    {
                        var start = TimeSpan.FromSeconds(mp4SubtitleTrack.Mdia.Minf.Stbl.StartTimeCodes[i]);
                        var end = TimeSpan.FromSeconds(mp4SubtitleTrack.Mdia.Minf.Stbl.EndTimeCodes[i]);
                        string text = mp4SubtitleTrack.Mdia.Minf.Stbl.Texts[i];
                        var p = new Paragraph(text, start.TotalMilliseconds, end.TotalMilliseconds);
                        if (p.EndTime.TotalMilliseconds - p.StartTime.TotalMilliseconds > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                            p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds;

                        if (mp4SubtitleTrack.Mdia.IsClosedCaption && string.IsNullOrEmpty(text))
                        {
                            // do not add empty lines
                        }
                        else
                        {
                            subtitle.Paragraphs.Add(p);
                        }
                    }
                }
            }
            return subtitle;
        }

        private SubtitleStylized CreateEnhancedSubTitles(Subtitle selectedSubs, SubtitleFormat format, Size playerControllerVideoSize)
        {
            try
            {
                var finalSubs = SubtitleFormatReader.ParseHeader(selectedSubs, format);
                if (finalSubs.Header.PlayResX == null && finalSubs.Header.PlayResY == null)
                {
                    finalSubs.Header.PlayResX = (int)playerControllerVideoSize.Width;
                    finalSubs.Header.PlayResY = (int)playerControllerVideoSize.Height;
                }
                else if (finalSubs.Header.PlayResY == null)
                {
                    finalSubs.Header.PlayResY = (int)(finalSubs.Header.PlayResX * playerControllerVideoSize.Height / playerControllerVideoSize.Width);
                }
                else if (finalSubs.Header.PlayResX == null)
                {
                    finalSubs.Header.PlayResY = (int)(finalSubs.Header.PlayResY * playerControllerVideoSize.Width / playerControllerVideoSize.Height);
                }

                finalSubs.Paragraphs = new List<EnhancedParagraph>();

                for (int i = 0; i < selectedSubs.Paragraphs.Count; i++)
                {
                    var p = selectedSubs.Paragraphs[i];
                    finalSubs.Paragraphs.Add(new EnhancedParagraph(finalSubs.Header, p));
                }

                return finalSubs;
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            return null;
        }


        #endregion 

        public void Activate()
        {
            if (graphs.HasVideo)
            {
                RegisterCustomAllocator(graphs.Allocator);
                graphs.Allocator.InvokeSurfaceCreation();
            }
            else
            {
                NewAllocatorSurface?.Invoke(this, IntPtr.Zero);
            }

            //graphs.Audio.put_Volume(DShowHelper.DSHOW_VOLUME_SILENCE);
            graphs.Audio.put_Volume(DShowHelper.VolumePercentageToDirectShowVolume(volume));
            fadingVolume = volume;
            //fader.SetVolume(volume);
            queuedCommand = MediaCommand.Play;
            StartGraphPollTimer();
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

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (timerUpdating) { return; }
            timerUpdating = true;

            HandleCommand();

            UpdateFade();
            if (currentCommand == MediaCommand.Close)
            {
                timerUpdating = false;
                return;
            }
            ProcessGraphEvents();

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
            MediaEnded?.Invoke();
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
            NewAllocatorFrame?.Invoke();
        }

        /// <summary>
        ///     Invokes the NewAllocatorSurface event, notifying any subscriber of a new surface
        /// </summary>
        /// <param name="pSurface">The COM pointer to the D3D surface</param>
        protected void InvokeNewAllocatorSurface(IntPtr pSurface)
        {
            NewAllocatorSurface?.Invoke(this, pSurface);
        }

        public void MoveTo(long value)
        {
            fader.SetPosition(value);
        }
    }
}