namespace Base.FileData.FileReading
{
    /// <summary>
    /// Class which creates an appropriate interface based on the file extension.
    /// </summary>
    internal abstract class AudioFile
    {

        protected double sBitrate;

        protected double sTotalSeconds;
        protected int sFrequency = 0;
        protected int sChannels = 0;

        protected long sSamples = 0;
        protected string sArtist = "";
        protected string sTitle = "";
        protected string sAlbum = "";

        protected int sTrack = -1;
        /// <summary>
        /// Gets the bitrate.
        /// </summary>
        public double Bitrate
        {
            get { return sBitrate; }
        }
        /// <summary>
        /// Gets the total seconds.
        /// </summary>
        public double TotalSeconds
        {
            get { return sTotalSeconds; }
        }
        /// <summary>
        /// Gets the artist if tag is available
        /// </summary>
        public string Artist
        {
            get { return sArtist; }
        }
        /// <summary>
        /// Gets the Title of the file if tag is available
        /// </summary>
        public string Title
        {
            get { return sTitle; }
        }
        /// <summary>
        /// Gets the track number is one is found
        /// </summary>
        public int Track
        {
            get { return sTrack; }
        }
        /// <summary>
        /// Gets the album name
        /// </summary>
        public string Album
        {
            get { return sAlbum; }
        }
    }
}