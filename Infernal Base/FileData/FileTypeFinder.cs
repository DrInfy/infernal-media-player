#region Usings

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace Base.FileData
{
    public class FileTypeFinder
    {
        #region Fields

        private readonly List<string> acceptableExtensions;

        #endregion

        #region Properties

        public FileTypes Type { get; }

        #endregion

        public FileTypeFinder(List<string> acceptableExtensions, FileTypes type)
        {
            this.acceptableExtensions = acceptableExtensions;
            Type = type;
        }

        public bool IsFileType(string e)
        {
            return acceptableExtensions.Exists(
                extension => String.Compare(e, extension, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static List<string> GetFiltersList(FileTypes allowedFileTypes)
        {
            var filtersList = new List<string>();
            foreach (var filetype in globalFileTypes)
                if ((allowedFileTypes & filetype.Type) == filetype.Type)
                    filtersList.AddRange(filetype.acceptableExtensions);

            return filtersList;
        }

        /// <summary>
        /// Determines the correct filetype(s) that this file fits in
        /// </summary>
        /// <param name="path"> full path or filename with extension </param>
        /// <returns></returns>
        public static FileTypes DetermineFileType(string path)
        {
            var types = FileTypes.Any;
            var s = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(s)) return types;

            var extension = s.ToLowerInvariant();
            foreach (var filetype in globalFileTypes)
                if (filetype.IsFileType(extension))
                    types |= filetype.Type;

            return types;
        }

        public static List<FileTypeFinder> CreateExtensions()
        {
            var types = new List<FileTypeFinder>
            {
                new FileTypeFinder(VideoFilters(), FileTypes.Videos),
                new FileTypeFinder(MusicFilters(), FileTypes.Music),
                new FileTypeFinder(PictureFilters(), FileTypes.Pictures),
                new FileTypeFinder(PlaylistFilters(), FileTypes.Playlist)
            };
            return types;
        }

        private static List<string> VideoFilters()
        {
            var filters = new List<string>();
            filters.Add(".avi");
            filters.Add(".mpg");
            filters.Add(".mpeg");
            filters.Add(".ogm");
            filters.Add(".mkv");
            filters.Add(".divx");
            filters.Add(".mp2");
            filters.Add(".mp4");
            filters.Add(".mov");
            filters.Add(".asf");
            filters.Add(".wmv");
            filters.Add(".div");
            filters.Add(".rm");
            filters.Add(".m4v");
            return filters;
        }

        private static List<string> MusicFilters()
        {
            var filters = new List<string>();
            filters.Add(".ape");
            filters.Add(".flac");
            filters.Add(".mp3");
            filters.Add(".ogg");
            filters.Add(".wma");
            filters.Add(".wav");
            filters.Add(".m4a");
            filters.Add(".aac");
            return filters;
        }

        private static List<string> PictureFilters()
        {
            var filters = new List<string>();
            filters.Add(".jpg");
            filters.Add(".jpeg");
            filters.Add(".bmp");
            filters.Add(".tga");
            filters.Add(".tif");
            filters.Add(".png");
            filters.Add(".tiff");
            filters.Add(".gif");
            filters.Add(".ico");
            filters.Add(".icon");
            return filters;
        }

        private static List<string> PlaylistFilters()
        {
            var filters = new List<string>();
            filters.Add(".iml");
            return filters;
        }

        private static readonly List<FileTypeFinder> globalFileTypes = CreateExtensions();
    }
}