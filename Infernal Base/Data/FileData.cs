using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Imp.Base.Data
{
    public class FileData
    {
        [PrimaryKey]
        public long Id { set; get; }
        public string SmartName { get; set; }
        public string LastPath { get; set; }
    }
}
