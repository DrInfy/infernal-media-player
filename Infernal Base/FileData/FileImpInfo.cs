#region Usings

using System;
using System.IO;
using Imp.Base.Libraries;

#endregion

namespace Imp.Base.FileData
{
    public class FileImpInfo
    {
        #region Fields

        public string FullPath;
        public string FileName;
        public string SmartName;
        public long DateModified;
        public FileTypes FileType = FileTypes.Any;
        public long DataSize;

        #endregion

        protected FileImpInfo() { }

        public FileImpInfo(ErrorType result)
        {
            this.FullPath = "";
            switch (result)
            {
                case ErrorType.FailedToOpenFolder:
                    this.FileName = "Failed to open folder";
                    break;
                case ErrorType.FailedToOpenFile:
                    this.FileName = "Failed to open file";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("result");
            }
        }

        public FileImpInfo(string path)
        {
            this.FullPath = path;
            this.FileName = StringHandler.GetFilename(path);
            this.SmartName = StringHandler.GetSmartName(this.FileName);
            CheckDate();
        }

        public FileImpInfo(FileInfo info)
        {
            this.FullPath = info.FullName;
            this.FileName = StringHandler.GetFilename(this.FullPath);
            this.SmartName = StringHandler.GetSmartName(this.FileName);
            this.DateModified = info.LastWriteTimeUtc.Ticks;
            this.DataSize = info.Length;
        }

        public override string ToString()
        {
            return this.FileName;
        }

        public void CheckDate()
        {
            try
            {
                var info = new FileInfo(this.FullPath);
                if (info.Exists)
                {
                    this.DateModified = info.LastWriteTimeUtc.Ticks;
                    this.DataSize = info.Length;
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}