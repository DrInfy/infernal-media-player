#region Usings

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Base;
using Base.ListLogic;

#endregion

namespace ImpControls.Lists
{
    public class PlaylistListBox : ImpListBox<PlaylistItem>
    {
        #region Helpers

        public delegate void LoadPlaylistItemEvent(PlaylistItem item);

        #endregion

        #region Fields

        private readonly ComparerPlayListItemName nameComparer = new ComparerPlayListItemName();
        private readonly ComparerPlayListItemDate dateComparer = new ComparerPlayListItemDate();
        private readonly ComparerPlayListItemPath pathComparer = new ComparerPlayListItemPath();
        private readonly ComparerPlayListItemRandom randomComparer = new ComparerPlayListItemRandom();
        private PlayListController playListController;

        #endregion

        #region Events

        public event LoadPlaylistItemEvent LoadPlaylistItem;

        #endregion

        public PlaylistListBox() : base(true, true)
        {
            playListController.LoadPlaylistItem += OpenItem;
            ItemsDragable = true;
        }

        /// <summary>
        /// Sets the dispatcher for playlist updater. This method needs to be called before using list.
        /// </summary>
        /// <param name="dispatcher"></param>
        public void SetDispatcher(Dispatcher dispatcher)
        {
            playListController.SetDispatcher(dispatcher);
        }

        public void PlayingThis(PlaylistItem item)
        {
            var index = playListController.PlayingThis(item);
            SeekTo(index);
        }

        /// <summary>
        /// Attempts to put the index specified to view. Values smaller than 0 are ignored.
        /// </summary>
        /// <param name="visibleIndex"></param>
        private void SeekTo(int visibleIndex)
        {
            if (visibleIndex < 0)
                return; // playing item is not currently visible

            if (visibleIndex < LowIndex || visibleIndex > HighIndex)
            {
                // playing item is not currently in view
                // put it in the center of the view
                LowIndex = visibleIndex - (HighIndex - LowIndex) / 2;
            }
        }

        protected override void CreateController(bool searchable, bool multiSelectable)
        {
            playListController = new PlayListController(searchable, multiSelectable);
            controller = playListController;
        }

        protected override void DrawText(int index, DrawingContext drawingContext, Brush brush)
        {
            if (ActualWidth - sStyle.ScrollbarWidth < 0)
                return;
            FormattedText formatText;

            if (playListController.IsPlaying(index))
            {
                formatText = FormatText("> " + (index + 1) + ". " + controller.GetText(index), ref brush);
                formatText.SetFontWeight(FontWeights.Bold);
            }
            else
            {
                formatText = FormatText((index + 1) + ". " + controller.GetText(index), ref brush);
            }

            drawingContext.DrawText(formatText, new Point(3, (index - LowIndex) * sStyle.RowHeight + 3));
        }

        public void OpenNext(LoopMode loopMode)
        {
            playListController.OpenNext(loopMode);
        }

        public void OpenPrev(LoopMode loopMode)
        {
            playListController.OpenPrev(loopMode);
        }

        public void OpenRandom()
        {
            playListController.OpenRandom();
        }

        public void OpenItem(PlaylistItem playlistItem)
        {
            LoadPlaylistItem?.Invoke(playlistItem);
        }

        public void Sort(FileSortMode fileSortMode)
        {
            switch (fileSortMode)
            {
                case FileSortMode.Name:
                    controller.Sort(nameComparer);
                    break;
                case FileSortMode.Date:
                    controller.Sort(dateComparer);
                    break;
                case FileSortMode.Random:
                    controller.Sort(randomComparer);
                    break;
                case FileSortMode.Path:
                    controller.Sort(pathComparer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("fileSortMode");
            }
        }
    }
}