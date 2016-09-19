#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;

#endregion

namespace Imp.Base.ListLogic
{
    public class PlaylistThreadedUpdater
    {
        #region Static Fields and Constants

        private const int updateBatchCount = 100;
        private const int updateBatchLast = updateBatchCount - 1;

        #endregion

        #region Fields

        public ConcurrentQueue<PlaylistItem> FinishedAdding = new ConcurrentQueue<PlaylistItem>();
        public HashSet<string> ExistingPaths = new HashSet<string>();
        public Action CallForFinalAddAction;
        private readonly ConcurrentQueue<PlaylistItem> itemsToAdd = new ConcurrentQueue<PlaylistItem>();
        private readonly ConcurrentQueue<string> pathsToRemove = new ConcurrentQueue<string>();
        private readonly Dispatcher dispatcher;
        private readonly object addLock = new object();
        private Thread updaterThread;
        private bool adding = false;
        private bool clear;

        #endregion

        public PlaylistThreadedUpdater(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void Add(PlaylistItem item)
        {
            itemsToAdd.Enqueue(item);
            CallForUpdate();
        }

        public void Remove(string path)
        {
            pathsToRemove.Enqueue(path);
            CallForUpdate();
        }

        private void CallForUpdate()
        {
            lock (addLock)
            {
                if ((itemsToAdd.IsEmpty && pathsToRemove.IsEmpty) || adding) return;

                adding = true;
                updaterThread = new Thread(ThreadedUpdate);
                updaterThread.Start();
            }
        }

        private void ThreadedUpdate()
        {
            PlaylistItem item;
            while (!itemsToAdd.IsEmpty || !pathsToRemove.IsEmpty)
            {
                while (!pathsToRemove.IsEmpty)
                {
                    string path;
                    var success = pathsToRemove.TryDequeue(out path);
                    if (success) { ExistingPaths.Remove(path); }
                }

                if (!itemsToAdd.IsEmpty)
                {
                    var success = itemsToAdd.TryDequeue(out item);
                    if (success)
                    {
                        item.ReadFileData();
                        FinishedAdding.Enqueue(item);
                    }
                }


                if (FinishedAdding.Count % updateBatchCount == updateBatchLast)
                    dispatcher.Invoke(CallForFinalAddAction, DispatcherPriority.Background);
            }

            lock (addLock)
            {
                if (clear)
                {
                    while (FinishedAdding.TryDequeue(out item)) { }
                    clear = false;
                    adding = false;
                    return;
                }

                adding = false;
            }
            dispatcher.Invoke(CallForFinalAddAction, DispatcherPriority.Background);
        }

        /// <summary>
        /// Clear updater queues.
        /// </summary>
        public void Clear()
        {
            PlaylistItem item;
            while (itemsToAdd.TryDequeue(out item)) { }
            while (FinishedAdding.TryDequeue(out item)) { }

            lock (addLock)
            {
                if (adding) { clear = true; }
            }
        }
    }
}