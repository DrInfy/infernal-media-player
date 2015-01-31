using System;
using System.Collections.Generic;
using System.IO;
using Base.FileData;
using Base.Libraries;
using Base.ListLogic;

namespace Base.FileLoading
{
    public class DirectoryLoadOptions
    {
        public string RootPath;
        public SearchOption SearchOption;
        public bool PlayFirstFile = false;
        public FilterOptions FilterOptions = FilterOptions.Files;
        public FindString[] FindWords { get; private set; }
        private readonly List<string> filterList;
        private string findText;
        

        public DirectoryLoadOptions(string rootPath, SearchOption searchOption, FileTypes fileTypes)
        {
            RootPath = rootPath;
            this.SearchOption = searchOption;
            this.filterList = FileTypeFinder.GetFiltersList(fileTypes);
            FindText = findText;
        }

        public DirectoryLoadOptions(string rootPath, SearchOption searchOption, FileTypes fileTypes, string findText)
        {
            RootPath = rootPath;
            this.SearchOption = searchOption;
            this.filterList = FileTypeFinder.GetFiltersList(fileTypes);
            FindText = findText;
        }


        public string FindText
        {
            get { return findText; }
            set
            {
                findText = value;
                FindWords = string.IsNullOrWhiteSpace(findText) ? null : StringHandler.GetFindWords(findText);
            }
        }


        public FileImpInfo[] FilterFiles(FileInfo[] files, bool filter)
        {
            var fileInfos = LibImp.FilterFiles(files, filterList);
            if (FindWords == null || !filter)
                return fileInfos;

            int added = 0;
            for (int i = 0; i < fileInfos.Length; i++)
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