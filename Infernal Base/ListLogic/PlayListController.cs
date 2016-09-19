#region Usings

using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Imp.Base.Libraries;

#endregion

namespace Imp.Base.ListLogic
{
    public class PlayListController : ListController<PlaylistItem>
    {
        #region Helpers

        public delegate void LoadPlaylistItemEvent(PlaylistItem item);

        #endregion

        #region Fields

        private PlaylistThreadedUpdater updater;

        #endregion

        #region Properties

        public string PlayingFullPath { get; set; }

        #endregion

        #region Events

        public event LoadPlaylistItemEvent LoadPlaylistItem;

        #endregion

        public PlayListController(bool searchable, bool multiSelectable)
            : base(searchable, multiSelectable) {}

        public void SetDispatcher(Dispatcher dispatcher)
        {
            updater = new PlaylistThreadedUpdater(dispatcher);
            updater.CallForFinalAddAction += FinalAdd;
        }

        private void FinalAdd()
        {
            while (!updater.FinishedAdding.IsEmpty)
            {
                PlaylistItem item;
                var success = updater.FinishedAdding.TryDequeue(out item);
                if (success && !string.IsNullOrEmpty(PlayingFullPath))
                {
                    if (string.CompareOrdinal(item.FullPath, PlayingFullPath) == 0)
                    {
                        item.Playing = true;
                    }
                }
            }
            //UpdateItems();
            OnListSizeChanged(true);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void AddItem(PlaylistItem item)
        {
            if (!updater.ExistingPaths.Contains(item.FullPath))
            {
                updater.ExistingPaths.Add(item.FullPath);
                items.Add(new Selectable<PlaylistItem>(item));
                updater.Add(item);
            }
        }

        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="list">The list.</param>
        public override void AddItems(ICollection<PlaylistItem> list)
        {
            foreach (var item in list)
                AddItem(item);
        }

        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="array">The array.</param>
        public override void AddItems(PlaylistItem[] array)
        {
            foreach (var item in array)
                AddItem(item);
        }

        public override void Clear()
        {
            foreach (var item in items)
            {
                updater.Remove(item.Content.FullPath);
                updater.Clear();
            }
            base.Clear();
        }

        protected override void RemoveItem(int index)
        {
            updater.Remove(items[index].Content.FullPath);
            base.RemoveItem(index);
        }

        public int PlayingThis(PlaylistItem item)
        {
            if (item == null)
            {
                PlayingFullPath = null;

                foreach (var playListItem in items)
                {
                    playListItem.Content.Playing = false;
                }

                return -1;
            }
            PlayingFullPath = item.FullPath;

            var index = -1;
            for (var i = 0; i < items.Count; i++)
            {
                if (!items[i].Content.FullPath.Equals(item.FullPath))
                    items[i].Content.Playing = false;
                else
                {
                    items[i].Content.Playing = true;
                    if (SearchActive)
                    {
                        for (var j = 0; j < findlist.Length; j++)
                        {
                            if (findlist[j] == i)
                            {
                                index = j;
                                break;
                            }
                        }
                        if (index >= 0)
                        {
                            Select(SelectionMode.One, index);
                        }
                    }
                    else
                    {
                        Select(SelectionMode.One, i);
                        index = i;
                    }
                }
            }
            if (!item.Playing)
            {
                OnListSelectionChanged();
                item.Playing = true;
            }

            OnListSelectionChanged();
            return index;
        }

        public bool IsPlaying(int visibleIndex)
        {
            return SearchActive ? items[findlist[visibleIndex]].Content.Playing : items[visibleIndex].Content.Playing;
        }

        /// <summary>
        /// Open next item in the list, using visible items first if using search
        /// </summary>
        /// <param name="loopMode"></param>
        public void OpenNext(LoopMode loopMode)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Content.Playing)
                {
                    if (SearchActive)
                    {
                        for (var j = 0; j < findlist.Length; j++)
                        {
                            if (i == findlist[j])
                            {
                                // item was part of found list, let's open next in shown list
                                if (j >= findlist.Length - 1)
                                {
                                    // last item in the list
                                    if (loopMode == LoopMode.LoopAll)
                                    {
                                        RaiseLoadEvent(items[findlist[0]].Content);
                                    }
                                    // stop playing
                                }
                                else
                                {
                                    RaiseLoadEvent(items[findlist[++j]].Content);
                                }
                                return; // item was found, no need to search further
                            }
                        }

                        // item was not in the visible list, play next item from overall then instead
                        PlayNextListItem(i, loopMode);
                    }
                    else
                    {
                        PlayNextListItem(i, loopMode);
                    }
                    return; // item was found, no need to search further
                }
            }

            if (items.Count <= 0) return;

            if (SearchActive && findlist.Length > 0)
                RaiseLoadEvent(items[findlist[0]].Content);
            else
                RaiseLoadEvent(items[0].Content);
        }

        /// <summary>
        /// Plays the next item from the overall list, ignoring any and all searches
        /// </summary>
        /// <param name="i"></param>
        /// <param name="loopMode"></param>
        private void PlayNextListItem(int i, LoopMode loopMode)
        {
            if (i >= items.Count - 1)
            {
                // last item in the list
                if (loopMode == LoopMode.LoopAll)
                {
                    RaiseLoadEvent(items[0].Content);
                }
            }
            else
            {
                RaiseLoadEvent(items[++i].Content);
            }
        }

        /// <summary>
        /// Open previous item in the list, using visible items first if using search
        /// </summary>
        /// <param name="loopMode"></param>
        public void OpenPrev(LoopMode loopMode)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Content.Playing)
                {
                    if (SearchActive)
                    {
                        for (var j = 0; j < findlist.Length; j++)
                        {
                            if (i == findlist[j])
                            {
                                // item was part of found list, let's open next in shown list
                                if (j <= 0)
                                {
                                    // last item in the list
                                    if (loopMode == LoopMode.LoopAll)
                                    {
                                        RaiseLoadEvent(items[findlist[findlist.Length - 1]].Content);
                                    }
                                    // stop playing
                                }
                                else
                                {
                                    RaiseLoadEvent(items[findlist[--j]].Content);
                                }
                                return; // item was found, no need to search further
                            }
                        }

                        // item was not in the visible list, play next item from overall then instead
                        PlayPreviousListItem(i, loopMode);
                    }
                    else
                    {
                        PlayPreviousListItem(i, loopMode);
                    }
                    return; // item was found, no need to search further
                }
            }
        }

        /// <summary>
        /// Plays the next item from the overall list, ignoring any and all searches
        /// </summary>
        /// <param name="i"></param>
        /// <param name="loopMode"></param>
        private void PlayPreviousListItem(int i, LoopMode loopMode)
        {
            if (i <= 0)
            {
                // last item in the list
                if (loopMode == LoopMode.LoopAll)
                {
                    RaiseLoadEvent(items[items.Count - 1].Content);
                }
            }
            else
            {
                RaiseLoadEvent(items[--i].Content);
            }
        }

        private void RaiseLoadEvent(PlaylistItem playlistItem)
        {
            if (LoadPlaylistItem != null)
            {
                LoadPlaylistItem.Invoke(playlistItem);
            }
        }

        public void OpenRandom()
        {
            if (SearchActive && findlist.Length > 0)
            {
                var playIndex = (int) Math.Round(LibImp.Rnd.NextDouble() * findlist.Length - 0.49999999);
                if (playIndex >= findlist.Length)
                    playIndex = findlist.Length - 1;
                RaiseLoadEvent(items[findlist[playIndex]].Content);
            }
            else if (items.Count > 0)
            {
                var playIndex = (int) Math.Round(LibImp.Rnd.NextDouble() * items.Count - 0.49999999);
                if (playIndex >= items.Count)
                    playIndex = items.Count - 1;
                RaiseLoadEvent(items[playIndex].Content);
            }
        }
    }
}