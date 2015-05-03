#region Usings

using System;
using System.IO;
using Base.Libraries;

#endregion

namespace Base.FileData
{
    public class FileImpInfo
    {
        #region Fields

        public string Path;
        public string Name;
        public string SmartName;
        public long DateModified;
        public FileTypes FileType = FileTypes.Any;

        #endregion

        public FileImpInfo(ErrorType result)
        {
            Path = "";
            switch (result)
            {
                case ErrorType.FailedToOpenFolder:
                    Name = "Failed to open folder";
                    break;
                case ErrorType.FailedToOpenFile:
                    Name = "Failed to open file";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("result");
            }
        }

        public FileImpInfo(string path)
        {
            Path = path;
            Name = StringHandler.GetFilename(path);
            SmartName = StringHandler.GetSmartName(Name);
            CheckDate();
            //DateModified = dateModified;
        }

        public FileImpInfo(FileInfo info)
        {
            Path = info.FullName;
            Name = StringHandler.GetFilename(Path);
            SmartName = StringHandler.GetSmartName(Name);
            DateModified = info.LastWriteTimeUtc.Ticks;
        }

        public override string ToString()
        {
            return Name;
        }

        public void CheckDate()
        {
            try
            {
                var info = new FileInfo(Path);
                if (info.Exists)
                    DateModified = info.LastWriteTimeUtc.Ticks;
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}