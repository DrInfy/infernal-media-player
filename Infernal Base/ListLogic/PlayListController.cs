using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using Base.Libraries;

namespace Base.ListLogic
{
    public class PlayListController : ListController<PlaylistItem>
    {
        public string PlayingFullPath { get; set; }

        private PlaylistThreadedUpdater updater;

        #region Delegates and Events

        public delegate void LoadPlaylistItemEvent(PlaylistItem item);

        public event LoadPlaylistItemEvent LoadPlaylistItem;

        #endregion


        public PlayListController(bool searchable, bool multiSelectable)
            : base(searchable, multiSelectable)
        {

        }


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
                //    items.Add(new Selectable<PlaylistItem>(item));
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
            //base.AddItem(item);
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
            foreach (PlaylistItem item in list)
                AddItem(item);
        }


        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="array">The array.</param>
        public override void AddItems(PlaylistItem[] array)
        {
            foreach (PlaylistItem item in array)
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


        public override void RemoveSelected()
        {
            if (multiSelectable)
            {
                bool removed = false;
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    if (items[i].Selected)
                    {
                        updater.Remove(items[i].Content.FullPath);
                        items.RemoveAt(i);

                        if (selectedIndex == i)
                            selectedIndex = -1;
                        else if (selectedIndex > i)
                            selectedIndex--;

                        removed = true;
                    }
                }
                if (removed)
                    OnListSizeChanged(false);
            }
            else
            {
                if (selectedIndex >= 0)
                {
                    updater.Remove(items[selectedIndex].Content.FullPath);
                    items.RemoveAt(selectedIndex);
                    selectedIndex = -1;
                    OnListSizeChanged(false);
                }
            }
        }


        public int PlayingThis(PlaylistItem item)
        {
            if (item == null)
            {
                PlayingFullPath = null;
                return -1;
            }
            PlayingFullPath = item.FullPath;

            int index = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].Content.FullPath.Equals(item.FullPath))
                    items[i].Content.Playing = false;
                else
                {
                    items[i].Content.Playing = true;
                    if (SearchActive)
                    {

                        for (int j = 0; j < findlist.Length; j++)
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
            if (SearchActive)
            {
                return items[findlist[visibleIndex]].Content.Playing;
            }
            return items[visibleIndex].Content.Playing;
        }


        /// <summary>
        /// Open next item in the list, using visible items first if using search
        /// </summary>
        /// <param name="loopMode"></param>
        public void OpenNext(LoopMode loopMode)
        {

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Content.Playing)
                {
                    if (SearchActive)
                    {
                        for (int j = 0; j < findlist.Length; j++)
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
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Content.Playing)
                {
                    if (SearchActive)
                    {
                        for (int j = 0; j < findlist.Length; j++)
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
                int playIndex = (int)Math.Round(LibImp.Rnd.NextDouble() * findlist.Length - 0.49999999);
                if (playIndex >= findlist.Length)
                    playIndex = findlist.Length - 1;
                RaiseLoadEvent(items[findlist[playIndex]].Content);
            }
            else if (items.Count > 0)
            {
                int playIndex = (int)Math.Round(LibImp.Rnd.NextDouble() * items.Count - 0.49999999);
                if (playIndex >= items.Count)
                    playIndex = items.Count - 1;
                RaiseLoadEvent(items[playIndex].Content);
            }
        }
    }
}