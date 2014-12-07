using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Base
{
    public abstract class FileLoader<T>
    {
        protected readonly Dispatcher dispatcher;
        private volatile bool currentlyLoading;
        private string fileInQueue;
        private string fileInLoading;

        public event LoadedEventHandler Loaded;
        public delegate void LoadedEventHandler(T loadedFile);
        public delegate void LoadedBitmapEventHandler(T loadedFile);
        protected LoadedBitmapEventHandler LoadedMainEvent;

        public event LoadFailedEventHandler LoadFailed;
        public delegate void LoadFailedEventHandler(ImpError errorMessage);
        public delegate void LoadFailedMainEventHandler(ImpError errorMessage);
        protected LoadFailedMainEventHandler loadFailedMainEvent;

        private object loadLock = new object();

        public bool IsLoading { get { return currentlyLoading; } }

        public FileLoader(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            LoadedMainEvent = RaiseLoadedMainThread;
            loadFailedMainEvent = RaiseLoadFailedMainThread;
        }

        /// <summary>
        /// Call dispatcher to switch to main thread so we can call load success event
        /// </summary>
        /// <param name="loadedFile"></param>
        private void RaiseLoaded(T loadedFile)
        {
            if (Loaded != null)
            {
                dispatcher.BeginInvoke(LoadedMainEvent, DispatcherPriority.Normal, loadedFile);
            }
        }


        /// <summary>
        /// Using Main thread, raise the load success event
        /// </summary>
        /// <param name="loadedFile"></param>
        protected void RaiseLoadedMainThread(T loadedFile)
        {
            Loaded(loadedFile);
        }


        /// <summary>
        /// Call dispatcher to switch to main thread so we can call load failed event
        /// </summary>
        /// <param name="errorMessage"></param>
        public void RaiseLoadFailed(ImpError errorMessage)
        {
            if (LoadFailed != null)
                dispatcher.BeginInvoke(loadFailedMainEvent, DispatcherPriority.Normal, errorMessage);
        }

        /// <summary>
        /// Using Main thread, raise the load failed event
        /// </summary>
        /// <param name="errorMessage"></param>
        public void RaiseLoadFailedMainThread(ImpError errorMessage)
        {
            LoadFailed(errorMessage);
        }


        public void OpenFile(string path)
        {
            lock (loadLock)
            {
                if (currentlyLoading && string.Equals(fileInLoading, path))
                {
                    return;
                }

                fileInQueue = path;
            }
        }

        public void Update()
        {
            lock (loadLock)
            {

                if (!currentlyLoading && !string.IsNullOrEmpty(fileInQueue))
                {
                    fileInLoading = fileInQueue;
                    fileInQueue = string.Empty;
                    //StartLoad();

                    var thread = new Thread(StartLoad);
                    thread.Name = "file loader";
                    thread.Start();
                }
            }
        }

        private void StartLoad()
        {
            ImpError error;
            var file = Load(fileInLoading, out error);
            if (error == null)
                RaiseLoaded(file);
            else
            {
                RaiseLoadFailed(error);
            }
            currentlyLoading = false;
        }

        protected abstract T Load(string path, out ImpError error);
    }
}
