#region Usings

using System.Windows;
using System.Windows.Media;

#endregion

namespace Imp.Controls
{
    public struct ControlRenderData
    {
        #region Fields

        public Brush BackBrush;
        public Brush BorderBrush;
        public Brush FrontBrush;
        public double XTranlate;
        public double YTranlate;

        #endregion

        public void ResetTranslate()
        {
            XTranlate = 0;
            YTranlate = 0;
        }

        public void SetTranslate(Point translation)
        {
            XTranlate = translation.X;
            YTranlate = translation.Y;
        }
    }
}