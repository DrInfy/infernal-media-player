namespace Base.FileData.FileReading
{
    /// <summary>
    /// Class which creates an appropriate interface based on the file extension.
    /// </summary>
    internal abstract class AudioFile
    {
        #region Fields

        protected int sFrequency = 0;
        protected int sChannels = 0;
        protected long sSamples = 0;
        protected int sTrack = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bitrate.
        /// </summary>
        public double Bitrate { get; protected set; }

        /// <summary>
        /// Gets the duration in total seconds.
        /// </summary>
        public double TotalSeconds { get; protected set; }

        /// <summary>
        /// Gets the artist if tag is available
        /// </summary>
        public string Artist { get; protected set; } = "";

        /// <summary>
        /// Gets the Title of the file if tag is available
        /// </summary>
        public string Title { get; protected set; } = "";

        /// <summary>
        /// Gets the track number is one is found
        /// </summary>
        public int Track => sTrack;

        /// <summary>
        /// Gets the album name
        /// </summary>
        public string Album { get; protected set; } = "";

        #endregion
    }
}