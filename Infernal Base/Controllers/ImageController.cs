﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEdge.Core.Maths;

namespace Imp.Base.Controllers
{
    public abstract class ImageController
    {
        protected bool scalingToSpace = true;
        protected double zoom = 1;
        protected double minZoom = 0.1;
        protected double maxZoom = 10;
        protected double moveX = 0;
        protected double moveY = 0;

        public void SetZoom(double value)
        {
            zoom = value;
            this.zoom = MathHelper.Clamp(this.zoom, this.minZoom, this.maxZoom);
            ManipulateImage();
        }

        public void ChangeZoom(double value)
        {
            zoom += value;
            this.zoom = MathHelper.Clamp(this.zoom, this.minZoom, this.maxZoom);
            ManipulateImage();
        }

        public void SetTranslation(double x, double y)
        {
            this.moveX = MathHelper.Clamp(x, -1, 1);
            this.moveY = MathHelper.Clamp(y, -1, 1);
            
            ManipulateImage();
        }

        public void MoveTranslation(double x, double y)
        {
            this.moveX = MathHelper.Clamp(this.moveX + x, -1, 1);
            this.moveY = MathHelper.Clamp(this.moveY + y, -1, 1);

            ManipulateImage();
        }

        public void ImageChanged()
        {
            this.moveY = 1;
            this.moveX = 0;
        }

        public void ScreenSizeChanged()
        {
            ManipulateImage();
        }

        protected abstract void ManipulateImage();
    }
}
