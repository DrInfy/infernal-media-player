using System;
using SQLite;

namespace Imp.Base.Data
{
    public class FileUsageData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public long FileInfoId { set; get; }

        public DateTime TimeOpened { set; get; }
        public DateTime? TimeClosed { set; get; }

        public TimeSpan? FileTimeClosed { set; get; }
    }
}