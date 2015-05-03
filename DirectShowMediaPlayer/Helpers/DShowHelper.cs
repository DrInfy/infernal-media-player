#region Usings

using System;

#endregion

namespace MediaPlayer.Helpers
{
    public static class DShowHelper
    {
        #region Static Fields and Constants

        /// <summary>
        /// The name of the default audio render.  This is the
        /// same on all versions of windows
        /// </summary>
        public const string DEFAULT_AUDIO_RENDERER_NAME = "Default DirectSound Device";

        /// <summary>
        /// The custom windows message constant for graph events
        /// </summary>
        private const int WM_GRAPH_NOTIFY = 0x0400 + 13;

        /// <summary>
        /// One second in 100ns units
        /// </summary>
        private const long DSHOW_ONE_SECOND_UNIT = 10000000;

        /// <summary>
        /// The IBasicAudio volume value for silence
        /// </summary>
        internal const int DSHOW_VOLUME_SILENCE = -10000;

        /// <summary>
        /// The IBasicAudio volume value for full volume
        /// </summary>
        internal const int DSHOW_VOLUME_MAX = 0;

        /// <summary>
        /// The IBasicAudio balance max absolute value
        /// </summary>
        private const int DSHOW_BALACE_MAX_ABS = 10000;

        /// <summary>
        /// Rate which our DispatcherTimer polls the graph, equals roughly 1/30 second
        /// </summary>
        internal const int DSHOW_TIMER_POLL_MS = 33;

        /// <summary>
        /// UserId value for the VMR9 Allocator - Not entirely useful
        /// for this application of the VMR
        /// </summary>
        private static readonly IntPtr m_userId = new IntPtr(unchecked((int) 0xDEADBEEF));

        #endregion

        public static int VolumePercentageToDirectShowVolume(double volumePercentage)
        {
            return (int) (volumePercentage * (DSHOW_VOLUME_MAX - DSHOW_VOLUME_SILENCE) + DSHOW_VOLUME_SILENCE);
        }

        public static double DirectShowVolumeToVolumePercentage(int directShowVolume)
        {
            return ((double) (directShowVolume - DSHOW_VOLUME_SILENCE))
                   / (DSHOW_VOLUME_MAX - DSHOW_VOLUME_SILENCE);
        }
    }
}