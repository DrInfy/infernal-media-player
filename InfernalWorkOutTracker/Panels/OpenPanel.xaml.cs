﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Imp.Base;
using Imp.Base.Controllers;
using Imp.Base.FileData;
using Imp.Base.FileLoading;
using Imp.Base.Libraries;
using Imp.Base.ListLogic;
using Imp.Controls;
using Imp.Controls.Gui;
using Imp.Controls.SpecialFolder;
using InfernalWorkOutTracker.Controllers;

namespace Imp.Panels
{
    /// <summary>
    /// Interaction logic for OpenPanel.xaml
    /// </summary>
    public partial class OpenPanel : UserControl
    {
        private TrackerController mainC;
        private List<string> filterList = null; // store filters, reload only when required
        private Thread loader;
        private bool refreshing = false;

        public OpenPanel()
        {
            InitializeComponent();
        }


        public void SetStyles(IStyleLib styleLib, TrackerController mainController)
        {
            mainC = mainController;

            styleLib.SetStyle(ListFiles);
            styleLib.SetStyle(ListDirectories);

            styleLib.SetStyle(ListPlaces);


            styleLib.SetStyle(ButtonAddFolder, BtnNumber.AddFolder);
            styleLib.SetStyle(ButtonAddSubFolder, BtnNumber.AddSubFolder);
            styleLib.SetStyle(ButtonAddSelected, BtnNumber.AddFile);

            styleLib.SetStyle(ButtonFilterVideo, "video");
            styleLib.SetStyle(ButtonFilterMusic, "audio");
            styleLib.SetStyle(ButtonFilterPictures, "image");
            styleLib.SetStyle(ButtonFilterPlaylist, "playlist");
            styleLib.SetStyle(ButtonFilterStream, "stream");

            styleLib.SetStyle(ButtonClearPlaylist, BtnNumber.ClearList);

            styleLib.SetStyle(ButtonClosePanel, BtnNumber.Close);
            styleLib.SetStyle(ButtonMaximizePanel, BtnNumber.Maximize);
            styleLib.SetStyle(ButtonRefresh, BtnNumber.Refresh);

            styleLib.SetStyle(ButtonSort, "Name");
            ButtonSort.SetContent("Date", 1);

            Background = styleLib.GetGridBrush(false);
            styleLib.SetStyle(TextBoxFind);
            LabelTopic.Background = styleLib.GetGridBrush(true);
            LabelTopic.Foreground = styleLib.GetForeground();
        }


        private void ListFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var file = ListFiles.GetSelected();
            if (file != null)
                mainC.Exec(TrackerCommand.Open, file);
            //var files = ListFiles.GetSelectedList();
            //if (files.Count > 0)
            //    mainC.Exec(ImpCommand.OpenFile, files[0]);
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPlaces();
            ListPlaces.Select(0);
        }


        private void LoadPlaces()
        {
            var customPlaces = new DoubleString[mainC.Settings.CustomPaths.Count];
            for (int i = 0; i < mainC.Settings.CustomPaths.Count; i++)
            {
                customPlaces[i] = new DoubleString(mainC.Settings.CustomPaths[i],
                    StringHandler.GetFilename(mainC.Settings.CustomPaths[i]));
            }
            ListPlaces.LoadPlaces(customPlaces);
        }


        private void ButtonSort_Clicked(object sender)
        {
            ButtonSort.CurrentState++;
            ListFiles.SortMode = (FileSortMode) ButtonSort.CurrentState;
        }


        private void ButtonClosePanel_Clicked(object sender)
        {
            mainC.Exec(TrackerCommand.PanelOpen);
        }


        private void ListPlaces_SelectionChanged()
        {
            if (!refreshing)
            {
                var path = ListPlaces.GetSelected();
                if (path != null && !string.IsNullOrEmpty(path.Value))
                {
                    // valid path, update lists
                    ListDirectories.SetPath(path.Value);
                }
            }
        }


        private void ListDirectories_SelectionChanged()
        {
            if (ListDirectories.GetSelected() != null)
            {
                ListFiles.SetPath(ListDirectories.GetSelected().Value, filterList);
            }

        }


        public List<string> GetFilters()
        {
            var fileTypes = GetFileTypes();

            filterList = FileTypeFinder.GetFiltersList(fileTypes);

            if (filterList.Count > 0)
                return filterList;
            return null;
        }


        public FileTypes GetFileTypes()
        {
            var filter = FileTypes.Any;
            int filterCount = 0;
            if (ButtonFilterPlaylist.CurrentState != 0)
            {
                filter |= FileTypes.Playlist;
                filterCount++;
            }
            if (ButtonFilterVideo.CurrentState != 0)
            {
                filter |= FileTypes.Videos;
                filterCount++;
            }
            if (ButtonFilterMusic.CurrentState != 0)
            {
                filter |= FileTypes.Music;
                filterCount++;
            }
            if (ButtonFilterPictures.CurrentState != 0)
            {
                filter |= FileTypes.Pictures;
                filterCount++;
            }

            ListFiles.ColorCoding = (filterCount != 1);
            return filter;
        }


        private void ButtonFilter_Clicked(object sender)
        {
            (sender as ImpButton).CurrentState++;
            GetFilters();
            Refresh(null);
        }


        public void Refresh(object sender)
        {
            refreshing = true;
            LoadPlaces();

            ListDirectories.Refresh();

            refreshing = false;
        }


        private void TextBoxFind_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListFiles.FindText = TextBoxFind.Text;
        }


        private void ButtonAddSubFolder_Clicked(object sender)
        {
            PrepareFolderLoader(new DirectoryLoadOptions(ListDirectories.GetSelected().Value,
                SearchOption.AllDirectories,
                GetFileTypes(), TextBoxFind.Text));
        }


        private void ButtonAddFolder_Clicked(object sender)
        {
            PrepareFolderLoader(new DirectoryLoadOptions(ListDirectories.GetSelected().Value,
                SearchOption.TopDirectoryOnly,
                GetFileTypes(), TextBoxFind.Text));
        }


        public void PrepareFolderLoader(DirectoryLoadOptions options)
        {
            if (loader != null && loader.IsAlive)
            {
                mainC.EventC.SetEvent(new EventText("Folder load already active", 1, EventType.Delayed));
                return;
            }

            ButtonAddSubFolder.IsEnabled = false;
            ButtonAddFolder.IsEnabled = false;

            loader = new Thread(LoadDirectories);
            loader.Start(options);
        }


        public void ButtonAddSelected_Clicked(object sender)
        {
            var list = ListFiles.GetSelectedList();
            foreach (var fileImpInfo in list)
            {
                mainC.Exec(TrackerCommand.AddFile, fileImpInfo);
            }
        }


        private void LoadDirectories(object options)
        {
            var loadOptions = (DirectoryLoadOptions) options;
            AddFolderToPlayList(loadOptions, loadOptions.RootPath);
            Dispatcher.Invoke(FixFolderButtons);

        }


        private void FixFolderButtons()
        {
            ButtonAddSubFolder.IsEnabled = true;
            ButtonAddFolder.IsEnabled = true;
        }


        private void AddFolderToPlayList(DirectoryLoadOptions options, string path)
        {
            if (options.SearchOption == SearchOption.AllDirectories)
            {
                if (LoadSubDirectories(options, path)) return;
            }

            FileInfo[] fileInfos;
            if (StringHandler.IsSpecialFolder(path))
            {
                var specialFolder = SpecialFolderLoader.LoadSpecialFolder(path);
                fileInfos = new FileInfo[specialFolder.FilePaths.Count];
                for (int i = 0; i < specialFolder.FilePaths.Count; i++)
                    fileInfos[i] = new FileInfo(specialFolder.FilePaths[i]);
            }
            else
            {
                try
                {
                    //files = Directory.GetFiles(path);
                    var directoryInfo = new DirectoryInfo(path);

                    fileInfos = directoryInfo.GetFiles();
                }
                catch (Exception e)
                {
                    // doesn't matter why path choosing failed, no files available in this folder
                    ImpError error = new ImpError(ErrorType.FailedToOpenFolder, e.Message);
                    mainC.EventC.ShowError(error);
                    return;
                }

            }
            if (fileInfos.Length < 1)
                return;

            var files = options.FilterFiles(fileInfos, true);

            IComparer<FileImpInfo> comparer;
            switch ((FileSortMode) ButtonSort.CurrentState)
            {
                case FileSortMode.Name:
                    comparer = new ComparerFileName();
                    break;
                case FileSortMode.Date:
                    comparer = new ComparerFileDate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Array.Sort(files, comparer);

            foreach (var fileInfo in files)
            {
                if (options.PlayFirstFile)
                {
                    mainC.Exec(TrackerCommand.AddFile, fileInfo);

                    object[] args = { TrackerCommand.Open, fileInfo };
                    Dispatcher.BeginInvoke(new ExecDelegate<TrackerCommand>(mainC.Exec), DispatcherPriority.Normal, args);

                    options.PlayFirstFile = false;
                }
                else
                {
                    mainC.Exec(TrackerCommand.AddFile, fileInfo);
                }
            }
        }


        private bool LoadSubDirectories(DirectoryLoadOptions options, string path)
        {
            DirectoryInfo[] folderinfos;
            if (StringHandler.IsSpecialFolder(path))
            {
                var specialFolder = SpecialFolderLoader.LoadSpecialFolder(path);
                folderinfos = new DirectoryInfo[specialFolder.FolderPaths.Count];
                for (int i = 0; i < specialFolder.FolderPaths.Count; i++)
                    folderinfos[i] = new DirectoryInfo(specialFolder.FolderPaths[i]);
            }
            else
            {
                try
                {
                    var directoryInfo = new DirectoryInfo(path);

                    folderinfos = directoryInfo.GetDirectories();
                }
                catch (Exception e)
                {
                    ImpError error = new ImpError(ErrorType.FailedToOpenFolder, e.Message);
                    mainC.EventC.ShowError(error);
                    return true;
                }
            }


            foreach (var folderinfo in folderinfos)
            {
                AddFolderToPlayList(options, folderinfo.FullName);
            }
            return false;
        }


        private void ButtonClearPlaylist_Clicked(object sender)
        {
            //mainC.Exec(ImpCommand.ClearPlaylist);
        }




        private void ButtonMaximizePanel_Clicked(object sender)
        {
            mainC.Exec(TrackerCommand.PanelOpen, PanelCommand.MaxToggle);
        }
    }
}
