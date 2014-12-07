using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Interfaces
{
    public interface IMediaUriPlayer
    {
        /// <summary>
        /// Plays video or sound file
        /// </summary>
        void Play();
        /// <summary>
        /// Pauses playback, holding the stream in place
        /// </summary>
        void Pause();
        /// <summary>
        /// Stops playback and returns to position 0
        /// </summary>
        void Stop();

        /// <summary>
        /// Close down this instance of the media player controller
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets whether the player has an active media controller
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Gets current media duration in ticks.
        /// </summary>
        double Duration { get; }

        /// <summary>
        /// Gets or sets current media position in ticks
        /// </summary>
        double Position { get; set; }

        /// <summary>
        /// Gets or sets volume in percentages between 0 - 1
        /// </summary>
        double Volume { get; set; }

        /// <summary>
        /// Gets whether this player controller holds video data
        /// </summary>
        bool HasVideo { get; }
    }
}
