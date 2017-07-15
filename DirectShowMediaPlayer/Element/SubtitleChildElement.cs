using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Imp.DirectShow.Element
{
    public class SubtitleChildElement// : Control // System.Windows.Controls.Image
    {
        #region  Public Fields and Properties

        public readonly BlurEffect BlurEffect = new BlurEffect();
        public readonly DropShadowEffect DropShadowEffect = new DropShadowEffect();

        public FormattedText FormattedText { get; set; }
        public Geometry Geometry { get; set; }

        public Pen GeometryPen { get; set; }
        public Point FormattedTextPos { get; set; }
        public Effect Effect { get; set; }
        public Point Translate { get; set; }
        public double Opacity { get; set; }
        public Geometry Clip { get; set; }
        public Transform RenderTransform { get; set; }
        public string Name { get; set; }

        public Visibility Visibility { get; set; } = Visibility.Hidden;

        #endregion

        #region Common

        public SubtitleChildElement()
        {
            //VerticalAlignment = VerticalAlignment.Stretch;
            //HorizontalAlignment = HorizontalAlignment.Stretch;
            //Margin = new Thickness(0);

            this.GeometryPen = new Pen(Brushes.Black, 1);
            //this.SnapsToDevicePixels = false;
            //this.VisualTextRenderingMode = TextRenderingMode.Aliased;
            //Draw();
        }

        public void ClearContent()
        {
            this.FormattedText = null;
            this.Geometry = null;

            this.Visibility = Visibility.Hidden;
            this.Clip = null;
            this.FormattedText = null;
            this.Geometry = null;
            this.Effect = null;
            this.Opacity = 1;
            this.DropShadowEffect.Opacity = 1;
            this.RenderTransform = null;
        }

        //public void TranslatePoint(Point translatePoint, SubtitleChildElement control)
        //{
        //}

        public void Render(DrawingContext drawingContext)
        {
            
            if (this.FormattedText != null)
            {
                drawingContext.DrawText(this.FormattedText, this.FormattedTextPos);
            }

            if (this.Geometry != null)
            {
                this.GeometryPen.DashCap = PenLineCap.Round;
                this.GeometryPen.EndLineCap = PenLineCap.Round;
                this.GeometryPen.LineJoin = PenLineJoin.Round;
                this.GeometryPen.StartLineCap = PenLineCap.Round;
                drawingContext.DrawGeometry(null, this.GeometryPen, this.Geometry);
            }
        }

        #endregion

        //}

        //    base.OnRender(drawingContext);

        //    }
        //        drawingContext.DrawGeometry(null, GeometryPen, Geometry.GetAsFrozen() as Geometry);
        //        GeometryPen.StartLineCap = PenLineCap.Round;
        //        GeometryPen.LineJoin = PenLineJoin.Round;
        //        GeometryPen.EndLineCap = PenLineCap.Round;
        //        GeometryPen.DashCap = PenLineCap.Round;
        //    {

        //    if (Geometry != null)
        //    }
        //        drawingContext.DrawText(FormattedText, FormattedTextPos);
        //    {
        //    if (FormattedText != null)
        //    this.Effect = null;
        //{

        //protected override void OnRender(DrawingContext drawingContext)
    }
}