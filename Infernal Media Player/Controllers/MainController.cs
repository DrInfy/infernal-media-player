using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Base;
using Base.Commands;
using Base.Controllers;
using Base.FileData;
using Base.Libraries;
using Base.ListLogic;
using Imp.Image;
using Imp.Libraries;
using ImpControls.Controllers;
using MediaPlayer;
using MediaPlayer.Player;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Imp.Controllers
{
    public partial class MainController : BaseController
    {
        private static readonly OperatingSystem osInfo = Environment.OSVersion;
        public readonly Popup ContentMenu;

        private readonly ImageLoader imageLoader;
        private readonly MediaLoader mediaLoader;
        private readonly MainWindow window;
        private BitmapSource currentImage;

        public MainController(MainWindow window)
        {
            ToolTipService.ShowDurationProperty.OverrideMetadata(
                typeof (DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            EventC = new EventController(window.Dispatcher, window.LabelEvent, window.LabelTopic);
            PanelC = new PanelController(window);

            var mediaController = new MediaController(
                window.UriPlayer,
                window.PlayerBottom.ButtonPlay,
                window.PlayerBottom.ButtonMute,
                window.PlayerBottom.ButtonLoop,
                EventC);

            Initialize(EventC, PanelC, mediaController);

            this.window = window;
            imageLoader = new ImageLoader(window.Dispatcher);
            imageLoader.Loaded += ImageLoaded;
            imageLoader.LoadFailed += LoadFailed;

            mediaLoader = new MediaLoader(window.Dispatcher);
            mediaLoader.Loaded += MediaLoaded;
            mediaLoader.LoadFailed += LoadFailed;

            ContentMenu = window.ContentMenu;

            ApplySettings();
        }

        public override bool Focused
        {
            get { return !window.PanelOpen.TextBoxFind.IsFocused && !window.PanelPlaylist.TextBoxFind.IsFocused; }
        }

        public override bool Selected
        {
            get { return window.IsActive && window.ExtWindowState != ExtWindowState.Minimized; }
        }

        public override bool IsMostRecentInstance
        {
            get { return ImpMessaging.LastActive; }
        }

        protected void ApplySettings()
        {
            MediaC.SetVolume(Settings.Volume);
            MediaC.LoopMode = Settings.LastLoopMode;
            if (MediaC.LoopMode == LoopMode.LoopAll)
                window.PlayerBottom.ButtonLoop.CurrentState = 1;
            window.PanelOpen.ButtonFilterVideo.CurrentState =
                (Settings.LastFileTypes & FileTypes.Videos) == FileTypes.Videos ? 1 : 0;
            window.PanelOpen.ButtonFilterMusic.CurrentState =
                (Settings.LastFileTypes & FileTypes.Music) == FileTypes.Music ? 1 : 0;
            window.PanelOpen.ButtonFilterPictures.CurrentState =
                Settings.LastFileTypes.HasFlag(FileTypes.Pictures) ? 1 : 0;
            window.PanelOpen.ButtonFilterPlaylist.CurrentState =
                (Settings.LastFileTypes & FileTypes.Playlist) == FileTypes.Playlist ? 1 : 0;

            window.PanelOpen.GetFilters();
        }

        protected override void UpdateSettings()
        {
            Settings.Volume = MediaC.Volume;
            Settings.LastLoopMode = MediaC.LoopMode;
            if (Settings.LastLoopMode == LoopMode.LoopOne)
                Settings.LastLoopMode = LoopMode.LoopAll;
            Settings.LastFileTypes = window.PanelOpen.GetFileTypes();
        }

        protected override void CloseWindows()
        {
            window.Close();
            window.WindowClosed = true;
        }

        protected override void OpenFile(PlaylistItem item)
        {
            if (item == null ||
                loadingItem != null && loadingItem.FullPath == item.FullPath
                || playingItem != null && playingItem.FullPath == item.FullPath)
            {
                if (item == null)
                {
                    EventC.SetEvent(new EventText("Cannot play item that doesn't exist"));
                    return;
                }
                if (mediaLoader.IsLoading || mediaLoader.IsLoading)
                {
                    EventC.SetEvent(new EventText("Already loading that item"));
                    return;
                }
                EventC.SetEvent(new EventText("Already playing that item"));
                return;
            }

            //window.UriPlayer.com
            if (window.UriPlayer.Controller != null)
            {
                window.UriPlayer.Controller.Command(MediaCommand.Close);
                MediaC.MediaClosed();
            }

            //MediaC.Stop();
            EventC.SetTitle("Loading...");
            window.Title = "Loading...";

            window.PanelPlaylist.ListPlaylist.PlayingThis(item);

            loadingItem = item;

            //window.PanelPlaylist.ListPlaylist.PlayingThis(item);

            if (item.FileType == FileTypes.Pictures)
            {
                imageLoader.OpenFile(item.FullPath);
            }
            else
            {
                mediaLoader.OpenFile(item.FullPath);
            }
        }

        private void LoadFailed(ImpError error)
        {
            EventC.ShowError(error);
            MediaC.MediaClosed();

            playingItem = null;
            loadingItem = null;

            if (window.UriPlayer.Controller != null)
            {
                window.UriPlayer.Controller.Command(MediaCommand.Close);
            }
            window.ImageViewer.Source = null;
            //playingItem = itemOnPlayer;
            //window.PanelPlaylist.ListPlaylist.PlayingThis(playingItem);
            //if (playingItem != null)
            //{
            //    SetPlayingTitle();    
            //}
            //else
            //{
            ResetTitle();
            //}
        }

        private void ResetTitle()
        {
            EventC.SetTitle("");
            window.Title = "Infernal Media Player";
        }

        private void ImageLoaded(BitmapSource bitmap)
        {
            playingItem = loadingItem;
            itemOnPlayer = playingItem;
            window.PanelPlaylist.ListPlaylist.PlayingThis(playingItem);
            window.ImageViewer.Visibility = Visibility.Visible;
            window.UriPlayer.Visibility = Visibility.Hidden;
            window.LogoViewer.Visibility = Visibility.Hidden;
            window.UriPlayer.Clear();
            currentImage = bitmap;

            window.ImageViewer.Source = bitmap;
            window.ImageViewer.Stretch = Stretch.Uniform;
            window.ImageViewer.StretchDirection = StretchDirection.Both;

            SetPlayingTitle();
        }

        private void SetPlayingTitle()
        {
            EventC.SetTitle(playingItem.Name);
            window.Title = playingItem.Name;
        }

        private void MediaLoaded(PlayerController playerController)
        {
            playingItem = loadingItem;
            itemOnPlayer = playingItem;

            window.UriPlayer.Controller = playerController;
            if (window.UriPlayer.HasVideo)
            {
                window.ImageViewer.Visibility = Visibility.Hidden;
                window.UriPlayer.Visibility = Visibility.Visible;
                window.LogoViewer.Visibility = Visibility.Hidden;
            }
            else
            {
                window.ImageViewer.Visibility = Visibility.Hidden;
                window.UriPlayer.Visibility = Visibility.Hidden;
                window.LogoViewer.Visibility = Visibility.Visible;
            }

            window.PanelPlaylist.ListPlaylist.PlayingThis(playingItem);

            SetPlayingTitle();
            EventC.SetEvent(new EventText(">" + playingItem.Name));
            MediaC.MediaOpened();
        }

        protected override void OpenFile(FileImpInfo fileInfo)
        {
            var item = new PlaylistItem(fileInfo);
            window.PanelPlaylist.ListPlaylist.AddToList(item);

            OpenFile(item);
        }

        protected override void CreateContextMenu(Point cursorPositionInDesktop, List<ImpTextAndCommand> cmdList)
        {
            Rect area;

            if (window.ExtWindowState != ExtWindowState.Fullscreen)
            {
                area = LibImp.GetWorkArea(window);
            }
            else
            {
                area = LibImp.GetMonitorArea(window);
            }

            window.MenuList.SetList(cmdList);

            Size size = window.MenuList.DesiredSize();
            ContentMenu.Height = size.Height;
            ContentMenu.Width = size.Width;

            //ContentMenu.HorizontalOffset = -20;
            //ContentMenu.VerticalOffset = -5;
            ContentMenu.HorizontalOffset = Math.Max(Math.Min(cursorPositionInDesktop.X - 20, area.Right - size.Width),
                area.Left);
            ContentMenu.VerticalOffset = Math.Max(Math.Min(cursorPositionInDesktop.Y - 5, area.Bottom - size.Height),
                area.Top);
            ContentMenu.IsOpen = true;
        }

        protected override void RemoveSelectedPath()
        {
            string path = window.PanelOpen.ListPlaces.GetSelected().Value;
            if (!Settings.CustomPaths.Contains(path))
            {
                EventC.SetEvent(new EventText("Path cannot be removed", 1, EventType.Delayed));
                return;
            }
            Settings.CustomPaths.Remove(path);
            window.PanelOpen.Refresh(this);
        }

        protected override void DeleteOpenFolder()
        {
            // TODO: implement this
        }

        protected override void AddSelectedFolderToPaths()
        {
            string path = window.PanelOpen.ListDirectories.GetSelected().Value;
            if (StringHandler.IsSpecialFolder(path)) return;
            if (Settings.CustomPaths.Contains(path)) return;
            Settings.CustomPaths.Add(path);
            window.PanelOpen.Refresh(this);
        }

        protected override void AddSelectedFolderFolders()
        {
            window.PanelOpen.PrepareFolderLoader(
                new DirectoryLoadOptions(window.PanelOpen.ListDirectories.GetSelected().Value,
                    SearchOption.AllDirectories,
                    window.PanelOpen.GetFileTypes(), window.PanelOpen.TextBoxFind.Text));
        }

        protected override void AddSelectedFolderFiles()
        {
            window.PanelOpen.PrepareFolderLoader(
                new DirectoryLoadOptions(window.PanelOpen.ListDirectories.GetSelected().Value,
                    SearchOption.TopDirectoryOnly,
                    window.PanelOpen.GetFileTypes(), window.PanelOpen.TextBoxFind.Text));
        }

        protected override void PlaySelectedFolderFiles()
        {
            Exec(ImpCommand.ClearPlaylist);
            window.PanelOpen.PrepareFolderLoader(
                new DirectoryLoadOptions(window.PanelOpen.ListDirectories.GetSelected().Value,
                    SearchOption.AllDirectories,
                    window.PanelOpen.GetFileTypes(), window.PanelOpen.TextBoxFind.Text) {PlayFirstFile = true});
        }

        protected override void RequestDeleteOpenFiles()
        {
            List<FileImpInfo> items = window.PanelOpen.ListFiles.GetSelectedList();
            var playlistItems = new List<PlaylistItem>(items.Count);
            var paths = new List<string>(items.Count);

            foreach (FileImpInfo fileImpInfo in items)
            {
                paths.Add(fileImpInfo.Path);
                playlistItems.Add(new PlaylistItem(fileImpInfo));
            }

            if (PopupDeleteWindow(paths, playlistItems))
                window.PanelOpen.ListFiles.Refresh();
        }

        protected override void AddSelectedOpenFiles()
        {
            window.PanelOpen.ButtonAddSelected_Clicked(this);
        }

        protected override void PlaySelectedOpenFiles()
        {
            List<FileImpInfo> list = window.PanelOpen.ListFiles.GetSelectedList();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                    Exec(ImpCommand.OpenFile, list[i]);
                else
                    Exec(ImpCommand.AddFile, list[i]);
            }
        }

        protected override void RequestPlaylistFileDeletion()
        {
            List<PlaylistItem> items = window.PanelPlaylist.ListPlaylist.GetSelectedList();

            var paths = new List<string>(items.Count);

            foreach (PlaylistItem playlistItem in items)
                paths.Add(playlistItem.FullPath);

            PopupDeleteWindow(paths, items);
        }

        private bool PopupDeleteWindow(List<string> paths, List<PlaylistItem> items)
        {
            var deleteWindow = new DeleteItemsWindow(this, paths);
            deleteWindow.Owner = window;

            deleteWindow.SetStyles(window.Styling);
            if (deleteWindow.ShowDialog() == true)
            {
                PermanentlyDeleteFiles(items);
                return true;
            }
            return false;
        }

        protected override void OpenSelectedInExplorer()
        {
            PlaylistItem item = window.PanelPlaylist.ListPlaylist.GetSelected();
            if (item != null)
            {
                Process.Start("explorer.exe", string.Format("/select,\"{0}\"", item.FullPath));
            }
        }

        protected override void OpenSelectedInPlayList()
        {
            OpenFile(window.PanelPlaylist.ListPlaylist.GetSelected());
        }

        protected override void PlaylistSort(FileSortMode fileSortMode)
        {
            window.PanelPlaylist.ListPlaylist.Sort(fileSortMode);
        }

        protected override void RemoveSelectedFromPlaylist()
        {
            window.PanelPlaylist.ListPlaylist.RemoveSelected();
        }

        protected override void OpenRandom()
        {
            window.PanelPlaylist.ListPlaylist.OpenRandom();
        }

        public override void MediaEnded()
        {
            if (MediaC.LoopMode == LoopMode.LoopOne)
            {
                MediaC.SetPosition(0);
                MediaC.Play();
            }
            else
            {
                window.PanelPlaylist.ListPlaylist.OpenNext(MediaC.LoopMode);
            }
        }

        protected override void OpenPrev()
        {
            if (MediaC.LoopMode == LoopMode.LoopOne)
            {
                window.PanelPlaylist.ListPlaylist.OpenPrev(LoopMode.NoLoop);
            }
            else
            {
                window.PanelPlaylist.ListPlaylist.OpenPrev(MediaC.LoopMode);
            }
        }

        protected override void OpenNext()
        {
            if (MediaC.LoopMode == LoopMode.LoopOne)
            {
                window.PanelPlaylist.ListPlaylist.OpenNext(LoopMode.NoLoop);
            }
            else
            {
                window.PanelPlaylist.ListPlaylist.OpenNext(MediaC.LoopMode);
            }
        }

        protected override void ClearPlayList()
        {
            window.PanelPlaylist.ListPlaylist.ClearList();
        }

        protected override void AddFile(FileImpInfo fileInfo)
        {
            window.PanelPlaylist.ListPlaylist.AddToList(new PlaylistItem(fileInfo));
        }

        protected override void ToggleMaximize()
        {
            if (window.ExtWindowState >= ExtWindowState.Maximized) window.ExtWindowState = ExtWindowState.Normal;
            else window.ExtWindowState = ExtWindowState.Maximized;
        }

        protected override void ToggleFullscreen()
        {
            if (window.ExtWindowState >= ExtWindowState.Maximized) window.ExtWindowState = ExtWindowState.Normal;
            else
            {
                window.ExtWindowState = ExtWindowState.Fullscreen;
                PanelC.HideLeftPanel();
                PanelC.HideRightPanel();
            }
        }

        protected override void Minimize()
        {
            window.ExtWindowState = ExtWindowState.Minimized;
        }

        public override void Update()
        {
            MediaC.MoveTemp = false;

            base.Update();

            MediaC.LastMoved = MediaC.MoveTemp;

            imageLoader.Update();
            mediaLoader.Update();

            window.Updating = false;

            window.PlayerBottom.SliderVolume.Value = MediaC.Volume;

            // TODO: update position only when it has changed
            if (window.UriPlayer.IsPlaying)
            {
                double duration = window.UriPlayer.Duration;
                double position = window.UriPlayer.Position;
                window.PlayerBottom.SliderTime.Maximum = duration;
                window.PlayerBottom.SliderTime.Value = position;

                window.PlayerBottom.LabelPosition.Content =
                    string.Format("{0} / {1}",
                        StringHandler.SecondsToTimeText((int) Math.Round(position)),
                        StringHandler.SecondsToTimeText((int) Math.Round(duration)));

                if (osInfo.Version.Major >= 6)
                {
                    TaskbarManager.Instance.SetProgressValue((int) position, (int) duration);
                    if (MediaC.Paused)
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                    else
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                }
            }
            else
            {
                window.PlayerBottom.SliderTime.Value = 0;
                window.PlayerBottom.SliderTime.Maximum = 1;
                window.PlayerBottom.LabelPosition.Content =
                    string.Format("{0} / {1}",
                        StringHandler.SecondsToTimeText(0),
                        StringHandler.SecondsToTimeText(0));

                if (osInfo.Version.Major >= 6)
                {
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                }
            }
        }

        public override List<PlaylistItem> GetSelectedPlaylistItems()
        {
            return window.PanelPlaylist.ListPlaylist.GetSelectedList();
        }

        public void PermanentlyDeleteFiles(List<PlaylistItem> playlistItemsToDelete)
        {
            int fileDeleteCount = 0;
            foreach (PlaylistItem playlistItem in playlistItemsToDelete)
            {
                if (playingItem != null && playingItem.FullPath.Equals(playlistItem.FullPath))
                {
                    if (playlistItem.FileType == FileTypes.Pictures)
                    {
                        window.ImageViewer.Source = null;

                        currentImage = null;
                        GC.Collect();
                    }
                    else
                    {
                        window.UriPlayer.Clear();
                        while (window.UriPlayer.IsPlaying)
                        {
                            Thread.Sleep(5);
                        }
                    }

                    playingItem = null;
                    ResetTitle();
                }
                try
                {
                    File.Delete(playlistItem.FullPath);
                    fileDeleteCount++;
                }
                catch (Exception)
                {
                    EventC.SetEvent(new EventText("File could not be deleted: " + playlistItem.FullPath, 3,
                        EventType.Delayed));
                }
            }
            EventC.SetEvent(new EventText(fileDeleteCount + " files permanently deleted", 3, EventType.Delayed));
            window.PanelPlaylist.ListPlaylist.RemoveSelected();
        }
    }
}