namespace Imp.Base.Commands
{
    public enum ImpCommand
    {
        None = 0,
        Rewind = 1,
        Fastforward = 2,
        Play,
        Playpause,
        GlobalPlaypause, // pauses every mediaplayer, pause/plays only the most recent one
        Pause,
        StopFile,

        AddFile,

        /// <summary>Open from a file path</summary>
        OpenFile,

        /// <summary>Open from a playlist</summary>
        Open,
        OpenNext,
        OpenPrev,

        SetPosition,
        ChangePosition,

        /// <summary>Pictures only</summary>
        SetDuration,

        TimescaleUp,
        TimescaleDown,

        VolumeMute,
        SetVolume,
        VolumeChange,

        /// <summary>Pictures only</summary>
        SetZoom,

        /// <summary>Pictures only</summary>
        ChangeZoom,

        /// <summary>Pictures only</summary>
        ToggleZoom,

        AlwaysOnTop,
        CopyName,

        PanelOpen,
        PanelSettings,
        PanelHelp,
        PanelPlaylist,

        PlayerMinimize,
        ToggleMaximize,
        ToggleFullscreen,
        Exit,

        ClearPlaylist,

        HideBars,

        Fullscreen,

        Find,

        ShowFilepath,


        LoopChange,

        AddfastFile,

        PlaySelected,
        RemoveSelected,

        Sort,

        DeletePlFiles,


        SavePlaylist,

        OpenPl,

        ShowInExplorer,

        TimescaleSet,

        SelectAll,
        SelectInverse,

        TogglePanels,

        FrameSkip5,
        FrameNext,

        OpenRandom,

        Pankeys,
        PanReset,
        ResizeReset,

        PanLeft,
        PanUp,
        PanRight,
        PanDown,

        PanLeftup,
        PanRightup,
        PanRightdown,
        PanLeftdown,

        ResizeLeft,
        ResizeUp,
        ResizeRight,
        ResizeDown,

        ResizeLeftup,
        ResizeRightup,
        ResizeRightdown,
        ResizeLeftdown,

        PlaySelectedOpenFiles,
        AddSelectedOpenFiles,
        DeleteOpenFiles,

        PlaySelectedFolderFiles,
        AddSelectedFolderFiles,
        AddSelectedFolderFolders,
        AddSelectedFolderToPaths,
        DeleteOpenFolder,

        RemoveSelectedPath,
        Shuffle
    }
}