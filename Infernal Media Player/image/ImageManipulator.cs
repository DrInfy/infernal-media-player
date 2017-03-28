using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Imp.Base.Controllers;
using Imp.Controls;
using SEdge.Core.Maths;

namespace Imp.Player.Image
{
    public class ImageManipulator:ImageController
    {
        private readonly System.Windows.Controls.Image image;
        private ScaleTransform scaleTransform = new ScaleTransform();
        private TranslateTransform translateTransform = new TranslateTransform();
        private TransformGroup transformGroup = new TransformGroup();

        public ImageManipulator(System.Windows.Controls.Image image)
        {
            this.image = image;
            this.transformGroup.Children.Add(this.scaleTransform);
            this.transformGroup.Children.Add(this.translateTransform);
        }

        protected override void ManipulateImage()
        {
            if (this.Zoom == 1 && this.scalingToSpace)
            {
                this.image.RenderTransform = null;
            }
            else
            {
                this.scaleTransform.ScaleX = this.Zoom;
                this.scaleTransform.ScaleY = this.Zoom;
                this.image.RenderTransform = this.transformGroup;
                this.image.RenderTransformOrigin = new Point(0.5, 0.5);

                var mult = MathHelper.Clamp(this.Zoom - 1, 0, this.maxZoom) * 0.5;
                var x = MathHelper.Clamp(this.moveX * this.image.ActualWidth * mult, -this.image.ActualWidth * mult, this.image.ActualWidth * mult);
                var y = MathHelper.Clamp(this.moveY * this.image.ActualHeight * mult, -this.image.ActualHeight * mult, this.image.ActualHeight * mult);
                this.translateTransform.X = x;
                this.translateTransform.Y = y;
            }
        }
    }
}
