using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base.Libraries;
using Base.ListLogic;

namespace Base.FileData
{
    public class FileTypeFinder
    {
        private readonly static List<FileTypeFinder> globalFileTypes = CreateExtensions();
        private readonly List<string> acceptableExtensions;
        private readonly FileTypes type;


        public FileTypeFinder(List<string> acceptableExtensions, FileTypes type)
        {
            this.acceptableExtensions = acceptableExtensions;
            this.type = type;
        }


        public FileTypes Type
        {
            get { return type; }
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
            var types = new List<FileTypeFinder>();
            types.Add(new FileTypeFinder(VideoFilters(), FileTypes.Videos));
            types.Add(new FileTypeFinder(MusicFilters(), FileTypes.Music));
            types.Add(new FileTypeFinder(PictureFilters(), FileTypes.Pictures));
            types.Add(new FileTypeFinder(PlaylistFilters(), FileTypes.Playlist));
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
    }
}
