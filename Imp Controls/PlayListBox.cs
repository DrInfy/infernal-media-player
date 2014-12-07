using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Base;
using Base.ListLogic;

namespace ImpControls
{
    public class PlayListBox : ImpListBox<PlaylistItem>
    {
        public PlayListBox() : base(true, true)
        {
        }

        /// <summary> the item number that is playing according to item index </summary>
        protected int playing
        {
            get
            {
                for (int i = 0; i <= item.Count - 1; i++)
                {
                    // ia As PlaylistItem In item
                    if (pItem[i].Playing)
                    {
                        return i;
                    }
                }
                return -1;
            }
            set
            {
                int i = 0;
                foreach (Selectable<PlaylistItem> selectable in item)
                {
                    if (i == value)
                    {
                        selectable.Content.Playing = true;
                    }
                    else
                    {
                        selectable.Content.Playing = false;
                    }
                    i += 1;
                }

                InvalidateVisual();
            }
        }

        public PlaylistItem pItem
        {
            get { return (PlaylistItem)item(index); }
        }


        public LoopMode Looping { get; set; }

        public event OpenFileEventHandler OpenFile;

        public delegate void OpenFileEventHandler(object sender, PlaylistItem path);


        public void OpenSelected()
        {
            if (SelectedIndex > -1)
            {
                playing = SelectedIndex;
                GoPlay();
                //Else
                //    OpenNext()
            }
        }

        public void OpenRandom()
        {
            int index = 0;
            Random yy = new Random();
            index = yy.NextDouble() * (Findlist.Count - 1);
            index = Math.Max(Math.Min(index, Findlist.Count - 1), 0);

            SelectedIndex = index;
            playing = index;
            GoPlay();
        }


        public void OpenPrev()
        {
            bool found = false;
            if (Findlist.Count > 1)
            {
                for (int i = 1; i <= Findlist.Count - 1; i++)
                {
                    if (Findlist[i] == playing)
                    {
                        playing = Findlist[i - 1];
                        found = true;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }

                if (!found && Looping == LoopMode.LoopAll && playing == Findlist[0])
                {
                    playing = Findlist[Findlist.Count - 1];
                    found = true;
                }
                else if (!found && playing < item.Count - 1)
                {
                    playing -= 1;
                    found = true;

                }
                else if (!found && Looping == LoopMode.LoopAll && playing == 0)
                {
                    playing = Findlist[Findlist.Count - 1];
                    found = true;
                }

                if (found)
                {
                    GoPlay();
                }
                //ElseIf Looping = LoopMode.LoopAll AndAlso item.Count() > 0 And playing = 0 Then
                //    pItem(item.Count - 1).Playing = True
                //    playing = item.Count - 1
                //    GoPlay()
            }

        }

        public void OpenNext()
        {
            bool found = false;
            if (Findlist.Count > 1)
            {
                for (int i = 0; i <= Findlist.Count - 2; i++)
                {
                    if (Findlist[i] == playing)
                    {
                        playing = Findlist[i + 1];
                        found = true;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }

                if (!found && Looping == LoopMode.LoopAll && playing == Findlist[Findlist.Count - 1])
                {
                    playing = Findlist[0];
                    found = true;
                }
                else if (!found && playing < item.Count - 1)
                {
                    playing += 1;
                    found = true;

                }
                else if (!found && Looping == LoopMode.LoopAll && playing == item.Count - 1)
                {
                    playing = Findlist[0];
                    found = true;
                }

                if (found)
                {
                    GoPlay();
                }
                //ElseIf Looping = LoopMode.LoopAll AndAlso item.Count() > 0 And playing = item.Count - 1 Then
                //    playing = 0
                //    GoPlay()
            }
        }


        public bool OpenPath(string path)
        {
            int i = CheckPath(path);
            if (i > -1)
            {
                playing = i;
                GoPlay();
                return true;
            }
            return false;
        }


        public void OpenNewest()
        {
            if (0 < item.Count())
            {
                playing = item.Count() - 1;
                GoPlay();
            }
        }


        private void GoPlay()
        {
            if (OpenFile != null)
            {
                OpenFile(this, item(playing));
            }
            dynamic playingIndex = playing;
            for (int i = 0; i <= Findlist.Count - 1; i++)
            {
                if (Findlist[i] == playingIndex)
                {
                    SelectOne(i, true);

                    // If playing item not shown then
                    // Move list to show item that is playing
                    if (!this.IsMouseOver)
                    {
                        if (playingIndex < LowIndex | playingIndex > HighIndex)
                        {
                            LowIndex = playingIndex - (HighIndex - LowIndex) / 2;
                        }
                    }

                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            Activate_drawing();
        }


        public int FindPlaying()
        {
            for (int i = 0; i <= Findlist.Count - 1; i++)
            {
                if (pItem[i].Playing)
                {
                    return i;
                }
            }
            return -1;
        }



        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (this.ActualWidth <= 0 | this.ActualHeight <= 0)
            {
                return;
            }
            base.OnRender(drawingContext);
            if (GetCount() > 0 && HighIndex - LowIndex < GetCount() - 1 & playing > -1)
            {
                Brush brush = default(Brush);
                //brush = New SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 100, 0, 0)) 'sStyle.BackPressedBrush
                brush = sStyle.PressedBrush;
                drawingContext.DrawLine(new Pen(brush, 1),
                                        new Point(this.ActualWidth - 0.5 - SCROLLBARWIDTH,
                                                  Math.Floor(this.ActualHeight * playing / (GetCount() - 1)) + 0.5),
                                        new Point(this.ActualWidth - 0.5,
                                                  Math.Floor(this.ActualHeight * playing / (GetCount() - 1)) + 0.5));
            }
        }


        protected override void DrawText(int index, System.Windows.Media.DrawingContext drawingContext, ref Brush brush)
        {
            if (this.ActualWidth - SCROLLBARWIDTH < 0)
                return;

            FormattedText text = FormatText(getText(index), brush);
            int fromborder = 0;

            if (pItem[Findlist[index]].Playing)
                text.SetFontWeight(FontWeights.Bold);

            if (string.IsNullOrEmpty(pItem[Findlist[index]].Duration))
            {
                drawingContext.DrawText(text, new Point(3, (index - LowIndex) * ISize + 3));
            }
            else
            {
                fromborder = 6 + pItem[Findlist[index]].Duration.Length * 5;
                text.MaxTextWidth -= fromborder;
                drawingContext.DrawText(text, new Point(3, (index - LowIndex) * ISize + 3));
                text = FormatText(pItem[Findlist[index]].Duration, brush);


                if ((item != null) && HighIndex - LowIndex < GetCount() - 1)
                {
                    fromborder += SCROLLBARWIDTH;
                }
                drawingContext.DrawText(text, new Point(this.ActualWidth - fromborder, (index - LowIndex) * ISize + 3));
            }

        }


        protected override Brush getBrush(int i, System.Windows.Media.DrawingContext drawingContext, Brush brush)
        {
            brush = base.getBrush(i, drawingContext, brush);
            if (pItem[GetCorrectI(i)].Playing)
            {
                if (pItem[GetCorrectI(i)].SelectedIndex)
                {
                    brush = sStyle.PressedBrush;
                }
                else if (i == MouseoverIndex)
                {
                    brush = sStyle.PressedBrush;
                }
                else
                {
                    brush = sStyle.MouseoverBrush;
                }
            }
            return brush;
        }


        public void SortList(bool byPath = false, bool random = false)
        {
            long lasttick = System.DateTime.Now.Ticks;

            string[] sortBy = new string[item.Count];

            if (random)
            {
                VBMath.Randomize();
                for (i = 0; i <= item.Count - 1; i++)
                {
                    sortBy[i] = VBMath.Rnd();
                }
            }
            else
            {
                if (byPath)
                {
                    for (i = 0; i <= item.Count - 1; i++)
                    {
                        sortBy[i] = pItem[i].FullPath;
                    }
                }
                else
                {
                    for (i = 0; i <= item.Count - 1; i++)
                    {
                        sortBy[i] = pItem[i].SmartName;
                    }
                }
            }

            ISelectable[] itemArray = null;
            itemArray = item.ToArray();

            System.Array.Sort(sortBy, itemArray);


            item = itemArray.ToList();

            UpdateItems();
        }
    }
}
