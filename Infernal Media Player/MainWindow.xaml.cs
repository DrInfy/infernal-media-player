﻿#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Base;
using Base.Commands;
using Base.FileData;
using Base.FileLoading;
using Base.Libraries;
using Imp.Controllers;
using Imp.Libraries;
using ImpControls.Gui;

#endregion

namespace Imp
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

        #endregion

        #region Properties

        public ExtWindowState ExtWindowState
        {
            get { return extWindowState; }
            set
            {
                if (ExtWindowState == value) return;

                allowStateChange = true;
                if ((int) value < 2)
                {
                    ResizeMode = ResizeMode.CanResize;
                    WindowState = (WindowState) value;
                    if (ExtWindowState > ExtWindowState.Minimized)
                    {
                        SetRect(this, backupRect);
                    }

                    Topmost = false;
                    ButtonMax.CurrentState = 0;
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
                        backupRect = ImpNativeMethods.GetWindowRect(this);
                        ResizeMode = ResizeMode.CanMinimize;
                        WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        //Me.Topmost = False
                        backupRect = ImpNativeMethods.GetWindowRect(this);
                        ResizeMode = ResizeMode.CanMinimize;
                        WindowState = WindowState.Normal;
                        var area = ImpNativeMethods.GetWorkArea(this);

                        SetRect(this, area);
                        //Me.SizeToContent = Windows.SizeToContent.WidthAndHeight ' =SystemParameters.WorkArea 
                    }
                    ButtonMax.CurrentState = 1;
                }

                extWindowState = value;
                if (ExtWindowState > ExtWindowState.Minimized)
                {
                    Focus();
                }
                allowStateChange = false;
            }
        }

        private bool MouseOverGrid
        {
            get
            {
                return grid.IsMouseDirectlyOver ||
                       LabelTopic.IsMouseOver ||
                       BarBottom.IsMouseDirectlyOver ||
                       BarTop.IsMouseDirectlyOver ||
                       BarTop2.IsMouseDirectlyOver ||
                       ImageViewer.IsMouseDirectlyOver ||
                       LogoViewer.IsMouseDirectlyOver ||
                       PanelOpen.LabelTopic.IsMouseOver ||
                       PanelPlaylist.LabelTopic.IsMouseOver ||
                       PlayerBottom.LabelPosition.IsMouseOver;
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            mainC = new MainController(this);

            SetStyles();
            SetButtonEvents();
        }

        /// <summary>
        /// Sets the events for button so that they can be used.
        /// </summary>
        private void SetButtonEvents()
        {
            ButtonOpen.Clicked += ToggleOpenPanel;
            ButtonPlayList.Clicked += ToggleOpenPlaylist;
            ButtonMin.Clicked += Minimize;
            ButtonMax.Clicked += Maximize;
            ButtonExit.Clicked += Exit;
        }

        /// <summary>
        /// Sets the styles for buttons and panels.
        /// </summary>
        public void SetStyles()
        {
            Styling.LoadStyles();

            PanelOpen.SetStyles(Styling, mainC);
            PanelPlaylist.SetStyles(Styling, mainC);
            PlayerBottom.SetStyles(Styling, mainC);


            Styling.SetStyle(ButtonOpen, BtnNumber.Open);
            Styling.SetStyle(ButtonPlayList, BtnNumber.Playlist);

            Styling.SetStyle(ButtonMin, BtnNumber.Minimize);
            Styling.SetStyle(ButtonMax, BtnNumber.Maximize);
            Styling.SetStyle(ButtonExit, BtnNumber.Close);

            Styling.SetStyle(MenuList);
            LabelTopic.Foreground = Styling.GetForeground();

            BarBottom.Fill = Styling.GetGridBrush(true);
            BarTop.Fill = Styling.GetGridBrush(true);
            BarTop2.Fill = Styling.GetGridBrush(true);

            SplitterLeft.Fill = Styling.GetForeground();
            SplitterRight.Fill = Styling.GetForeground();
        }

        public void PlayPause(object sender)
        {
            mainC.Exec(ImpCommand.Playpause);
        }

        private void ToggleOpenPanel(object sender)
        {
            mainC.Exec(ImpCommand.PanelOpen);
        }

        private void ToggleOpenPlaylist(object sender)
        {
            mainC.Exec(ImpCommand.PanelPlaylist);
        }

        public void Minimize(object sender)
        {
            mainC.Exec(ImpCommand.PlayerMinimize);
        }

        public void Maximize(object sender)
        {
            mainC.Exec(ImpCommand.ToggleFullscreen);
        }

        public void Exit(object sender)
        {
            mainC.Exec(ImpCommand.Exit);
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
                Width = Math.Min(1200 / dpiX, rect.Width * dpiX);
                Height = Math.Min(800 / dpiY, rect.Height * dpiY);
            }
        }

        private void positionUpdate()
        {
            do
            {
                Thread.Sleep(15);

                if (Updating) continue;
                Updating = true;
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Update(mainC.Update));
            } while (!WindowClosed);
        }

        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SplitterLeft.IsMouseOver && e.LeftButton == MouseButtonState.Pressed)
            {
                mouseState = MouseStates.PanLeftPressed;
                mainC.PanelC.RememberThisPanelPosition(e.GetPosition(this), e.GetPosition(SplitterLeft));
                grid.CaptureMouse();
                return;
            }

            if (SplitterRight.IsMouseOver && e.LeftButton == MouseButtonState.Pressed)
            {
                mouseState = MouseStates.PanRightPressed;
                mainC.PanelC.RememberThisPanelPosition(e.GetPosition(this), e.GetPosition(SplitterRight));
                grid.CaptureMouse();
                return;
            }
            // set focus to "something", when clicking away from text boxes
            if (!IsMouseOverList() && !IsMouseOverTextbox())
                ButtonExit.Focus();
        }

        private void grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (mouseState == MouseStates.PanLeftPressed)
                {
                    mainC.PanelC.PanelPanLeft(e.GetPosition(this));
                    return;
                }

                if (mouseState == MouseStates.PanRightPressed)
                {
                    mainC.PanelC.PanelPanRight(e.GetPosition(this));
                    return;
                }


                if (MouseOverGrid && extWindowState == ExtWindowState.Normal)
                    DragMove();
            }
            else
            {
                if (SplitterLeft.IsMouseOver || SplitterRight.IsMouseOver)
                {
                    Cursor = Cursors.SizeWE;
                }
                else
                {
                    Cursor = null;
                }
            }
        }

        private void grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            grid.ReleaseMouseCapture();
            mouseState = MouseStates.None;
            if (e.ChangedButton == MouseButton.Right)
            {
                if (this.MouseOverGrid)
                {
                    mainC.ContextMenu(mainC.CursorPositionInDesktop(e), ContextMenuEnum.None);
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
            if (allowStateChange)
                base.OnStateChanged(e);
            else
                ExtWindowState = (ExtWindowState) WindowState;
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
            mainC.PanelC.CheckMainGrid();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mainC.ContentMenu.IsMouseOver) return;
            mouseState = MouseStates.None;
            mainC.PanelC.CheckPanelHide(new Point(double.MaxValue, double.MaxValue));
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            mainC.PanelC.CheckPanelHide(e.GetPosition(this));
        }

        private void UriPlayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                mainC.Exec(ImpCommand.Playpause);
        }

        private void UriPlayer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mainC.Exec(ImpCommand.ToggleFullscreen);
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (IsMouseOverList()) return;

            mainC.Exec(ImpCommand.VolumeChange, Math.Sign(e.Delta) * 0.03);
        }

        private bool IsMouseOverList()
        {
            return PanelOpen.ListPlaces.IsMouseOver ||
                   PanelOpen.ListDirectories.IsMouseOver ||
                   PanelOpen.ListFiles.IsMouseOver ||
                   PanelPlaylist.ListPlaylist.IsMouseOver;
        }

        private bool IsMouseOverTextbox()
        {
            return PanelOpen.TextBoxFind.IsMouseOver ||
                   PanelOpen.TextBoxFindFolder.IsMouseOver ||
                   PanelPlaylist.TextBoxFind.IsMouseOver;
        }

        private void UriPlayer_MediaPlayerEnded()
        {
            mainC.MediaEnded();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainC.PanelC.CheckResize();
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
                        mainC.EventC.ShowError(error);
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
                    mainC.EventC.ShowError(error);
                }
            }
            if (files.Count == 1)
                mainC.Exec(ImpCommand.OpenFile, files[0]);
            else
            {
                foreach (var fileInfo in files)
                {
                    mainC.Exec(ImpCommand.AddFile, fileInfo);
                }
            }
            return false;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (BarTop.IsMouseOver ||
                BarTop2.IsMouseOver ||
                LabelTopic.IsMouseOver ||
                LabelEvent.IsMouseOver ||
                grid.IsMouseDirectlyOver ||
                LogoViewer.IsMouseDirectlyOver)
            {
                mainC.Exec(ImpCommand.ToggleFullscreen);
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
            var textAndCommand = MenuList.GetSelected();
            if (textAndCommand != null)
                mainC.Exec(textAndCommand.Command, textAndCommand.Argument);
            ContentMenu.IsOpen = false;
        }

        private void ContentMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentMenu.IsOpen = false;
        }

        private void MenuList_OnTouchTap(object sender, TouchEventArgs e)
        {
            var textAndCommand = MenuList.GetSelected();
            if (textAndCommand != null)
                mainC.Exec(textAndCommand.Command, textAndCommand.Argument);
            ContentMenu.IsOpen = false;
        }
    }
}