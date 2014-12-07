using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ImpControls
{
    public struct ControlRenderData
    {
        public Brush BackBrush;
        public Brush BorderBrush;
        public Brush FrontBrush;

        public double XTranlate;
        public double YTranlate;


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
