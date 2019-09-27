using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using WpfAnimatedGif.Decoding;

namespace Imp.Controls.Images
{
    public class ImpImage
    {
        public GifFile GifFile { get; }
        private BitmapSource source;
        private ImageSource imageSource;
        public bool Animated { get; }
        
        public ReadOnlyCollection<BitmapSource> Frames { get; }
        public BitmapSource InitialFrame => this.source ?? this.Frames.FirstOrDefault();


        //public ObjectAnimationUsingKeyFrames Animation { get; }
        public TimeSpan Duration { get; }

        public ImpImage(BitmapSource source)
        {
            this.source = source;
        }

        public ImpImage(BitmapDecoder decoder)
        {
            var list = new List<BitmapSource>();

            foreach (var frame in decoder.Frames)
            {
                list.Add(frame);
            }

            this.Frames = new ReadOnlyCollection<BitmapSource>(list);
            this.Animated = this.Frames.Count > 1;
            if (this.Animated)
            {
                this.Duration = TimeSpan.FromSeconds(this.Frames.Count / 10.0);
            }
        }

        public ImpImage(GifBitmapDecoder decoder, GifFile gifFile)
        {
            this.GifFile = gifFile;
            var animation = ImageBehavior.CreateGifAnimation(decoder, gifFile);
            var list = new List<BitmapSource>();

            foreach (ObjectKeyFrame animationKeyFrame in animation.KeyFrames)
            {
                list.Add((BitmapSource) animationKeyFrame.Value);
            }

            if (animation.Duration.HasTimeSpan)
            {
                this.Duration = animation.Duration.TimeSpan;
            }
            else
            {
                this.Duration = TimeSpan.FromSeconds(this.Frames.Count / 10.0);
            }

            this.Frames = new ReadOnlyCollection<BitmapSource>(list);
            this.Animated = this.Frames.Count > 1;
        }

        public ImpImage(ImageSource source)
        {
            this.imageSource = source;
        }



    }
}
