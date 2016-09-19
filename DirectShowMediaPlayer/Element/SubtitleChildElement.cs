using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Pen = System.Windows.Media.Pen;

namespace Imp.DirectShow.Element
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
            this.SnapsToDevicePixels = false;
            this.VisualTextRenderingMode = TextRenderingMode.Aliased;
            //Draw();
        }

        public void ClearContent()
        {
            this.FormattedText = null;
            this.Geometry = null;

            Visibility = Visibility.Hidden;
            Clip = null;
            FormattedText = null;
            Geometry = null;
            Effect = null;
            Opacity = 1;
            DropShadowEffect.Opacity = 1;
            RenderTransform = null;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            GeometryPen.LineJoin = PenLineJoin.Miter;
            GeometryPen.DashCap = PenLineCap.Round;
            GeometryPen.EndLineCap = PenLineCap.Round;
            GeometryPen.LineJoin = PenLineJoin.Round;
            GeometryPen.StartLineCap = PenLineCap.Round;
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
