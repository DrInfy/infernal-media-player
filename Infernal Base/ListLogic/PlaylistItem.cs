#region Usings

using System.IO;
using Imp.Base.FileData;
using Imp.Base.FileData.FileReading;
using Imp.Base.Libraries;

#endregion

namespace Imp.Base.ListLogic
{
    public class PlaylistItem: FileImpInfo
    {
        #region Fields

        public long DateModified;

        #endregion

        #region Properties

        public string AdditionalInfo { get; set; }
        public int Bitrate { get; set; }
        public string Name { get; set; }
        public bool Playing { get; set; }

        #endregion

        public PlaylistItem(string path) : base(path)
        {
            Name = StringHandler.RemoveExtension(this.FileName);
        }

        public PlaylistItem(FileInfo info): base(info)
        {
            Name = StringHandler.RemoveExtension(this.FileName);
        }

        public PlaylistItem(FileImpInfo info)
        {
            CopyFileInfo(info);
        }

        private void CopyFileInfo(FileImpInfo info)
        {
            this.SmartId = info.SmartId;
            this.FileType = info.FileType;
            this.FullPath = info.FullPath;
            this.FileName = info.FileName;
            this.LastUsage = info.LastUsage;
            SmartName = info.SmartName;
            DateModified = info.DateModified;
            Name = StringHandler.RemoveExtension(info.FileName);
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// This method reads file info from files, things like ID3v2 in mp3 files and FLAC fileinfo.
        /// </summary>
        public void ReadFileData()
        {
            var extension = Path.GetExtension(this.FullPath)?.ToLowerInvariant();
            if (extension == ".mp3")
            {
                var mp3Reader = new Mp3(this.FullPath);
                if (string.IsNullOrWhiteSpace(mp3Reader.Artist) || string.IsNullOrWhiteSpace(mp3Reader.Title))
                {
                    SetDefaultSongSmartName();
                    return;
                }

                SmartName = mp3Reader.Artist + " - " + mp3Reader.Album + " - " + mp3Reader.Track + " " + mp3Reader.Title;
                ArtistTitleToName(mp3Reader.Artist, mp3Reader.Title, mp3Reader.Track);
            }
            else if (extension == ".flac" || extension == ".ogg")
            {
                var mp3Reader = new Mp3(this.FullPath);
                var flacReader = new FlacOgg(this.FullPath);
                if (!string.IsNullOrWhiteSpace(mp3Reader.Title))
                {
                    SmartName = mp3Reader.Artist + " - " + mp3Reader.Album + " - " + mp3Reader.Track + " " + mp3Reader.Title;
                    ArtistTitleToName(mp3Reader.Artist, mp3Reader.Title, mp3Reader.Track);
                }
                else if (!string.IsNullOrWhiteSpace(flacReader.Title))
                {
                    SmartName = flacReader.Artist + " - " + flacReader.Album + " - " + flacReader.Track + " " + flacReader.Title;
                    ArtistTitleToName(flacReader.Artist, flacReader.Title, flacReader.Track);
                }
                else
                {
                    SetDefaultSongSmartName();
                }
            }
            else if (extension == ".wma")
            {
                var asfReader = new Asf(this.FullPath);
                if (string.IsNullOrWhiteSpace(asfReader.Artist) || string.IsNullOrWhiteSpace(asfReader.Title))
                {
                    SetDefaultSongSmartName();
                    return;
                }
                SmartName = asfReader.Artist + " - " + asfReader.Album + " - " + asfReader.Track + " " + asfReader.Title;
                ArtistTitleToName(asfReader.Artist, asfReader.Title, asfReader.Track);
            }

            else if (extension == ".wmv")
            {
                var asfReader = new Asf(this.FullPath);
                if (string.IsNullOrWhiteSpace(asfReader.Artist) || string.IsNullOrWhiteSpace(asfReader.Title)) return;
                ArtistTitleToName(asfReader.Artist, asfReader.Title, asfReader.Track);
            }

            else if (extension == ".m4a")
            {
                var m4AReader = new M4A(this.FullPath);
                if (string.IsNullOrWhiteSpace(m4AReader.Artist) || string.IsNullOrWhiteSpace(m4AReader.Title)) return;

                SmartName = m4AReader.Artist + " - " + m4AReader.Album + " - " + m4AReader.Track + " " + m4AReader.Title;
                ArtistTitleToName(m4AReader.Artist, m4AReader.Title, m4AReader.Track);
            }
        }

        public void SetDefaultSongSmartName()
        {
            var folderName = Path.GetDirectoryName(this.FullPath);
            SmartName = folderName + " " + SmartName;
        }

        /// <summary>
        /// Converts File information according to what we know of the file to a single like
        /// name for the file
        /// </summary>
        /// <param name="Artist"></param>
        /// <param name="Title"></param>
        /// <param name="Track"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private void ArtistTitleToName(string Artist, string Title, int Track)
        {
            Artist = Artist.Trim();
            Title = Title.Trim();
            string trackText;
            trackText = Track < 1 ? "" : Track.ToString();

            if (!string.IsNullOrEmpty(Artist) & !string.IsNullOrEmpty(Title))
            {
                Name = Artist + " - " + Title;
            }
            else if (!string.IsNullOrEmpty(Title))
            {
                Name = Title;
            }
            else if (!string.IsNullOrEmpty(Artist) & !string.IsNullOrEmpty(trackText))
            {
                Name = Artist + " - Track " + Track;
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) || this.FullPath == (obj as PlaylistItem)?.FullPath;
        }
    }
}