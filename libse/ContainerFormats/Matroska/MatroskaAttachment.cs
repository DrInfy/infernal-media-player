using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nikse.SubtitleEdit.Core.ContainerFormats.Matroska
{
    public class MatroskaAttachment
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string Uid { get; set; }
        public byte[] Data { get; set; }
    }
}
