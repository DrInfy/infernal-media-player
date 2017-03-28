using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Imp.Controls
{
    public static class Extensions
    {
        public static Rect BoundsRelativeTo(this FrameworkElement element,
                                         Visual relativeTo)
        {
            return
              element.TransformToVisual(relativeTo)
                     .TransformBounds(LayoutInformation.GetLayoutSlot(element));
        }


        public static Geometry GeometryRelativeTo(this FrameworkElement element,
                                         Visual relativeTo)
        {
            var rect = element.TransformToVisual(relativeTo)
                     .TransformBounds(LayoutInformation.GetLayoutSlot(element));

            var rectangleGeometry = new RectangleGeometry();
            rectangleGeometry.Rect = rect;

            return rectangleGeometry;
        }
    }
}
