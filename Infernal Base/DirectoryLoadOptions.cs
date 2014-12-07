using System;
using System.Collections.Generic;
using System.IO;
using Base.FileData;
using Base.Libraries;
using Base.ListLogic;

namespace Base
{
    public class DirectoryLoadOptions
    {
        public string RootPath;
        public SearchOption SearchOption;
        public bool PlayFirstFile = false;
        private readonly List<string> filterList;
        private FindString[] findWords;
        private string findText;


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
            private set
            {
                findText = value;
                if (!string.IsNullOrWhiteSpace(findText))
                    findWords = StringHandler.GetFindWords(findText);
                else
                {
                    findWords = null;
                }
            }
        }


        public FileImpInfo[] FilterFiles(FileInfo[] files)
        {
            var fileInfos = LibImp.FilterFiles(files, filterList);
            if (findWords == null)
                return fileInfos;

            int added = 0;
            for (int i = 0; i < fileInfos.Length; i++)
            {
                bool Found = StringHandler.FindFound(fileInfos[i].Path, findWords);

                if (Found)
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