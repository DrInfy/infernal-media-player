#region Usings

using System;
using System.IO;
using System.Linq;
using Edge.Tools.Cryptography;
using Imp.Base.Data;
using Imp.Base.Libraries;

#endregion

namespace Imp.Base.FileData
{
    public class FileImpInfo
    {
        #region Fields

        public long SmartId { get; protected set; }
        public string FullPath;
        public string FileName;
        public string SmartName;
        public long DateModified;
        public FileTypes FileType = FileTypes.Any;
        public long DataSize;
        public DateTime? LastUsage;

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
            this.SmartId = (long)IdCipher.GetKeyFromText(this.SmartName);
            this.FileType = FileTypeFinder.DetermineFileType(this.FileName);

            CheckDate();
            FindLastUsage();
        }


        public FileImpInfo(FileInfo info)
        {
            this.FullPath = info.FullName;
            this.FileName = StringHandler.GetFilename(this.FullPath);
            this.SmartName = StringHandler.GetSmartName(this.FileName);
            this.DateModified = info.LastWriteTimeUtc.Ticks;
            this.DataSize = info.Length;
            this.SmartId = (long)IdCipher.GetKeyFromText(this.SmartName);
            this.FileType = FileTypeFinder.DetermineFileType(this.FileName);
            FindLastUsage();
        }

        public void FindLastUsage()
        {
            var usages = ImpDatabase.FileUsages(this.SmartId);
            if (usages.Count == 0) { return; }

            if (this.FileType == FileTypes.Pictures)
            {
                var minTime = TimeSpan.FromSeconds(2);
                this.LastUsage = usages.Where(x => x.Completed
                                                   || (x.TimeClosed != null && x.TimeClosed.Value - x.TimeOpened > minTime))
                    .OrderBy(x => x.TimeOpened).FirstOrDefault()?.TimeOpened;
            }
            else if (this.FileType == FileTypes.Music)
            {
                var minTime = TimeSpan.FromSeconds(30);
                this.LastUsage = usages.Where(x => x.Completed
                                                   || (x.TimeClosed != null && x.TimeClosed.Value - x.TimeOpened > minTime))
                    .OrderBy(x => x.TimeOpened).FirstOrDefault()?.TimeOpened;
            }
            else if (this.FileType == FileTypes.Videos)
            {
                var minTime = TimeSpan.FromMinutes(2);
                var minFileTime = TimeSpan.FromMinutes(5);
                this.LastUsage = usages.Where(x => x.Completed 
                                                   || (x.TimeClosed != null && x.TimeClosed.Value - x.TimeOpened > minTime && x.FileTimeClosed > minFileTime))
                    .OrderBy(x => x.TimeOpened).FirstOrDefault()?.TimeOpened;
            }
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