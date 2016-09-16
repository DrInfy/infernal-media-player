#region Usings

using System;
using DirectShowLib;
using MediaPlayer.Player;

#endregion

namespace MediaPlayer.Helpers
{
    internal static class FormatHelper
    {
        /// <summary>
        /// Converts a MediaPositionFormat enum to a DShow TimeFormat GUID
        /// </summary>
        internal static Guid ConvertPositionFormat(MediaPositionFormat positionFormat)
        {
            Guid timeFormat;

            switch (positionFormat)
            {
                case MediaPositionFormat.MediaTime:
                    timeFormat = TimeFormat.MediaTime;
                    break;
                case MediaPositionFormat.Frame:
                    timeFormat = TimeFormat.Frame;
                    break;
                case MediaPositionFormat.Byte:
                    timeFormat = TimeFormat.Byte;
                    break;
                case MediaPositionFormat.Field:
                    timeFormat = TimeFormat.Field;
                    break;
                case MediaPositionFormat.Sample:
                    timeFormat = TimeFormat.Sample;
                    break;
                default:
                    timeFormat = TimeFormat.None;
                    break;
            }

            return timeFormat;
        }

        /// <summary>
        /// Converts a DirectShow TimeFormat GUID to a MediaPositionFormat enum
        /// </summary>
        internal static MediaPositionFormat ConvertPositionFormat(Guid positionFormat)
        {
            MediaPositionFormat format;

            if (positionFormat == TimeFormat.Byte)
                format = MediaPositionFormat.Byte;
            else if (positionFormat == TimeFormat.Field)
                format = MediaPositionFormat.Field;
            else if (positionFormat == TimeFormat.Frame)
                format = MediaPositionFormat.Frame;
            else if (positionFormat == TimeFormat.MediaTime)
                format = MediaPositionFormat.MediaTime;
            else if (positionFormat == TimeFormat.Sample)
                format = MediaPositionFormat.Sample;
            else
                format = MediaPositionFormat.None;

            return format;
        }
    }
}