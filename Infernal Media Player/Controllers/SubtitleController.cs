using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Imp.DirectShow.Element;
using Imp.DirectShow.Subtitles;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;
using Nikse.SubtitleEdit.Core.ContainerFormats.Mp4;
using Nikse.SubtitleEdit.Core.ContainerFormats.Mp4.Boxes;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using Size = System.Windows.Size;

namespace Imp.Player.Controllers
{
    public class SubtitleController
    {
        public bool Active { get; private set; }
        private SubtitleStylized selectedSubtitle = new SubtitleStylized();
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
            this.subtitleElement.ImageWidthFunc = () => window.UriPlayer.ActualWidth;
            this.subtitleElement.ImageHeightFunc = () => window.UriPlayer.ActualHeight;
        }

        public void Clear()
        {
            this.lastIndices.Clear();
            this.selectedSubtitle.Paragraphs.Clear();
            this.Active = false;
            this.indexToEnhancedParagraphs.Clear();
            this.subtitleElement.Clear();
            this.subtitleElement.Visibility = Visibility.Hidden;
        }

        

//        public void Update(double position)
//        {
//            this.nextIndices.Clear();
//            for (int i = 0; i < this.selectedSubtitle.Paragraphs.Count; i++)
//            {
//                var p = this.selectedSubtitle.Paragraphs[i];

//                if (p.StartTime.TotalSeconds <= position
//                    && p.EndTime.TotalSeconds >= position)
//                {
//                    this.nextIndices.Add(i);
//                }
//            }

//#if DEBUG
//            if (true || this.nextIndices.Count != this.lastIndices.Count || this.nextIndices.Any(x => !this.lastIndices.Contains(x)))
//#else
//            if (nextIndices.Count != lastIndices.Count || nextIndices.Any(x => !lastIndices.Contains(x)))
//#endif
//            {
//                this.subtitleElement.ClearContent();
//                this.lastIndices.Clear();
//                foreach (var nextIndex in this.nextIndices)
//                {
//                    this.lastIndices.Add(nextIndex);
//                    this.subtitleElement.Add(this.indexToEnhancedParagraphs[nextIndex]);
//                }
//            }

//            this.subtitleElement.Visibility = this.nextIndices.Count > 0 ? Visibility.Visible : Visibility.Hidden;
//        }
    }
}
