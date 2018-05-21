namespace Imp.DirectShow.Player
{
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