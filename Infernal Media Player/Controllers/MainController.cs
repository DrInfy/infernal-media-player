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
using Imp.Base.Player;
using Imp.Controls.Controllers;
using Imp.MpvPlayer;
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
        private readonly MpvLoader mediaLoader;
        private readonly MainWindow window;
        private BitmapSource currentImage;
        //private readonly SubtitleController subtitleController;

        #endregion

        #region Properties

        public override bool Focused => !this.window.PanelOpen.TextBoxFindFolder.IsFocused
                                        && !this.window.PanelOpen.TextBoxFind.IsFocused
                                        && !this.window.PanelPlaylist.TextBoxFind.IsFocused;

        public override bool Selected => this.window.IsActive && this.window.ExtWindowState != ExtWindowState.Minimized;
        public override bool IsMostRecentInstance => ImpMessaging.LastActive;

        #endregion

        public MainController(MainWindow window)
        {
            this.AllowedStyles = PlayerStyle.MediaPlayer;
            window.ViewerBottom.Visibility = Visibility.Hidden;

            ToolTipService.ShowDurationProperty.OverrideMetadata(
                typeof (DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            this.EventC = new EventController(window.Dispatcher, window.LabelPopup, window.LabelEvent, window.LabelTopic);
            this.PanelC = new PanelController(window);

            var mediaController = new MediaController(
                window.UriPlayer,
                window.PlayerBottom.ButtonPlay,
                window.PlayerBottom.ButtonMute,
                window.PlayerBottom.ButtonLoop, this.EventC);

            //window.UriPlayer.SubtitleElement = window.Subtitles;
            //subtitleController = new SubtitleController(window);

            Initialize(this.EventC, this.PanelC, mediaController, new ImageManipulator(window.ImageViewer));

            this.window = window;
            this.imageLoader = new ImageLoader(window.Dispatcher);
            this.imageLoader.Loaded += ImageLoaded;
            this.imageLoader.LoadFailed += LoadFailed;

            this.mediaLoader = new MpvLoader(window.Dispatcher, this.window.UriPlayer);
            this.mediaLoader.Loaded += MediaLoaded;
            this.mediaLoader.LoadFailed += LoadFailed;

            this.ContentMenu = window.ContentMenu;

            ApplySettings();
        }

        protected void ApplySettings()
        {
            this.MediaC.SetVolume(this.Settings.Volume);
            this.MediaC.LoopMode = this.Settings.LastLoopMode;
            if (this.MediaC.LoopMode == LoopMode.LoopAll)
                this.window.PlayerBottom.ButtonLoop.CurrentState = 1;
            this.window.PanelOpen.ButtonFilterVideo.CurrentState =
                (this.Settings.LastFileTypes & FileTypes.Videos) == FileTypes.Videos ? 1 : 0;
            this.window.PanelOpen.ButtonFilterMusic.CurrentState =
                (this.Settings.LastFileTypes & FileTypes.Music) == FileTypes.Music ? 1 : 0;
            this.window.PanelOpen.ButtonFilterPictures.CurrentState = this.Settings.LastFileTypes.HasFlag(FileTypes.Pictures) ? 1 : 0;
            //window.PanelOpen.ButtonFilterPlaylist.CurrentState =
            //    (Settings.LastFileTypes & FileTypes.Playlist) == FileTypes.Playlist ? 1 : 0;

            this.window.PanelOpen.GetFilters();
        }

        protected override void UpdateSettings()
        {
            this.Settings.Volume = this.MediaC.Volume;
            this.Settings.LastLoopMode = this.MediaC.LoopMode;
            if (this.Settings.LastLoopMode == LoopMode.LoopOne)
                this.Settings.LastLoopMode = LoopMode.LoopAll;
            this.Settings.LastFileTypes = this.window.PanelOpen.GetFileTypes();
        }

        protected override void CloseWindows()
        {
            this.window.Close();
            this.window.WindowClosed = true;
        }

        protected override void OpenFile(PlaylistItem item)
        {
            if (item == null || this.loadingItem != null && this.loadingItem.FullPath == item.FullPath
                || this.playingItem != null && this.playingItem.FullPath == item.FullPath)
            {
                if (item == null)
                {
                    this.EventC.SetEvent(new EventText("Cannot play item that doesn't exist"));
                    return;
                }
                if (this.mediaLoader.IsLoading || this.mediaLoader.IsLoading)
                {
                    this.EventC.SetEvent(new EventText("Already loading that item"));
                    return;
                }
                this.EventC.SetEvent(new EventText("Already playing that item"));
                return;
            }

            if (this.window.UriPlayer.IsPlaying)
            {
                this.window.UriPlayer.Close();
                this.MediaC.MediaClosed();
                this.playingItem = null;
            }

            this.EventC.SetTitle("Loading...");
            this.window.Title = "Loading...";

            this.window.PanelPlaylist.ListPlaylist.PlayingThis(item);

            if (this.loadingItem != null)
            {
                this.imageLoader.Abort();
                this.mediaLoader.Abort();
            }

            this.loadingItem = item;

            if (item.FileType == FileTypes.Pictures)
            {
                this.imageLoader.OpenFile(item.FullPath);
            }
            else
            {
                this.mediaLoader.OpenFile(item.FullPath);
            }
        }

        private void LoadFailed(ImpError error)
        {
            this.AllowedStyles = PlayerStyle.MediaPlayer;
            this.EventC.ShowError(error);
            this.MediaC.MediaClosed();

            this.playingItem = null;
            this.loadingItem = null;

            ClearPlayers();

            ResetTitle();
        }

        private void ResetTitle()
        {
            this.EventC.SetTitle("");
            this.window.Title = "Infernal Media Player";
        }

        private void ImageLoaded(BitmapSource bitmap)
        {
            this.playingItem = this.loadingItem;
            //loadingItem = null;
            this.itemOnPlayer = this.playingItem;
            this.window.PanelPlaylist.ListPlaylist.PlayingThis(this.playingItem);
            this.window.ImageViewer.Visibility = Visibility.Visible;
            this.window.UriPlayer.Visibility = Visibility.Hidden;
            this.window.LogoViewer.Visibility = Visibility.Hidden;
            this.window.UriPlayer.Clear();
            this.currentImage = bitmap;

            this.window.ImageViewer.Source = bitmap;
            this.window.ImageViewer.Stretch = Stretch.Uniform;
            this.window.ImageViewer.StretchDirection = StretchDirection.Both;

            this.AllowedStyles = PlayerStyle.PictureViewer;
            this.window.ViewerBottom.Visibility = Visibility.Visible;
            this.window.PlayerBottom.Visibility = Visibility.Hidden;
            this.imageController.ImageChanged();

            SetPlayingTitle();
        }

        private void SetPlayingTitle()
        {
            this.EventC.SetTitle(this.playingItem.Name);
            this.window.Title = this.playingItem.Name;
        }

        private void MediaLoaded(bool success)
        {
            this.playingItem = this.loadingItem;
            this.itemOnPlayer = this.playingItem;
            
            if (this.window.UriPlayer.HasVideo)
            {
                this.window.ImageViewer.Visibility = Visibility.Hidden;
                this.window.UriPlayer.Visibility = Visibility.Visible;
                this.window.LogoViewer.Visibility = Visibility.Hidden;
                this.AllowedStyles = PlayerStyle.VideoPlayer;
            }
            else
            {
                this.window.ImageViewer.Visibility = Visibility.Hidden;
                this.window.UriPlayer.Visibility = Visibility.Hidden;
                this.window.LogoViewer.Visibility = Visibility.Visible;
                this.AllowedStyles = PlayerStyle.MusicPlayer;
            }

            this.window.PanelPlaylist.ListPlaylist.PlayingThis(this.playingItem);

            SetPlayingTitle();
            this.EventC.SetEvent(new EventText(">" + this.playingItem.Name));
            this.MediaC.MediaOpened();

            this.window.ViewerBottom.Visibility = Visibility.Hidden;
            this.window.PlayerBottom.Visibility = Visibility.Visible;
        }

        protected override void OpenFile(FileImpInfo fileInfo)
        {
            var item = new PlaylistItem(fileInfo);
            this.window.PanelPlaylist.ListPlaylist.AddToList(item);

            OpenFile(item);
        }

        protected override void CreateContextMenu(Point cursorPositionInDesktop, List<ImpTextAndCommand> cmdList)
        {
            Rect area;

            if (this.window.ExtWindowState == ExtWindowState.Fullscreen)
            {
                area = ImpNativeMethods.GetMonitorArea(this.window);
            }
            else
            {
                area = ImpNativeMethods.GetWorkArea(this.window);
            }

            this.window.MenuList.SetList(cmdList);

            var size = this.window.MenuList.DesiredSize();
            this.ContentMenu.Height = size.Height;
            this.ContentMenu.Width = size.Width;

            //ContentMenu.HorizontalOffset = -20;
            //ContentMenu.VerticalOffset = -5;
            this.ContentMenu.HorizontalOffset = Math.Max(Math.Min(cursorPositionInDesktop.X - 20, area.Right - size.Width),
                area.Left);
            this.ContentMenu.VerticalOffset = Math.Max(Math.Min(cursorPositionInDesktop.Y - 5, area.Bottom - size.Height),
                area.Top);
            this.ContentMenu.IsOpen = true;
            this.window.MenuList.CaptureMouse();
        }

        protected override void ChangeAudioTrack(object argument)
        {
            if (this.window.UriPlayer.IsPlaying)
            {
                if (EnsureMainThread())
                {
                    this.window.Dispatcher.Invoke(() => ChangeAudioTrack(argument));
                    return;
                }

                if (argument != null)
                {
                    var trackId = (int)argument;
                    this.window.UriPlayer.SetAudioTrack(trackId);
                    this.EventC.SetEvent(new EventText("Subtitle " + trackId + "!"));
                }
                else
                {
                    var trackId = this.window.UriPlayer.NextAudioTrack();
                    this.EventC.SetEvent(new EventText("Subtitle " + trackId + "!"));
                }
            }
            else
            {
                this.EventC.SetEvent(new EventText("Not playing!"));
            }
        }

        private bool EnsureMainThread()
        {
            return Thread.CurrentThread != this.window.Dispatcher.Thread;
        }

        protected override void ChangeSubtitles(object argument)
        {
            if (this.window.UriPlayer.IsPlaying)
            {
                if (EnsureMainThread())
                {
                    this.window.Dispatcher.Invoke(() => ChangeSubtitles(argument));
                    return;
                }

                if (argument != null)
                {
                    var trackId = (int) argument;
                    this.window.UriPlayer.SetSubtitle(trackId);
                    this.EventC.SetEvent(new EventText("Subtitle " + trackId + "!"));
                }
                else
                {
                    var trackId = this.window.UriPlayer.NextSubtitle();
                    this.EventC.SetEvent(new EventText("Subtitle " + trackId + "!"));
                }
            }
            else
            {
                this.EventC.SetEvent(new EventText("Not playing!"));
            }
        }

        protected override void Shuffle()
        {
            ClearPlayers();
            this.playingItem = null;
            this.window.PanelPlaylist.ListPlaylist.SelectNone();
            this.window.PanelPlaylist.ListPlaylist.PlayingThis(null);
            Exec(ImpCommand.Sort, FileSortMode.Random);
            this.EventC.SetEvent(new EventText("Shuffled!"));
        }

        protected override void ClearPlayers()
        {
            this.window.ImageViewer.Source = null;
            this.window.UriPlayer.Close();
        }

        protected override void RemoveSelectedPath()
        {
            var path = this.window.PanelOpen.ListPlaces.GetSelected().Value;
            if (!this.Settings.CustomPaths.Contains(path))
            {
                this.EventC.SetEvent(new EventText("Path cannot be removed", 1, EventType.Delayed));
                return;
            }
            this.Settings.CustomPaths.Remove(path);
            this.window.PanelOpen.Refresh(this);
        }

        protected override void DeleteOpenFolder()
        {
            // TODO: implement this
        }

        protected override void AddSelectedFolderToPaths()
        {
            var item = this.window.PanelOpen.ListDirectories.GetSelected();

            var path = this.window.PanelOpen.ListDirectories.GetSelected()?.Value;

            if (path == null || StringHandler.IsSpecialFolder(path) || this.Settings.CustomPaths.Contains(path) ||
                path.Length < 4)
            {
                this.EventC.SetEvent(new EventText("Path cannot be added", 1d, EventType.Delayed));
                return;
            }

            this.Settings.CustomPaths.Add(path);
            this.window.PanelOpen.Refresh(this);
        }

        protected override void AddSelectedFolderFolders()
        {
            this.window.PanelOpen.PrepareFolderLoader(
                new DirectoryLoadOptions(this.window.PanelOpen.ListDirectories.GetSelected().Value,
                    SearchOption.AllDirectories, this.window.PanelOpen.GetFileTypes()));
        }

        protected override void AddSelectedFolderFiles()
        {
            this.window.PanelOpen.PrepareFolderLoader(
                new DirectoryLoadOptions(this.window.PanelOpen.ListDirectories.GetSelected().Value,
                    SearchOption.TopDirectoryOnly, this.window.PanelOpen.GetFileTypes()));
        }

        protected override void PlaySelectedFolderFiles()
        {
            var selected = this.window.PanelOpen.ListDirectories.GetSelected();

            if (selected != null)
            {
                Exec(ImpCommand.ClearPlaylist);
                this.window.PanelOpen.PrepareFolderLoader(
                    new DirectoryLoadOptions(selected.Value,
                        SearchOption.AllDirectories, this.window.PanelOpen.GetFileTypes())
                    { PlayFirstFile = true });
            }
        }

        protected override void RequestDeleteOpenFiles()
        {
            var items = this.window.PanelOpen.ListFiles.GetSelectedList();
            var playlistItems = new List<PlaylistItem>(items.Count);
            var paths = new List<string>(items.Count);

            foreach (var fileImpInfo in items)
            {
                paths.Add(fileImpInfo.Path);
                playlistItems.Add(new PlaylistItem(fileImpInfo));
            }

            if (PopupDeleteWindow(paths, playlistItems))
                this.window.PanelOpen.ListFiles.Refresh();
        }

        protected override void AddSelectedOpenFiles()
        {
            this.window.PanelOpen.ButtonAddSelected_Clicked(this);
        }

        protected override void PlaySelectedOpenFiles()
        {
            var list = this.window.PanelOpen.ListFiles.GetSelectedList();
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
            var items = this.window.PanelPlaylist.ListPlaylist.GetSelectedList();

            var paths = new List<string>(items.Count);

            foreach (var playlistItem in items)
                paths.Add(playlistItem.FullPath);

            PopupDeleteWindow(paths, items);
        }

        private bool PopupDeleteWindow(List<string> paths, List<PlaylistItem> items)
        {
            var deleteWindow = new DeleteItemsWindow(this, paths);
            deleteWindow.Owner = this.window;

            deleteWindow.SetStyles(this.window.Styling);
            if (deleteWindow.ShowDialog() == true)
            {
                PermanentlyDeleteFiles(items);
                return true;
            }
            return false;
        }

        protected override void OpenSelectedInExplorer()
        {
            var item = this.window.PanelPlaylist.ListPlaylist.GetSelected();
            if (item != null)
            {
                Process.Start("explorer.exe", string.Format("/select,\"{0}\"", item.FullPath));
            }
        }

        protected override void OpenSelectedInPlayList()
        {
            OpenFile(this.window.PanelPlaylist.ListPlaylist.GetSelected());
        }

        protected override void PlaylistSort(FileSortMode fileSortMode)
        {
            this.window.PanelPlaylist.ListPlaylist.Sort(fileSortMode);
        }

        protected override void RemoveSelectedFromPlaylist()
        {
            this.window.PanelPlaylist.ListPlaylist.RemoveSelected();
        }

        protected override void OpenRandom()
        {
            this.window.PanelPlaylist.ListPlaylist.OpenRandom();
        }

        public override void MediaEnded()
        {
            if (this.MediaC.LoopMode == LoopMode.LoopOne)
            {
                this.MediaC.SetPosition(0);
                this.MediaC.Play();
            }
            else
            {
                this.window.PanelPlaylist.ListPlaylist.OpenNext(this.MediaC.LoopMode);
            }
        }

        protected override void OpenPrev()
        {
            if (this.MediaC.LoopMode == LoopMode.LoopOne)
            {
                this.window.PanelPlaylist.ListPlaylist.OpenPrev(LoopMode.NoLoop);
            }
            else
            {
                this.window.PanelPlaylist.ListPlaylist.OpenPrev(this.MediaC.LoopMode);
            }
        }

        protected override void OpenNext()
        {
            if (this.MediaC.LoopMode == LoopMode.LoopOne)
            {
                this.window.PanelPlaylist.ListPlaylist.OpenNext(LoopMode.NoLoop);
            }
            else
            {
                this.window.PanelPlaylist.ListPlaylist.OpenNext(this.MediaC.LoopMode);
            }
        }

        protected override void ClearPlayList()
        {
            this.window.PanelPlaylist.ListPlaylist.ClearList();
        }

        protected override void AddFile(FileImpInfo fileInfo)
        {
            this.window.PanelPlaylist.ListPlaylist.AddToList(new PlaylistItem(fileInfo));
        }

        protected override void ToggleMaximize()
        {
            if (this.window.ExtWindowState >= ExtWindowState.Maximized) this.window.ExtWindowState = ExtWindowState.Normal;
            else this.window.ExtWindowState = ExtWindowState.Maximized;
        }

        protected override void ToggleFullscreen()
        {
            if (this.window.ExtWindowState >= ExtWindowState.Maximized) this.window.ExtWindowState = ExtWindowState.Normal;
            else
            {
                this.window.ExtWindowState = ExtWindowState.Fullscreen;
                this.PanelC.HideLeftPanel();
                this.PanelC.HideRightPanel();
            }
        }

        protected override void Minimize()
        {
            this.window.ExtWindowState = ExtWindowState.Minimized;
        }

        public override void Update()
        {
            this.MediaC.MoveTemp = false;

            base.Update();

            this.MediaC.LastMoved = this.MediaC.MoveTemp;

            this.imageLoader.Update();
            this.mediaLoader.Update();

            this.window.Updating = false;

            this.window.PlayerBottom.SliderVolume.Value = this.MediaC.Volume;

            // TODO: update position only when it has changed
            if (this.window.UriPlayer.IsPlaying)
            {
                var duration = this.window.UriPlayer.Duration;
                var position = this.window.UriPlayer.Position;
                this.window.PlayerBottom.SliderTime.Maximum = duration;
                this.window.PlayerBottom.SliderTime.Value = position;

                //if (subtitleController.Active)
                //{
                //    this.subtitleController.Update(position);
                //}

                if (this.window.Width > 400)
                {
                    this.window.PlayerBottom.LabelPosition.Content =
                        string.Format("{0} / {1}",
                            StringHandler.SecondsToTimeText((int) Math.Round(position)),
                            StringHandler.SecondsToTimeText((int) Math.Round(duration)));
                }
                else
                {
                    this.window.PlayerBottom.LabelPosition.Content =
                        StringHandler.SecondsToTimeText((int) Math.Round(position));
                }


                if (osInfo.Version.Major >= 6)
                {
                    TaskbarManager.Instance.SetProgressValue((int) position, (int) duration);
                    if (this.MediaC.Paused)
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                    else
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                }
            }
            else
            {
                this.window.PlayerBottom.SliderTime.Value = 0;
                this.window.PlayerBottom.SliderTime.Maximum = 1;
                this.window.PlayerBottom.LabelPosition.Content = null; // "-:--:--";

                if (osInfo.Version.Major >= 6)
                {
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                }
            }
        }

        public override List<PlaylistItem> GetSelectedPlaylistItems()
        {
            return this.window.PanelPlaylist.ListPlaylist.GetSelectedList();
        }

        public void PermanentlyDeleteFiles(List<PlaylistItem> playlistItemsToDelete)
        {
            var fileDeleteCount = 0;
            foreach (var playlistItem in playlistItemsToDelete)
            {
                if (this.playingItem != null && this.playingItem.FullPath.Equals(playlistItem.FullPath))
                {
                    if (playlistItem.FileType == FileTypes.Pictures)
                    {
                        this.window.ImageViewer.Source = null;

                        this.currentImage = null;
                        GC.Collect();
                    }
                    else
                    {
                        this.window.UriPlayer.Clear();
                        while (this.window.UriPlayer.IsPlaying)
                        {
                            Thread.Sleep(5);
                        }
                    }

                    this.playingItem = null;
                    ResetTitle();
                }
                try
                {
                    File.Delete(playlistItem.FullPath);
                    fileDeleteCount++;
                }
                catch (Exception)
                {
                    this.EventC.SetEvent(new EventText("File could not be deleted: " + playlistItem.FullPath, 3,
                        EventType.Delayed));
                }
            }
            this.EventC.SetEvent(new EventText(fileDeleteCount + " files permanently deleted", 3, EventType.Delayed));
            this.window.PanelPlaylist.ListPlaylist.RemoveSelected();
        }

        public Point CursorPositionInDesktop(MouseButtonEventArgs e)
        {
            var location = this.window.PointToScreen(e.GetPosition(this.window));
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

        public void Resize()
        {
            this.EventC.RefreshPosition();
            this.PanelC.CheckResize();
            this.imageController.ScreenSizeChanged();
        }
    }
}