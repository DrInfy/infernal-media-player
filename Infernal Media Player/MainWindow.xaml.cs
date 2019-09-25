#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Imp.Base;
using Imp.Base.Commands;
using Imp.Base.FileData;
using Imp.Base.FileLoading;
using Imp.Base.Libraries;
using Imp.Controls.Gui;
using Imp.Player.Controllers;
using Imp.Player.Libraries;

#endregion

namespace Imp.Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Helpers

        public delegate void Update();

        #endregion

        #region Fields

        public readonly StyleLib Styling = new StyleLib();
        public bool Updating;
        public bool WindowClosed;
        private readonly MainController mainC;
        private ExtWindowState extWindowState;
        private Rect backupRect;
        private bool allowStateChange; // prevents screen sizing when required
        private MouseStates mouseState = MouseStates.None;
        private Point lastMove;

        #endregion

        #region Properties

        public ExtWindowState ExtWindowState
        {
            get { return this.extWindowState; }
            set
            {
                if (this.ExtWindowState == value) return;

                this.allowStateChange = true;
                if ((int) value < 2)
                {
                    this.ResizeMode = ResizeMode.CanResize;
                    this.WindowState = (WindowState) value;
                    if (this.ExtWindowState > ExtWindowState.Minimized)
                    {
                        SetRect(this, this.backupRect);
                    }

                    this.Topmost = false;
                    this.ButtonMax.CurrentState = 0;
                    //If value = IMP4.ExtWindowState.Minimized Then
                    //    _ExtWindowState = value
                    //    Exit Property
                    //End If
                }
                else
                {
                    if (value == ExtWindowState.Fullscreen)
                    {
                        //Me.Topmost = True
                        this.backupRect = ImpNativeMethods.GetWindowRect(this);
                        this.ResizeMode = ResizeMode.CanMinimize;
                        this.WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        //Me.Topmost = False
                        this.backupRect = ImpNativeMethods.GetWindowRect(this);
                        this.ResizeMode = ResizeMode.CanMinimize;
                        this.WindowState = WindowState.Normal;
                        var area = ImpNativeMethods.GetWorkArea(this);

                        SetRect(this, area);
                        //Me.SizeToContent = Windows.SizeToContent.WidthAndHeight ' =SystemParameters.WorkArea 
                    }
                    this.ButtonMax.CurrentState = 1;
                }

                this.extWindowState = value;
                if (this.ExtWindowState > ExtWindowState.Minimized)
                {
                    Focus();
                }
                this.allowStateChange = false;
            }
        }

        private bool MouseOverGrid => this.grid.IsMouseDirectlyOver || this.LabelTopic.IsMouseOver || this.BarBottom.IsMouseDirectlyOver || this.BarTop.IsMouseDirectlyOver || this.BarTop2.IsMouseDirectlyOver ||
                                      //ImageViewer.IsMouseDirectlyOver ||
                                      this.UriPlayer.IsMouseOver || this.LogoViewer.IsMouseDirectlyOver ||
                                      //PanelOpen.LabelTopic.IsMouseOver ||
                                      //PanelPlaylist.LabelTopic.IsMouseOver ||
                                      this.PlayerBottom.LabelPosition.IsMouseOver || this.ViewerBottom.IsMouseOver;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            this.mainC = new MainController(this);

            SetStyles();
            SetButtonEvents();
        }

        /// <summary>
        /// Sets the events for button so that they can be used.
        /// </summary>
        private void SetButtonEvents()
        {
            this.ButtonOpen.Clicked += ToggleOpenPanel;
            this.ButtonPlayList.Clicked += ToggleOpenPlaylist;
            this.ButtonMin.Clicked += Minimize;
            this.ButtonMax.Clicked += Maximize;
            this.ButtonExit.Clicked += Exit;
        }

        /// <summary>
        /// Sets the styles for buttons and panels.
        /// </summary>
        public void SetStyles()
        {
            this.Styling.LoadStyles();

            this.PanelOpen.SetStyles(this.Styling, this.mainC);
            this.PanelPlaylist.SetStyles(this.Styling, this.mainC);
            this.PlayerBottom.SetStyles(this.Styling, this.mainC);
            this.ViewerBottom.SetStyles(this.Styling, this.mainC);


            this.Styling.SetStyle(this.ButtonOpen, BtnNumber.Open);
            this.Styling.SetStyle(this.ButtonPlayList, BtnNumber.Playlist);

            this.Styling.SetStyle(this.ButtonMin, BtnNumber.Minimize);
            this.Styling.SetStyle(this.ButtonMax, BtnNumber.Maximize);
            this.Styling.SetStyle(this.ButtonExit, BtnNumber.Close);

            this.Styling.SetStyle(this.MenuList);
            this.Styling.SetStyle(this.LabelTopic, false);
            this.Styling.SetStyle(this.LabelEvent, false);


            this.BarBottom.Fill = this.Styling.GetGridBrush(true);
            this.BarTop.Fill = this.Styling.GetGridBrush(true);
            this.BarTop2.Fill = this.Styling.GetGridBrush(true);

            this.SplitterLeft.Fill = this.Styling.GetForeground();
            this.SplitterRight.Fill = this.Styling.GetForeground();
        }

        public void PlayPause(object sender)
        {
            this.mainC.Exec(ImpCommand.Playpause);
        }

        private void ToggleOpenPanel(object sender)
        {
            this.mainC.Exec(ImpCommand.PanelOpen);
        }

        private void ToggleOpenPlaylist(object sender)
        {
            this.mainC.Exec(ImpCommand.PanelPlaylist);
        }

        public void Minimize(object sender)
        {
            this.mainC.Exec(ImpCommand.PlayerMinimize);
        }

        public void Maximize(object sender)
        {
            this.mainC.Exec(ImpCommand.ToggleFullscreen);
        }

        public void Exit(object sender)
        {
            this.mainC.Exec(ImpCommand.Exit);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timerUpdater = new Thread(new ThreadStart(positionUpdate));
            timerUpdater.Priority = ThreadPriority.Lowest;
            timerUpdater.Start();

            ImpMessaging.SetImp(this);
            OpenFileLinesFromMessaging();


            var source = PresentationSource.FromVisual(Application.Current.MainWindow);
            if (source != null)
            {
                var dpiX = source.CompositionTarget.TransformToDevice.M11;
                var dpiY = source.CompositionTarget.TransformToDevice.M22;

                var rect = ImpNativeMethods.GetWorkArea(this);
                this.Width = Math.Min(1200, rect.Width * dpiX);
                this.Height = Math.Min(800, rect.Height * dpiY);
            }
        }

        private void positionUpdate()
        {
            do
            {
                Thread.Sleep(15);

                if (this.Updating) continue;
                this.Updating = true;
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Update(this.mainC.Update));
            } while (!this.WindowClosed);
        }

        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.lastMove = e.MouseDevice.GetPosition(this);
            if (this.ImageViewer.IsMouseDirectlyOver)
            {
                this.ImageViewer.CaptureMouse();
            }
            
            if (this.SplitterLeft.IsMouseOver && e.LeftButton == MouseButtonState.Pressed)
            {
                this.mouseState = MouseStates.PanLeftPressed;
                this.mainC.PanelC.RememberThisPanelPosition(e.GetPosition(this), e.GetPosition(this.SplitterLeft));
                this.grid.CaptureMouse();
                return;
            }

            if (this.SplitterRight.IsMouseOver && e.LeftButton == MouseButtonState.Pressed)
            {
                this.mouseState = MouseStates.PanRightPressed;
                this.mainC.PanelC.RememberThisPanelPosition(e.GetPosition(this), e.GetPosition(this.SplitterRight));
                this.grid.CaptureMouse();
                return;
            }
            // set focus to "something", when clicking away from text boxes
            if (!IsMouseOverList() && !IsMouseOverTextbox())
                this.ButtonExit.Focus();

            if (!this.PlayerBottom.IsMouseOver)
            {
                this.PlayerBottom.CloseMenu();
            }
        }

        private void grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.mouseState == MouseStates.PanLeftPressed)
                {
                    this.mainC.PanelC.PanelPanLeft(e.GetPosition(this));
                    return;
                }

                if (this.mouseState == MouseStates.PanRightPressed)
                {
                    this.mainC.PanelC.PanelPanRight(e.GetPosition(this));
                    return;
                }


                if (this.MouseOverGrid && this.extWindowState == ExtWindowState.Normal)
                {
                    DragMove();
                }
                else if (this.ImageViewer.IsMouseDirectlyOver)
                {
                    var move = e.MouseDevice.GetPosition(this) - this.lastMove;
                    this.lastMove = e.MouseDevice.GetPosition(this);

                    this.mainC.Exec(ImpCommand.PanLeftup, new[] {move.X / this.ImageViewer.ActualWidth * 2, move.Y / this.ImageViewer.ActualHeight * 2});
                }
            }
            else
            {
                if (this.SplitterLeft.IsMouseOver || this.SplitterRight.IsMouseOver)
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else
                {
                    this.Cursor = null;
                }
            }
        }

        private void grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.grid.ReleaseMouseCapture();
            this.ImageViewer.ReleaseMouseCapture();
            this.mouseState = MouseStates.None;
            if (e.ChangedButton == MouseButton.Right)
            {
                if (this.MouseOverGrid)
                {
                    this.mainC.ContextMenu(this.mainC.CursorPositionInDesktop(e), ContextMenuEnum.None);
                }
                //if (PanelPlaylist.ListPlaylist.IsMouseDirectlyOver)
                //    mainC.ContextMenu(CursorPositionInDesktop(e), ContextMenuEnum.Playlist);
                //else if (PanelOpen.ListFiles.IsMouseDirectlyOver)
                //    mainC.ContextMenu(CursorPositionInDesktop(e), ContextMenuEnum.FileList);
                //else if (PanelOpen.ListDirectories.IsMouseDirectlyOver)
                //    mainC.ContextMenu(CursorPositionInDesktop(e), ContextMenuEnum.FolderList);
                //else if (PanelOpen.ListPlaces.IsMouseDirectlyOver)
                //    mainC.ContextMenu(CursorPositionInDesktop(e), ContextMenuEnum.PlacesList);
                //else
                //    mainC.ContextMenu(CursorPositionInDesktop(e), ContextMenuEnum.None);
            }
        }

        

        /// <summary>
        /// Overridden to ensure that ExtWindowState keeps up to date.
        /// </summary>
        protected override void OnStateChanged(EventArgs e)
        {
            if (this.allowStateChange)
                base.OnStateChanged(e);
            else
                this.ExtWindowState = (ExtWindowState) this.WindowState;
        }

        private static void SetRect(Window o, Rect r)
        {
            o.Left = r.Left;
            o.Top = r.Top;
            o.Height = r.Height;
            o.Width = r.Width;
        }

        private void Panel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.mainC.PanelC.CheckMainGrid();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.mainC.ContentMenu.IsMouseOver) return;
            this.mouseState = MouseStates.None;
            this.mainC.PanelC.CheckPanelHide(new Point(double.MaxValue, double.MaxValue));
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            this.mainC.PanelC.CheckPanelHide(e.GetPosition(this));
        }

        private void UriPlayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.mainC.Exec(ImpCommand.Playpause);
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (IsMouseOverList()) return;

            if (this.mainC.AllowedStyles == PlayerStyle.PictureViewer)
            {
                this.mainC.Exec(ImpCommand.ChangeZoom, Math.Sign(e.Delta) * 0.03);
            }
            else
            {
                this.mainC.Exec(ImpCommand.VolumeChange, Math.Sign(e.Delta) * 0.03);
            }
        }

        private bool IsMouseOverList()
        {
            return this.PanelOpen.ListPlaces.IsMouseOver || this.PanelOpen.ListDirectories.IsMouseOver || this.PanelOpen.ListFiles.IsMouseOver || this.PanelPlaylist.ListPlaylist.IsMouseOver;
        }

        private bool IsMouseOverTextbox()
        {
            return this.PanelOpen.TextBoxFind.IsMouseOver || this.PanelOpen.TextBoxFindFolder.IsMouseOver || this.PanelPlaylist.TextBoxFind.IsMouseOver;
        }

        private void UriPlayer_MediaPlayerEnded()
        {
            this.mainC.MediaEnded();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.mainC.Resize();
        }

        public void OpenFileLinesFromMessaging()
        {
            if (OpenLines(ImpMessaging.List)) return;

            ImpMessaging.List.Clear();
        }

        private bool OpenLines(IEnumerable<string> list)
        {
            var files = new List<FileImpInfo>();
            foreach (var path in list)
            {
                if (Directory.Exists(path))
                {
                    // path is a directory

                    var options = new DirectoryLoadOptions(path, SearchOption.TopDirectoryOnly,
                        FileTypes.Music | FileTypes.Videos | FileTypes.Pictures, string.Empty);
                    FileInfo[] fileInfos;
                    try
                    {
                        //files = Directory.GetFiles(path);
                        var directoryInfo = new DirectoryInfo(path);

                        fileInfos = directoryInfo.GetFiles();
                    }
                    catch (Exception e)
                    {
                        // doesn't matter why path choosing failed, no files available in this folder
                        var error = new ImpError(ErrorType.FailedToOpenFolder, e.Message);
                        this.mainC.EventC.ShowError(error);
                        return true;
                    }


                    if (fileInfos.Length < 1)
                        return true;

                    files.AddRange(options.FilterFiles(fileInfos, false));
                }
                else if (File.Exists(path))
                {
                    files.Add(new FileImpInfo(path));
                }
                else
                {
                    // File or folder could not be identified
                    var error = new ImpError(path, ErrorType.FileNotFound);
                    this.mainC.EventC.ShowError(error);
                }
            }
            if (files.Count == 1)
                this.mainC.Exec(ImpCommand.OpenFile, files[0]);
            else
            {
                foreach (var fileInfo in files)
                {
                    this.mainC.Exec(ImpCommand.AddFile, fileInfo);
                }
            }
            return false;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.BarTop.IsMouseOver || this.BarTop2.IsMouseOver || this.LabelTopic.IsMouseOver || this.LabelEvent.IsMouseOver || this.grid.IsMouseDirectlyOver || this.LogoViewer.IsMouseDirectlyOver || this.UriPlayer.IsMouseOver)
            {
                this.mainC.Exec(ImpCommand.ToggleFullscreen);
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ImpMessaging.DeclareActive();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                OpenLines((IEnumerable<string>) e.Data.GetData(DataFormats.FileDrop));
            }
        }

        private void MenuList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var textAndCommand = this.MenuList.GetSelected();
            if (textAndCommand != null)
                this.mainC.Exec(textAndCommand.Command, textAndCommand.Argument?.Invoke());
            this.ContentMenu.IsOpen = false;
        }

        private void ContentMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ContentMenu.IsOpen = false;
        }

        private void MenuList_OnTouchTap(object sender, TouchEventArgs e)
        {
            var textAndCommand = this.MenuList.GetSelected();
            if (textAndCommand != null)
                this.mainC.Exec(textAndCommand.Command, textAndCommand.Argument?.Invoke());
            this.ContentMenu.IsOpen = false;
        }
    }
}