#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using Imp.Base.FileData;
using Imp.Base.Libraries;
using Imp.Base.ListLogic;

#endregion

namespace Imp.Base.FileLoading
{
    public class DirectoryLoadOptions
    {
        #region Fields

        public string RootPath;
        public SearchOption SearchOption;
        public bool PlayFirstFile = false;
        public FilterOptions FilterOptions = FilterOptions.Files;
        private readonly List<string> filterList;
        private string findFilesText;
        private string findDirectoriesText;

        #endregion

        #region Properties

        public FindString[] FindFilesWords { get; private set; }
        public FindString[] FindDirectoriesWords { get; private set; }

        public string FindFilesText
        {
            get { return findFilesText; }
            set
            {
                findFilesText = value;
                FindFilesWords = string.IsNullOrWhiteSpace(findFilesText) ? null : StringHandler.GetFindWords(findFilesText);
            }
        }

        public string FindDirectoriesText
        {
            get { return findDirectoriesText; }
            set
            {
                findDirectoriesText = value;
                FindDirectoriesWords = string.IsNullOrWhiteSpace(findDirectoriesText) ? null : StringHandler.GetFindWords(findDirectoriesText);
            }
        }

        #endregion

        public DirectoryLoadOptions(string rootPath, SearchOption searchOption, FileTypes fileTypes)
        {
            RootPath = rootPath;
            SearchOption = searchOption;
            filterList = FileTypeFinder.GetFiltersList(fileTypes);
        }

        public DirectoryLoadOptions(string rootPath, SearchOption searchOption, FileTypes fileTypes, string findText)
        {
            RootPath = rootPath;
            SearchOption = searchOption;
            filterList = FileTypeFinder.GetFiltersList(fileTypes);
            FindFilesText = findText;
        }

        public FileImpInfo[] FilterFiles(FileInfo[] files, bool filter)
        {
            var fileInfos = LibImp.FilterFiles(files, filterList);
            if (FindFilesWords == null || !filter)
                return fileInfos;

            var added = 0;
            for (var i = 0; i < fileInfos.Length; i++)
            {
                if (StringHandler.FindFound(fileInfos[i].FileName, FindFilesWords))
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