#region Usings

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
using Imp.Base;
using Imp.Base.Commands;
using Imp.Base.Controllers;
using Imp.Base.FileData;
using Imp.Base.FileLoading;
using Imp.Base.Libraries;
using Imp.Base.ListLogic;
using Imp.Controls.Controllers;
using Imp.DirectShow.Player;
using Imp.Player.Image;
using Imp.Player.Libraries;
using Microsoft.WindowsAPICodePack.Taskbar;

#endregion

namespace Imp.Player.Controllers
{
    public partial class MainController : BaseController
    {
        #region Static Fields and Constants

        private static readonly OperatingSystem osInfo = Environment.OSVersion;

        #endregion

        #region Fields

        public readonly Popup ContentMenu;
        private readonly ImageLoader imageLoader;
        private readonly MediaLoader mediaLoader;
        private readonly MainWindow window;
        private BitmapSource currentImage;
        //private readonly SubtitleController subtitleController;

        #endregion

        #region Properties

        public override bool Focused => !window.PanelOpen.TextBoxFindFolder.IsFocused
                                        && !window.PanelOpen.TextBoxFind.IsFocused
                                        && !window.PanelPlaylist.TextBoxFind.IsFocused;

        public override bool Selected => window.IsActive && window.ExtWindowState != ExtWindowState.Minimized;
        public override bool IsMostRecentInstance => ImpMessaging.LastActive;

        #endregion

        public MainController(MainWindow window)
        {
            AllowedStyles = PlayerStyle.MediaPlayer;

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

            window.UriPlayer.SubtitleElement = window.Subtitles;
            //subtitleController = new SubtitleController(window);

            Initialize(EventC, PanelC, mediaController);

            this.window = window;
            imageLoader = new ImageLoader(window.Dispatcher);
            imageLoader.Loaded += ImageLoaded;
            imageLoader.LoadFailed += LoadFailed;

            mediaLoader = new MediaLoader(window.Dispatcher) { Subtitles = Settings.Subtitles };
            mediaLoader.Loaded += MediaLoaded;
            mediaLoader.LoadFailed += LoadFailed;

            ContentMenu = window.ContentMenu;

            ApplySettings();
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
            //window.PanelOpen.ButtonFilterPlaylist.CurrentState =
            //    (Settings.LastFileTypes & FileTypes.Playlist) == FileTypes.Playlist ? 1 : 0;

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

            if (window.UriPlayer.Controller != null)
            {
                window.UriPlayer.Controller.Command(MediaCommand.Close);
                MediaC.MediaClosed();
            }

            EventC.SetTitle("Loading...");
            window.Title = "Loading...";

            window.PanelPlaylist.ListPlaylist.PlayingThis(item);

            loadingItem = item;

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
            AllowedStyles = PlayerStyle.MediaPlayer;
            EventC.ShowError(error);
            MediaC.MediaClosed();

            playingItem = null;
            loadingItem = null;

            ClearPlayers();

            ResetTitle();
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

            AllowedStyles = PlayerStyle.PictureViewer;

            SetPlayingTitle();
        }

        private void SetPlayingTitle()
        {
            EventC.SetTitle(playingItem.Name);
            window.Title = playingItem.Name;
        }

        private void MediaLoaded(PlayerController playerController)
        {
            //if (loadingItem.FileType.HasFlag(FileTypes.Videos))
            //{
            //    subtitleController.LoadSubtitles(loadingItem.FullPath, Path.GetExtension(loadingItem.FullPath), playerController.VideoSize);
            //}

            playingItem = loadingItem;
            itemOnPlayer = playingItem;

            window.UriPlayer.Controller = playerController;
            if (window.UriPlayer.HasVideo)
            {
                window.ImageViewer.Visibility = Visibility.Hidden;
                window.UriPlayer.Visibility = Visibility.Visible;
                window.LogoViewer.Visibility = Visibility.Hidden;
                AllowedStyles = PlayerStyle.VideoPlayer;
            }
            else
            {
                window.ImageViewer.Visibility = Visibility.Hidden;
                window.UriPlayer.Visibility = Visibility.Hidden;
                window.LogoViewer.Visibility = Visibility.Visible;
                AllowedStyles = PlayerStyle.MusicPlayer;
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

            if (window.ExtWindowState == ExtWindowState.Fullscreen)
            {
                area = ImpNativeMethods.GetMonitorArea(window);
            }
            else
            {
                area = ImpNativeMethods.GetWorkArea(window);
            }

            window.MenuList.SetList(cmdList);

            var size = window.MenuList.DesiredSize();
            ContentMenu.Height = size.Height;
            ContentMenu.Width = size.Width;

            //ContentMenu.HorizontalOffset = -20;
            //ContentMenu.VerticalOffset = -5;
            ContentMenu.HorizontalOffset = Math.Max(Math.Min(cursorPositionInDesktop.X - 20, area.Right - size.Width),
                area.Left);
            ContentMenu.VerticalOffset = Math.Max(Math.Min(cursorPositionInDesktop.Y - 5, area.Bottom - size.Height),
                area.Top);
            ContentMenu.IsOpen = true;
            window.MenuList.CaptureMouse();
        }

        protected override void Shuffle()
        {
            ClearPlayers();
            playingItem = null;
            window.PanelPlaylist.ListPlaylist.SelectNone();
            window.PanelPlaylist.ListPlaylist.PlayingThis(null);
            Exec(ImpCommand.Sort, FileSortMode.Random);
            EventC.SetEvent(new EventText("Shuffled!"));
        }

        protected override void ClearPlayers()
        {
            window.ImageViewer.Source = null;
            window.UriPlayer.Controller?.Command(MediaCommand.Close);
        }

        protected override void RemoveSelectedPath()
        {
            var path = window.PanelOpen.ListPlaces.GetSelected().Value;
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
            var item = window.PanelOpen.ListDirectories.GetSelected();

            var path = window.PanelOpen.ListDirectories.GetSelected()?.Value;

            if (path == null || StringHandler.IsSpecialFolder(path) || Settings.CustomPaths.Contains(path) ||
                path.Length < 4)
            {
                EventC.SetEvent(new EventText("Path cannot be added", 1d, EventType.Delayed));
                return;
            }

            Settings.CustomPaths.Add(path);
            window.PanelOpen.Refresh(this);
        }

        protected override void AddSelectedFolderFolders()
        {
            window.PanelOpen.PrepareFolderLoader(
                new DirectoryLoadOptions(window.PanelOpen.ListDirectories.GetSelected().Value,
                    SearchOption.AllDirectories,
                    window.PanelOpen.GetFileTypes()));
        }

        protected override void AddSelectedFolderFiles()
        {
            window.PanelOpen.PrepareFolderLoader(
                new DirectoryLoadOptions(window.PanelOpen.ListDirectories.GetSelected().Value,
                    SearchOption.TopDirectoryOnly,
                    window.PanelOpen.GetFileTypes()));
        }

        protected override void PlaySelectedFolderFiles()
        {
            var selected = window.PanelOpen.ListDirectories.GetSelected();

            if (selected != null)
            {
                Exec(ImpCommand.ClearPlaylist);
                window.PanelOpen.PrepareFolderLoader(
                    new DirectoryLoadOptions(selected.Value,
                        SearchOption.AllDirectories,
                        window.PanelOpen.GetFileTypes())
                    { PlayFirstFile = true });
            }
        }

        protected override void RequestDeleteOpenFiles()
        {
            var items = window.PanelOpen.ListFiles.GetSelectedList();
            var playlistItems = new List<PlaylistItem>(items.Count);
            var paths = new List<string>(items.Count);

            foreach (var fileImpInfo in items)
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
            var list = window.PanelOpen.ListFiles.GetSelectedList();
            for (var i = 0; i < list.Count; i++)
            {
                if (i == 0)
                    Exec(ImpCommand.OpenFile, list[i]);
                else
                    Exec(ImpCommand.AddFile, list[i]);
            }
        }

        protected override void RequestPlaylistFileDeletion()
        {
            var items = window.PanelPlaylist.ListPlaylist.GetSelectedList();

            var paths = new List<string>(items.Count);

            foreach (var playlistItem in items)
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
            var item = window.PanelPlaylist.ListPlaylist.GetSelected();
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
                var duration = window.UriPlayer.Duration;
                var position = window.UriPlayer.Position;
                window.PlayerBottom.SliderTime.Maximum = duration;
                window.PlayerBottom.SliderTime.Value = position;

                //if (subtitleController.Active)
                //{
                //    this.subtitleController.Update(position);
                //}

                if (window.Width > 400)
                {
                    window.PlayerBottom.LabelPosition.Content =
                        string.Format("{0} / {1}",
                            StringHandler.SecondsToTimeText((int) Math.Round(position)),
                            StringHandler.SecondsToTimeText((int) Math.Round(duration)));
                }
                else
                {
                    window.PlayerBottom.LabelPosition.Content =
                        StringHandler.SecondsToTimeText((int) Math.Round(position));
                }


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
                window.PlayerBottom.LabelPosition.Content = null; // "-:--:--";

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
            var fileDeleteCount = 0;
            foreach (var playlistItem in playlistItemsToDelete)
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

        public Point CursorPositionInDesktop(MouseButtonEventArgs e)
        {
            var location = window.PointToScreen(e.GetPosition(this.window));
            var source = PresentationSource.FromVisual(Application.Current.MainWindow);
            if (source == null) return location;

            var dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            var dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            return new Point(location.X * 96.0 / dpiX, location.Y * 96.0 / dpiY);
            //if (extWindowState != ExtWindowState.Fullscreen )
            //    return e.GetPosition(null) + new Vector(Left, Top);
            //else
            //    return e.GetPosition(null);
        }
    }
}