using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using Base.Commands;
using Base.FileData;
using Base.Interfaces;
using Base.Libraries;
using Base.ListLogic;
using Imp.Controllers;

namespace Base.Controllers
{
    public delegate void ExecDelegate<in T>(T cmd, object argument = null);

    public abstract class BaseController : KeyboardController<ImpCommand>
    {
        public IPanelController PanelC;
        public MediaController MediaC;
        public Settings Settings;

        private readonly string settingsPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\settings.xml";

        private long lastExitAttempt = 0;
        protected PlaylistItem loadingItem;
        protected PlaylistItem playingItem;
        protected PlaylistItem itemOnPlayer;


        protected BaseController()
        {
            try
            {
                Settings = (Settings)ImpSerializer.ReadFile(settingsPath, typeof(Settings));
            }
            catch (Exception e)
            {
                Settings = new Settings();
            }
        }

        protected void Initialize(IEventController eventC, IPanelController panelC, MediaController mediaC)
        {
            EventC = eventC;
            MediaC = mediaC;
            PanelC = panelC;

            Initialize(true);
        }

        public abstract List<PlaylistItem> GetSelectedPlaylistItems();

        public abstract bool IsMostRecentInstance { get; }

        public void ContextMenu(Point cursorPositionInDesktop, ContextMenuEnum menuEnumPosition)
        {
            var cmdList = new List<ImpTextAndCommand>();
            switch (menuEnumPosition)
            {
                case ContextMenuEnum.None:
                    cmdList.Add(new ImpTextAndCommand("Play Next", ImpCommand.OpenNext));
                    cmdList.Add(new ImpTextAndCommand("Play Previous", ImpCommand.OpenPrev));
                    cmdList.Add(new ImpTextAndCommand("Play Random", ImpCommand.OpenRandom));
                    cmdList.Add(new ImpTextAndCommand("Copy to clipboard", ImpCommand.CopyName));
                    cmdList.Add(new ImpTextAndCommand("Shuffle playlist", ImpCommand.Shuffle));
                    break;
                case ContextMenuEnum.Playlist:
                    cmdList.Add(new ImpTextAndCommand("Play this", ImpCommand.OpenPl));
                    cmdList.Add(new ImpTextAndCommand("Remove selected", ImpCommand.RemoveSelected));
                    cmdList.Add(new ImpTextAndCommand("Sort files", ImpCommand.Sort));
                    cmdList.Add(new ImpTextAndCommand("Sort files by their paths", ImpCommand.SortBypath));
                    cmdList.Add(new ImpTextAndCommand("Randomize file order", ImpCommand.SortByrandom));
                    cmdList.Add(new ImpTextAndCommand("------------", ImpCommand.None));
                    cmdList.Add(new ImpTextAndCommand("Open path in file browser", ImpCommand.ShowInExplorer));
                    cmdList.Add(new ImpTextAndCommand("Delete Selected Files", ImpCommand.DeletePlFiles));
                    break;
                case ContextMenuEnum.FileList:
                    cmdList.Add(new ImpTextAndCommand("Play selected files", ImpCommand.PlaySelectedOpenFiles));
                    cmdList.Add(new ImpTextAndCommand("Add selected files", ImpCommand.AddSelectedOpenFiles));
                    cmdList.Add(new ImpTextAndCommand("------------", ImpCommand.None));
                    cmdList.Add(new ImpTextAndCommand("Delete Selected Files", ImpCommand.DeleteOpenFiles));
                    break;
                case ContextMenuEnum.FolderList:
                    cmdList.Add(new ImpTextAndCommand("Play Folder", ImpCommand.PlaySelectedFolderFiles));
                    cmdList.Add(new ImpTextAndCommand("Add files inside folder", ImpCommand.AddSelectedFolderFiles));
                    cmdList.Add(new ImpTextAndCommand("Add files and folders in folder", ImpCommand.AddSelectedFolderFolders));
                    cmdList.Add(new ImpTextAndCommand("------------", ImpCommand.None));
                    cmdList.Add(new ImpTextAndCommand("Remember selected folder", ImpCommand.AddSelectedFolderToPaths));
                    //cmdList.Add(new ImpTextAndCommand("Delete Selected Folder (NYI)", ImpCommand.DeleteOpenFolder));
                    break;
                case ContextMenuEnum.PlacesList:
                    cmdList.Add(new ImpTextAndCommand("Play Folder", ImpCommand.PlaySelectedFolderFiles));
                    cmdList.Add(new ImpTextAndCommand("Add files inside folder", ImpCommand.AddSelectedFolderFiles));
                    cmdList.Add(new ImpTextAndCommand("Add files and folders in folder", ImpCommand.AddSelectedFolderFolders));
                    cmdList.Add(new ImpTextAndCommand("------------", ImpCommand.None));
                    cmdList.Add(new ImpTextAndCommand("Remove selected path", ImpCommand.RemoveSelectedPath));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("menuEnumPosition");
            }
            CreateContextMenu(cursorPositionInDesktop, cmdList);
        }


        protected abstract void CreateContextMenu(Point cursorPositionInDesktop, List<ImpTextAndCommand> cmdList);


        public override void Exec(ImpCommand cmd, object argument = null)
        {
            switch (cmd)
            {
                case ImpCommand.None:
                    break;
                case ImpCommand.Rewind:
                    MediaC.Rewind();
                    break;
                case ImpCommand.Fastforward:
                    MediaC.Fastforward();
                    break;
                case ImpCommand.Play:
                    MediaC.Play();
                    break;
                case ImpCommand.GlobalPlaypause:
                    if (IsMostRecentInstance)
                        MediaC.PlayPause();
                    else
                        MediaC.Pause();
                    break;
                case ImpCommand.Playpause:
                    MediaC.PlayPause();
                    break;
                case ImpCommand.Pause:
                    MediaC.Pause();
                    break;
                case ImpCommand.StopFile:
                    MediaC.Stop();
                    break;
                case ImpCommand.AddFile:
                    AddFile(argument as FileImpInfo);
                    break;
                case ImpCommand.OpenFile:
                    OpenFile(argument as FileImpInfo);
                    break;
                case ImpCommand.Open:
                    OpenFile(argument as PlaylistItem);
                    break;
                case ImpCommand.OpenNext:
                    OpenNext();
                    break;
                case ImpCommand.OpenPrev:
                    OpenPrev();
                    break;
                case ImpCommand.SetPosition:
                    MediaC.SetPosition((double) argument);
                    break;
                case ImpCommand.ChangePosition:
                    break;
                case ImpCommand.SetDuration:
                    break;
                case ImpCommand.TimescaleUp:
                    break;
                case ImpCommand.TimescaleDown:
                    break;
                case ImpCommand.VolumeMute:
                    MediaC.ToggleMute();
                    break;
                case ImpCommand.SetVolume:
                    MediaC.SetVolume((double) argument);
                    break;
                case ImpCommand.VolumeChange:
                    MediaC.SetVolume(MediaC.Volume + (double) argument);
                    break;
                case ImpCommand.LoopChange:
                    MediaC.ToggleLoop();
                    break;
                case ImpCommand.SetZoom:
                    break;
                case ImpCommand.ChangeZoom:
                    break;
                case ImpCommand.ToggleZoom:
                    break;
                case ImpCommand.AlwaysOnTop:
                    break;
                case ImpCommand.CopyName:
                    if (playingItem != null)
                        Clipboard.SetText(playingItem.Name);
                    break;
                case ImpCommand.PanelOpen:
                    PanelC.CommandPanelOpen(argument as PanelCommand?);
                    break;
                case ImpCommand.PanelSettings:
                    break;
                case ImpCommand.PanelHelp:
                    break;
                case ImpCommand.PanelPlaylist:
                    PanelC.CommandPanelPlaylist(argument as PanelCommand?);
                    break;
                case ImpCommand.PlayerMinimize:
                    Minimize();
                    break;
                case ImpCommand.ToggleMaximize:
                    ToggleMaximize();
                    break;
                case ImpCommand.ToggleFullscreen:
                    ToggleFullscreen();
                    break;
                case ImpCommand.Exit:
                    Exit();
                    break;
                case ImpCommand.ClearPlaylist:
                    ClearPlayList();
                    break;
                case ImpCommand.HideBars:
                    break;
                case ImpCommand.Fullscreen:
                    break;
                case ImpCommand.Find:
                    break;
                case ImpCommand.ShowFilepath:
                    break;
                case ImpCommand.Pankeys:
                    break;
                case ImpCommand.PanReset:
                    break;
                case ImpCommand.ResizeReset:
                    break;
                case ImpCommand.PanLeft:
                    break;
                case ImpCommand.PanUp:
                    break;
                case ImpCommand.PanRight:
                    break;
                case ImpCommand.PanDown:
                    break;
                case ImpCommand.PanLeftup:
                    break;
                case ImpCommand.PanRightup:
                    break;
                case ImpCommand.PanRightdown:
                    break;
                case ImpCommand.PanLeftdown:
                    break;
                case ImpCommand.ResizeLeft:
                    break;
                case ImpCommand.ResizeUp:
                    break;
                case ImpCommand.ResizeRight:
                    break;
                case ImpCommand.ResizeDown:
                    break;
                case ImpCommand.ResizeLeftup:
                    break;
                case ImpCommand.ResizeRightup:
                    break;
                case ImpCommand.ResizeRightdown:
                    break;
                case ImpCommand.ResizeLeftdown:
                    break;
                case ImpCommand.PlaySelected:
                    break;
                case ImpCommand.RemoveSelected:
                    RemoveSelectedFromPlaylist();
                    break;
                case ImpCommand.Sort:
                    PlaylistSort(FileSortMode.Name);
                    break;
                case ImpCommand.SortBypath:
                    PlaylistSort(FileSortMode.Path);
                    break;
                case ImpCommand.SortByrandom:
                    PlaylistSort(FileSortMode.Random);
                    break;
                case ImpCommand.DeletePlFiles:
                    RequestPlaylistFileDeletion();
                    break;
                case ImpCommand.SavePlaylist:
                    break;
                case ImpCommand.OpenPl:
                    OpenSelectedInPlayList();
                    break;
                case ImpCommand.ShowInExplorer:
                    OpenSelectedInExplorer();
                    break;
                case ImpCommand.TimescaleSet:
                    break;
                case ImpCommand.SelectAll:
                    break;
                case ImpCommand.SelectInverse:
                    break;
                case ImpCommand.TogglePanels:
                    break;
                case ImpCommand.FrameSkip5:
                    break;
                case ImpCommand.FrameNext:
                    break;
                case ImpCommand.OpenRandom:
                    OpenRandom();
                    break;
                case ImpCommand.PlaySelectedOpenFiles:
                    PlaySelectedOpenFiles();
                    break;
                case ImpCommand.AddSelectedOpenFiles:
                    AddSelectedOpenFiles();
                    break;
                case ImpCommand.DeleteOpenFiles:
                    RequestDeleteOpenFiles();
                    break;
                case ImpCommand.PlaySelectedFolderFiles:
                    PlaySelectedFolderFiles();
                    break;
                case ImpCommand.AddSelectedFolderFiles:
                    AddSelectedFolderFiles();
                    break;
                case ImpCommand.AddSelectedFolderFolders:
                    AddSelectedFolderFolders();
                    break;
                case ImpCommand.AddSelectedFolderToPaths:
                    AddSelectedFolderToPaths();
                    break;
                case ImpCommand.DeleteOpenFolder:
                    DeleteOpenFolder();
                    break;
                case ImpCommand.RemoveSelectedPath:
                    RemoveSelectedPath();
                    break;
                case ImpCommand.Shuffle:
                    Shuffle();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("cmd");
            }
        }

        protected abstract void Shuffle();

        protected abstract void ClearPlayers();


        protected abstract void RemoveSelectedPath();


        protected abstract void DeleteOpenFolder();


        protected abstract void AddSelectedFolderToPaths();


        protected abstract void AddSelectedFolderFolders();


        protected abstract void AddSelectedFolderFiles();


        protected abstract void PlaySelectedFolderFiles();


        protected abstract void RequestDeleteOpenFiles();


        protected abstract void AddSelectedOpenFiles();


        protected abstract void PlaySelectedOpenFiles();


        protected abstract void RequestPlaylistFileDeletion();


        protected abstract void OpenSelectedInExplorer();


        protected abstract void OpenSelectedInPlayList();


        protected abstract void PlaylistSort(FileSortMode fileSortMode);


        protected abstract void RemoveSelectedFromPlaylist();


        protected abstract void OpenRandom();


        protected abstract void OpenPrev();


        protected abstract void OpenNext();


        protected abstract void ClearPlayList();

        protected abstract void AddFile(FileImpInfo fileImpInfo);

        protected abstract void ToggleMaximize();

        protected abstract void ToggleFullscreen();

        protected abstract void Minimize();


        protected void Exit()
        {
            if (lastExitAttempt < DateTime.Now.Ticks - 2 * LibImp.SecondToTicks)
            {
                // more than 2 seconds have passed since exit was last pressed
                EventC.SetEvent(new EventText("Press again to exit"));
                lastExitAttempt = DateTime.Now.Ticks;
            }
            else
            {
                // double pressed, time to quit
                MediaC.FreePlayer();
                UpdateSettings();
                WriteSettings(settingsPath, Settings);
                CloseWindows();
                Application.Current.Shutdown();
            }
        }

        //protected abstract void ApplySettings();

        protected abstract void UpdateSettings();


        private void WriteSettings(string path, Settings settings)
        {
            try
            {
                ImpSerializer.WriteFileXml(path, settings, settings.GetType());
            }
            catch
            {
                return;
            }
        }


        protected abstract void CloseWindows();

        protected abstract void OpenFile(PlaylistItem item);

        protected abstract void OpenFile(FileImpInfo file);
        public abstract void MediaEnded();
    }
}
