#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;

#endregion

namespace Base.ListLogic
{
    public class PlaylistThreadedUpdater
    {
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
                if ((!itemsToAdd.IsEmpty || !pathsToRemove.IsEmpty) && !adding)
                {
                    adding = true;
                    updaterThread = new Thread(ThreadedUpdate);
                    updaterThread.Start();
                }
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
                    if (success)
                        ExistingPaths.Remove(path);
                }

                if (!itemsToAdd.IsEmpty)
                {
                    var success = itemsToAdd.TryDequeue(out item);
                    if (success)
                    {
                        //if (!ExistingPaths.Contains(item.FullPath))
                        //{

                        item.ReadFileData();
                        FinishedAdding.Enqueue(item);
                        //}
                    }
                }

                if (FinishedAdding.Count % 100 == 99)
                    dispatcher.Invoke(CallForFinalAddAction, DispatcherPriority.Background);
            }

            lock (addLock)
            {
                if (clear == true)
                {
                    while (FinishedAdding.TryDequeue(out item)) { }
                    clear = false;
                    adding = false;
                    return;
                }
                else
                {
                    adding = false;
                }
            }
            dispatcher.Invoke(CallForFinalAddAction, DispatcherPriority.Background);
        }

        public void Clear()
        {
            PlaylistItem item;
            while (itemsToAdd.TryDequeue(out item)) { }
            while (FinishedAdding.TryDequeue(out item)) { }

            lock (addLock)
            {
                if (adding)
                    clear = true;
            }
        }
    }
}