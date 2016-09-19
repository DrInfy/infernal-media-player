using System.Windows.Media;

//using System.Windows.Media;

namespace Imp.Player.Libraries
{
    public static class ConverterUtility
    {
        public static System.Windows.Media.Color ColorConvert(this System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
