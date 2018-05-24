using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imp.Base;
using Mpv.NET;

namespace Imp.MpvPlayer
{
    public class LoadingResult
    {
        public bool IsLoading { get; set; } = true;
        public ImpError Error { get; set; }
        public bool Success { get; set; }

        public void MpvOnFileLoaded(object sender, EventArgs e)
        {
            this.IsLoading = false;
            this.Success = true;
        }

        public void MpvOnEndFile(object sender, MpvEndFileEventArgs e)
        {
            this.Success = false;

            if (e.EventEndFile.Reason == MpvEndFileReason.Error)
            {
                this.IsLoading = false;
                this.Error = new ImpError(ErrorType.FailedToOpenFile, e.EventEndFile.Error.ToString());
            }
        }
    }
}
