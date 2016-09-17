using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Base.Controllers;
using Base.Subtitles;
using Imp.Controls;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;
using Nikse.SubtitleEdit.Core.ContainerFormats.Mp4;
using Nikse.SubtitleEdit.Core.ContainerFormats.Mp4.Boxes;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace Imp.Controllers
{
    public class SubtitleController
    {
        public bool Active { get; private set; }
        private Subtitle selectedSubtitle = new Subtitle();
        private readonly Subtitles subtitleLabel;
        private readonly MainWindow window;
        private SubtitleFormat subtitleFormat;
        private Dictionary<int, EnhancedParagraph> indexToEnhancedParagraphs = new Dictionary<int, EnhancedParagraph>();

        public SubtitleController(MainWindow window)
        {
            this.subtitleLabel = window.Subtitles;
            this.window = window;
        }

        public void Clear()
        {
            selectedSubtitle.Paragraphs.Clear();
            Active = false;
            indexToEnhancedParagraphs.Clear();
            subtitleLabel.Clear();
            subtitleLabel.Visibility = Visibility.Hidden;
            indexToEnhancedParagraphs.Clear();
        }

        public void LoadSubtitles(string fileName, string extension, Size playerControllerVideoSize)
        {
            if (extension == ".mkv" || extension == ".mks")
            {
                using (var matroska = new MatroskaFile(fileName))
                {
                    if (matroska.IsValid)
                    {
                        var subtitleList = matroska.GetTracks(true);
                        if (subtitleList.Count == 0)
                        {
                            Active = false;
                        }
                        else
                        {
                            Active = true;
                            var selectedSubs = subtitleList.First();
                            var mkvSubs = matroska.GetSubtitle(selectedSubs.TrackNumber, null);

                            selectedSubtitle.Paragraphs.Clear();
                            subtitleFormat = Utilities.LoadMatroskaTextSubtitle(selectedSubs, matroska, mkvSubs,
                                selectedSubtitle);
                            subtitleLabel.Visibility = Visibility.Visible;

                            
                            CreateEnhancedSubTitles(selectedSubtitle, subtitleFormat, playerControllerVideoSize);
                        }
                    }
                }
            }
            else if (extension == ".mp4" || extension == ".m4v" || extension == ".3gp")
            {
                var mp4Parser = new MP4Parser(fileName);
                var mp4SubtitleTracks = mp4Parser.GetSubtitleTracks();
                if (mp4SubtitleTracks.Count == 0)
                {
                    Active = false;
                }
                else //if (mp4SubtitleTracks.Count == 1)
                {
                    selectedSubtitle = LoadMp4SubtitleForSync(mp4SubtitleTracks[0]);
                }
            }
        }

        private void CreateEnhancedSubTitles(Subtitle selectedSubs, SubtitleFormat format, Size playerControllerVideoSize)
        {
            try
            {
                var header = SubtitleFormatReader.ParseHeader(selectedSubs, format);
                if (header.PlayResX == null && header.PlayResY == null)
                {
                    header.PlayResX = (int)playerControllerVideoSize.Width;
                    header.PlayResY = (int)playerControllerVideoSize.Height;
                }
                else if (header.PlayResY == null)
                {
                    header.PlayResY = (int) (header.PlayResX * playerControllerVideoSize.Height / playerControllerVideoSize.Width);
                }
                else if (header.PlayResX == null)
                {
                    header.PlayResY = (int)(header.PlayResY * playerControllerVideoSize.Width / playerControllerVideoSize.Height);
                }
                
                for (int i = 0; i < selectedSubs.Paragraphs.Count; i++)
                {
                    var p = selectedSubs.Paragraphs[i];
                    indexToEnhancedParagraphs.Add(i, new EnhancedParagraph(header, p));
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
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

        public void Update(double position)
        {
            var index = selectedSubtitle.GetIndex(position);
            if (index >= 0)
            {
                subtitleLabel.Clear();

                while (true)
                {
                    var p = selectedSubtitle.GetParagraphOrDefault(index);
                    
                    if (p != null)
                    {
                        if (p.StartTime.TotalSeconds <= position)
                        {
                            subtitleLabel.Add(indexToEnhancedParagraphs[index]);
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                var w = window.UriPlayer.ActualWidth;
                subtitleLabel.Margin = new Thickness(w * 0.02f);
                subtitleLabel.FontSize = w / 30;
                subtitleLabel.Visibility = Visibility.Visible;
                subtitleLabel.ImageWidth = window.UriPlayer.ImageWidth;
                subtitleLabel.ImageHeight = window.UriPlayer.ImageHeight;
            }
            else
            {
                subtitleLabel.Clear();
            }
        }
    }
}
