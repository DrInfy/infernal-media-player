using System.Collections.Generic;
using System.IO;
using Base.Libraries;
using Base.ListLogic;
using Microsoft.WindowsAPICodePack.Shell;

namespace ImpControls.SpecialFolder
{
    public static class SpecialFolderLoader
    {
        
        public const string VideoFolderName = "Videos";
        public const string MusicFolderName = "Music";
        public const string DownloadFolderName = "Downloads";


        public static SpecialFolderContent LoadSpecialFolder(string pathData)
        {
            if (!StringHandler.IsSpecialFolder(pathData)) return null;

            var folderContent = new SpecialFolderContent(pathData);
            
            if (System.String.Compare(pathData, "$" + VideoFolderName, System.StringComparison.Ordinal) == 0)
            {
                var folders = (NonFileSystemKnownFolder)KnownFolders.VideosLibrary;
                LoadSpecialFolder(folders, folderContent);
            }
            else if (System.String.Compare(pathData, "$" + MusicFolderName, System.StringComparison.Ordinal) == 0)
            {
                var folders = (NonFileSystemKnownFolder)KnownFolders.MusicLibrary;
                LoadSpecialFolder(folders, folderContent);
            }
            else if (System.String.Compare(pathData, "$" + DownloadFolderName, System.StringComparison.Ordinal) == 0)
            {
                var folders = (FileSystemKnownFolder)KnownFolders.Downloads;
                LoadSpecialFolder(folders, folderContent);
            }
            return folderContent;
        }


        private static void LoadSpecialFolder(IEnumerable<ShellObject> folders, SpecialFolderContent folderContent)
        {
            foreach (var shellObject in folders)
            {
                var shellFile = shellObject as ShellFile;
                var shellFolder = shellObject as FileSystemKnownFolder;
                if (shellFile != null)
                    folderContent.FilePaths.Add(shellFile.ParsingName);
                else if (shellFolder != null)
                {
                    // prevents .zip files from being added as imp is unable to handle .zip files
                    if (!Path.HasExtension(shellFolder.ParsingName))
                        folderContent.FolderPaths.Add(shellFolder.ParsingName);
                }
                    
            }
        }
    }
}
