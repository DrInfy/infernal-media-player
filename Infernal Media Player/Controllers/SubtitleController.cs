using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Base.Controllers;
using Base.Subtitles;
using Imp.Controls;
using MediaPlayer.Subtitles;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;
using Nikse.SubtitleEdit.Core.ContainerFormats.Mp4;
using Nikse.SubtitleEdit.Core.ContainerFormats.Mp4.Boxes;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using Size = System.Windows.Size;

namespace Imp.Controllers
{
    public class SubtitleController
    {
        public bool Active { get; private set; }
        private Subtitle selectedSubtitle = new Subtitle();
        private readonly SubtitleElement subtitleElement;
        private readonly MainWindow window;
        private SubtitleFormat subtitleFormat;
        private Dictionary<int, EnhancedParagraph> indexToEnhancedParagraphs = new Dictionary<int, EnhancedParagraph>();
        private List<int> lastIndices = new List<int>();
        private List<int> nextIndices = new List<int>();
        public SubtitleController(MainWindow window)
        {
            this.subtitleElement = window.Subtitles;
            this.window = window;
            subtitleElement.ImageWidthFunc = () => window.UriPlayer.ActualWidth;
            subtitleElement.ImageHeightFunc = () => window.UriPlayer.ActualHeight;
        }

        public void Clear()
        {
            lastIndices.Clear();
            selectedSubtitle.Paragraphs.Clear();
            Active = false;
            indexToEnhancedParagraphs.Clear();
            subtitleElement.Clear();
            subtitleElement.Visibility = Visibility.Hidden;
        }

        public void LoadSubtitles(string fileName, string extension, Size playerControllerVideoSize)
        {
            Clear();

            if (extension == ".mkv" || extension == ".mks")
            {
                using (var matroska = new MatroskaFile(fileName))
                {
                    if (matroska.IsValid)
                    {
                        if (matroska.ReadSubtitlesAndTracks())
                        {
                            var subtitleList = matroska.Subtitles;
                            if (subtitleList.Count == 0)
                            {
                                Active = false;
                            }
                            else
                            {
                                Active = true;
                                LoadFonts(matroska);

                                var selectedSubs = subtitleList.First();
                                var mkvSubs = matroska.GetSubtitle(selectedSubs.TrackNumber, null);

                                selectedSubtitle.Paragraphs.Clear();
                                subtitleFormat = Utilities.LoadMatroskaTextSubtitle(selectedSubs, matroska, mkvSubs,
                                    selectedSubtitle);
                                subtitleElement.Visibility = Visibility.Visible;

                                CreateEnhancedSubTitles(selectedSubtitle, subtitleFormat, playerControllerVideoSize);
                            }
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

        private void LoadFonts(MatroskaFile matroska)
        {
            try
            {
                var attachments = matroska.Attachments;
                Dictionary<string, List< MatroskaAttachment>> fontPackageDictionary = new Dictionary<string, List<MatroskaAttachment>>();
                foreach (var attachment in attachments)
                {
                    string familyName;
                    FontLoader.SaveToDisc(attachment.Data, attachment.Name);
                }

                var result = FontLoader.LoadFromDisc();

                foreach (var fontFamily in result)
                {
                    subtitleElement.AddFont(fontFamily.Key, fontFamily.Value);
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
            catch (Exception ex )
            {
                Debugger.Break();
                
            }
            
        }


        public void LoadFont(Byte[] fontData)
        {
            throw new NotImplementedException();
            //GlyphTypeface
            //var writer = new ResourceWriter();
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
                
                nextIndices.Clear();

                while (true)
                {
                    var p = selectedSubtitle.GetParagraphOrDefault(index);
                    
                    if (p != null)
                    {
                        if (p.StartTime.TotalSeconds <= position)
                        {
                            nextIndices.Add(index);
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

                //if (nextIndices.Count != lastIndices.Count || nextIndices.Any(x => !lastIndices.Contains(x)))
                {
                    subtitleElement.ClearContent();
                    lastIndices.Clear();
                    foreach (var nextIndex in nextIndices)
                    {
                        lastIndices.Add(nextIndex);
                        subtitleElement.Add(indexToEnhancedParagraphs[nextIndex]);
                    }
                }
                
                subtitleElement.Visibility = nextIndices.Count > 0 ? Visibility.Visible : Visibility.Hidden;
                
            }
            else
            {
                subtitleElement.ClearContent();
            }
        }
    }
}
