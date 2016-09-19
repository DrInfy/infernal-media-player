using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SEdge.Core;

namespace Imp.DirectShow.Helpers
{
    public static class PresentationExtensions
    {
        public static System.Windows.Media.Color ColorConvert(this CustomColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
