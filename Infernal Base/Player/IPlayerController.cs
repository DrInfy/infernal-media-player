using System;
using System.Windows;
using System.Windows.Media;

namespace Imp.Base.Player
{
    public interface IPlayerController
    {
        bool Released { get; }

        /// <summary>
        /// Read-only property for current media duration.
        /// </summary>
        long Duration { get; }

        long Position { get; }
        double Volume { get; set; }
        bool HasVideo { get; }
        Size VideoSize { get; }
        int SubtitleTrackIndex { get; set; }

        /// <summary>
        /// Notifies when the media has completed
        /// </summary>
        event Action MediaEnded;

        void HandleCommand();
        void Command(MediaCommand command);
        void OpenSource(out ImpError result);
        void AddFont(string fontFaceName, FontFamily fontFamily);
        void Activate();
        void MoveTo(long value);
    }
}