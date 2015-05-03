#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using Base.FileData;
using Base.Libraries;
using Base.ListLogic;

#endregion

namespace Base.FileLoading
{
    public class DirectoryLoadOptions
    {
        #region Fields

        public string RootPath;
        public SearchOption SearchOption;
        public bool PlayFirstFile = false;
        public FilterOptions FilterOptions = FilterOptions.Files;
        private readonly List<string> filterList;
        private string findText;

        #endregion

        #region Properties

        public FindString[] FindWords { get; private set; }

        public string FindText
        {
            get { return findText; }
            set
            {
                findText = value;
                FindWords = string.IsNullOrWhiteSpace(findText) ? null : StringHandler.GetFindWords(findText);
            }
        }

        #endregion

        public DirectoryLoadOptions(string rootPath, SearchOption searchOption, FileTypes fileTypes)
        {
            RootPath = rootPath;
            SearchOption = searchOption;
            filterList = FileTypeFinder.GetFiltersList(fileTypes);
            FindText = findText;
        }

        public DirectoryLoadOptions(string rootPath, SearchOption searchOption, FileTypes fileTypes, string findText)
        {
            RootPath = rootPath;
            SearchOption = searchOption;
            filterList = FileTypeFinder.GetFiltersList(fileTypes);
            FindText = findText;
        }

        public FileImpInfo[] FilterFiles(FileInfo[] files, bool filter)
        {
            var fileInfos = LibImp.FilterFiles(files, filterList);
            if (FindWords == null || !filter)
                return fileInfos;

            var added = 0;
            for (var i = 0; i < fileInfos.Length; i++)
            {
                if (StringHandler.FindFound(fileInfos[i].Name, FindWords))
                {
                    fileInfos[added] = fileInfos[i];
                    added++;
                }
            }

            // resize extras away
            Array.Resize(ref fileInfos, added);
            return fileInfos;
        }
    }
}