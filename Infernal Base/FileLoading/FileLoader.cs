#region Usings

using System.Threading;
using System.Windows.Threading;
using Edge.Tools.Threading;

#endregion

namespace Imp.Base.FileLoading
{
    public abstract class FileLoader<T>
    {
        #region Helpers

        //public delegate void LoadedBitmapEventHandler(T loadedFile);

        public delegate void LoadedEventHandler(T loadedFile);

        public delegate void LoadFailedEventHandler(ImpError errorMessage);

        public delegate void LoadFailedMainEventHandler(ImpError errorMessage);

        #endregion

        #region Fields

        protected readonly Dispatcher dispatcher;
        protected LoadedEventHandler LoadedMainEvent;
        protected LoadFailedMainEventHandler loadFailedMainEvent;
        private readonly object loadLock = new object();
        private volatile bool currentlyLoading;
        private string fileInQueue;
        private string fileInLoading;
        private AbortableTask abortableTask;

        #endregion

        #region Properties

        public bool IsLoading => this.currentlyLoading;

        #endregion

        #region Events

        public event LoadedEventHandler Loaded;
        public event LoadFailedEventHandler LoadFailed;

        #endregion

        protected FileLoader(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.LoadedMainEvent = RaiseLoadedMainThread;
            this.loadFailedMainEvent = RaiseLoadFailedMainThread;
        }

        /// <summary>
        /// Call dispatcher to switch to main thread so we can call load success event
        /// </summary>
        /// <param name="loadedFile"></param>
        private void RaiseLoaded(T loadedFile)
        {
            if (Loaded != null)
            {
                this.dispatcher.BeginInvoke(this.LoadedMainEvent, DispatcherPriority.Normal, loadedFile);
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
                this.dispatcher.BeginInvoke(this.loadFailedMainEvent, DispatcherPriority.Normal, errorMessage);
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
            lock (this.loadLock)
            {
                if (this.currentlyLoading && string.Equals(this.fileInLoading, path))
                {
                    return;
                }

                this.fileInQueue = path;
            }
        }

        public void Update()
        {
            if (this.currentlyLoading) return;

            lock (this.loadLock)
            {
                if (this.currentlyLoading || string.IsNullOrEmpty(this.fileInQueue)) return;

                this.fileInLoading = this.fileInQueue;
                this.fileInQueue = string.Empty;

                this.abortableTask = new AbortableTask();
                this.abortableTask.Start(StartLoad, (x) => RaiseLoadFailed(new ImpError(ErrorType.FailedToOpenFile, x.Message)), "FileLoad");
                //var thread = new Thread(StartLoad);
                //thread.Name = "file loader";
                //thread.Start();

            }
        }

        private void StartLoad()
        {
            ImpError error;
            var file = Load(this.fileInLoading, out error);
            if (error == null)
                RaiseLoaded(file);
            else
            {
                RaiseLoadFailed(error);
            }
            this.currentlyLoading = false;
        }

        public void Abort()
        {
            if (this.abortableTask != null && !this.abortableTask.WasAborted && !this.abortableTask.IsCompleted)
            {
                lock (this.abortableTask)
                {
                    this.abortableTask.Abort();
                }
            }
        }

        protected abstract T Load(string path, out ImpError error);
    }
}