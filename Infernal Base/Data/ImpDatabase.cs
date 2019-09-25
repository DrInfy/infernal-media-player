using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Edge.Tools.Cryptography;
using Imp.Base.FileData;
using SQLite;

namespace Imp.Base.Data
{
    public static class ImpDatabase
    {
        private static readonly string dbPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\ImpData.db";

        private static SQLiteConnection db;

        static ImpDatabase()
        {
            // Get an absolute path to the database file
            //var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ImpData.db");

            db = new SQLiteConnection(dbPath);
            db.CreateTable<FileData>();
            db.CreateTable<FileUsageData>();
        }

        public static void FileOpened(FileImpInfo fileInfo)
        {
            var id = fileInfo.SmartId;
            var data = db.Find<FileData>(id);

            if (data == null)
            {
                data = new FileData()
                {
                    Id = id,
                    SmartName = fileInfo.SmartName,
                    LastPath = fileInfo.FullPath
                };

                db.Insert(data);
            }
            else if (data.LastPath != fileInfo.FullPath)
            {
                db.Update(data);
            }

            var usageData = new FileUsageData
            {
                FileInfoId = id,
                TimeOpened = DateTime.UtcNow
            };

            db.Insert(usageData);
        }

        public static void FileClosed(FileImpInfo fileInfo, TimeSpan? mediaTime = null)
        {
            var id = fileInfo.SmartId;
            var nonClosed = db.Query<FileUsageData>("Select * FROM [FileUsageData] WHERE [FileInfoId] = ? AND [TimeClosed] IS NULL", id);

            foreach (var fileUsageData in nonClosed)
            {
                fileUsageData.TimeClosed = DateTime.UtcNow;
                fileUsageData.FileTimeClosed = mediaTime;
            }

            db.UpdateAll(nonClosed);
        }

        public static List<FileUsageData> FileUsages(long smartId)
        {
            var opened = db.Query<FileUsageData>("Select * FROM [FileUsageData] WHERE [FileInfoId] = ? AND [TimeClosed] IS NOT NULL", smartId);
            return opened;
        }

        public static void SetWatched(long smartId)
        {
            var usageData = new FileUsageData
            {
                FileInfoId = smartId,
                TimeOpened = DateTime.UtcNow,
                TimeClosed = DateTime.UtcNow,
                Completed = true
            };

            db.Insert(usageData);
        }

        public static void SetWatchedAll(IEnumerable<long> smartIds)
        {
            var list = new List<FileUsageData>();

            foreach (var smartId in smartIds)
            {
                var usageData = new FileUsageData
                {
                    FileInfoId = smartId,
                    TimeOpened = DateTime.UtcNow,
                    TimeClosed = DateTime.UtcNow,
                    Completed = true
                };

                list.Add(usageData);
            }

            db.InsertAll(list);
        }


        public static void RemoveWatched(IEnumerable<long> smartIds)
        {
            db.Execute($"DELETE FROM [FileUsageData] WHERE [FileInfoId] IN ({String.Join(",", smartIds)})");
        }
    }
}
