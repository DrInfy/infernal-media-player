using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Pen = System.Windows.Media.Pen;

namespace Imp.Controls
{
    public class SubtitleChildElement: Control // System.Windows.Controls.Image
    {
        public readonly BlurEffect BlurEffect = new BlurEffect();
        public readonly DropShadowEffect DropShadowEffect = new DropShadowEffect();

        public FormattedText FormattedText { get; set; }
        public Geometry Geometry { get; set; }

        public Pen GeometryPen { get; set; }
        public Point FormattedTextPos { get; set; }

        VisualCollection visuals;
        DrawingVisual visual;

        public SubtitleChildElement()
        {
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Margin = new Thickness(0);

            visuals = new VisualCollection(this);
            visual = new DrawingVisual();
            visuals.Add(visual);
            GeometryPen = new Pen(Brushes.Black, 1);
            //Draw();
        }

        public void ClearContent()
        {
            this.FormattedText = null;
            this.Geometry = null;
        }

        public void Draw()
        {
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                if (FormattedText != null)
                {
                    drawingContext.DrawText(FormattedText, FormattedTextPos);
                }

                if (Geometry != null)
                {
                    drawingContext.DrawGeometry(null, GeometryPen, Geometry);
                }
            }
                //dc.DrawRectangle(Brushes.Red, null, new Rect(0, 0, 100, 100));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (FormattedText != null)
            {
                drawingContext.DrawText(FormattedText, FormattedTextPos);
            }

            if (Geometry != null)
            {
                drawingContext.DrawGeometry(null, GeometryPen, Geometry);
            }
        }
    }
}
