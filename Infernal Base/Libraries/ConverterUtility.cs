using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

//using System.Windows.Media;

namespace Imp.Libraries
{
    public static class ConverterUtility
    {
        public static System.Windows.Media.Color ColorConvert(this System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
