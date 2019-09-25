#region Usings

using System;

#endregion

namespace Imp.Base
{
    public enum LoopMode
    {
        NoLoop,
        LoopAll,
        LoopOne
    }


    public enum PanelCommand
    {
        None,
        Toggle,
        MaxToggle,
        Maximize,
        Normalize,
        Hide,
        Show
    }

    public enum MouseStates
    {
        None,
        PositionPressed,
        VolumePressed,
        PanLeftPressed,
        PanRightPressed,
        WindowPressed = 9,
        VideoPressedNotMoved = 10,
        VideoPressedMoved = 11
    }

    [Flags]
    public enum FileTypes
    {
        Any = 0,
        Videos = 1,
        Music = 2,
        Pictures = 4,
        Playlist = 256,
        PlaylistPictures = 512
    }


    public enum ContextMenuEnum
    {
        None,
        Playlist,
        FileList,
        FolderList,
        PlacesList
    }

    public enum ExtWindowState
    {
        Normal = 0,
        Minimized = 1,
        Maximized = 2,
        Fullscreen = 4
    }


    [Flags]
    public enum PlayerStyle
    {
        None = 0,
        MusicPlayer = 1,
        VideoPlayer = 2,
        MediaPlayer = 3,
        PictureViewer = 4,
        All = 7,
    }

    public enum FileSortMode
    {
        Name = 0,
        Date = 1,
        LastUsage = 2,
        Random,
        Path,
    }
}