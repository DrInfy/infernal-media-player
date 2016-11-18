using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Imp.DirectShow.Subtitles
{
    public class SubtitleProperties
    {
        /// <summary>
        /// Z rotation being the normal 2D rotation
        /// </summary>
        public Vector3D PitchRollYawRotation;

        public Size ExtraScaling;

        public void Apply(TransformGroup transformGroup)
        {
            if (this.ExtraScaling.Width != 100 || this.ExtraScaling.Height != 100)
            {
                var scaleTransform = new ScaleTransform(this.ExtraScaling.Width * 0.01, this.ExtraScaling.Height * 0.01);
                transformGroup.Children.Add(scaleTransform);
            }

            if (this.PitchRollYawRotation.LengthSquared > 0)
            {
                //if (this.PitchRollYawRotation.X != 0)
                //{
                //    var r = new MatrixTransform();
                //    var rotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), this.PitchRollYawRotation.X));
                //    transformGroup.Children.Add(rotation);    
                    
                //}
                //if (this.PitchRollYawRotation.Y != 0)
                //{
                //    var rotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), this.PitchRollYawRotation.Y));
                //    transformGroup.Children.Add(rotation);
                //}

                if (this.PitchRollYawRotation.Z != 0)
                {
                    //var rotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), this.PitchRollYawRotation.Z));
                    var rotation = new RotateTransform() { Angle = this.PitchRollYawRotation.Z };
                    transformGroup.Children.Add(rotation);
                }
            }
        }
    }
}
