namespace Imp.DirectShow.Player
{
    public enum MediaCommand
    {
        Stop,
        Play,
        Pause,
        Close
    }

    public enum MediaStatus
    {
        NoFile,
        Opening,
        CancelingOpen,
        Moving,
        FrameStepping,
        Playing,
        Stopped,
        Closing,
        Paused
    }

    public enum MediaPositionFormat
    {
        MediaTime,
        Frame,
        Byte,
        Field,
        Sample,
        None
    }
}