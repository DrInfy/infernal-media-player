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
            var id = (long)IdCipher.GetKeyFromText(fileInfo.SmartName);
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
            var id = (long)IdCipher.GetKeyFromText(fileInfo.SmartName);
            var nonClosed = db.Query<FileUsageData>("Select * FROM [FileUsageData] WHERE [FileInfoId] = ? AND [TimeClosed] IS NULL", id);

            foreach (var fileUsageData in nonClosed)
            {
                fileUsageData.TimeClosed = DateTime.UtcNow;
                fileUsageData.FileTimeClosed = mediaTime;
            }

            db.UpdateAll(nonClosed);
        }


        private static Guid GetGuid(string n1, string n2)
        {
            var firstPart = IdCipher.GetKeyFromText(n1);
            var secondPart = IdCipher.GetKeyFromText(n2);
            var guidBytes = new byte[128 / 8];
            var first = BitConverter.GetBytes(firstPart);
            var second = BitConverter.GetBytes(secondPart);

            for (int i = 0; i < first.Length; i++)
            {
                guidBytes[i] = first[i];
                guidBytes[i + first.Length] = second[i];
            }

            return new Guid(guidBytes);
        }
    }
}
