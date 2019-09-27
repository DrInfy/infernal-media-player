#region Usings

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Imp.Controls.Images;
using WpfAnimatedGif;

#endregion

namespace Imp.Controls
{
    public class ImpImageViewer : Image
    {
        #region Static Fields and Constants

        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof(int), typeof(ImpImageViewer), new UIPropertyMetadata(0, new PropertyChangedCallback(ChangingFrameIndex)));

        public static readonly DependencyProperty AutoStartProperty =
            DependencyProperty.Register("AutoStart", typeof(bool), typeof(ImpImageViewer), new UIPropertyMetadata(false, AutoStartPropertyChanged));

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(ImpImageViewer), new UIPropertyMetadata(string.Empty, ImageSourcePropertyChanged));

        #endregion

        #region  Public Fields and Properties

        public int FrameIndex
        {
            get { return (int) GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        /// <summary>
        ///     Defines whether the animation starts on it's own
        /// </summary>
        public bool AutoStart
        {
            get { return (bool) GetValue(AutoStartProperty); }
            set { SetValue(AutoStartProperty, value); }
        }

        public ImpImage ImageSource
        {
            get => this.imageSource;
            set
            {
                var old = this.imageSource;
                this.imageSource = value;
                ImageSourcePropertyChanged(this, new DependencyPropertyChangedEventArgs(ImageSourceProperty, old, value));
            }
        }

        #endregion

        #region Local Fields

        private bool _isInitialized;
        private Int32Animation _animation;
        private ImpImage imageSource;

        #endregion

        #region Common

        static ImpImageViewer()
        {
            VisibilityProperty.OverrideMetadata(typeof(ImpImageViewer),
                new FrameworkPropertyMetadata(VisibilityPropertyChanged));
        }

        public ImpImageViewer()
        {
            this.AutoStart = true;
        }

        private void InitializeGifAnimation()
        {
            this.StopAnimation();
            var source = this.ImageSource;

            if (this.ImageSource == null)
            {
                this._isInitialized = true;
                this.Source = null;
                return;
            }

            this.Source = source.InitialFrame;

            if (source.Animated)
            {
                this._animation = new Int32Animation(0, source.Frames.Count - 1, new Duration(source.Duration));
                this._animation.RepeatBehavior = RepeatBehavior.Forever;

                if (this.AutoStart)
                {
                    this.StartAnimation();
                }

            }
            else
            {
                this._animation = null;
            }


            this._isInitialized = true;
        }

        private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Visibility) e.NewValue == Visibility.Visible)
            {
                ((ImpImageViewer) sender).StartAnimation();
            }
            else
            {
                ((ImpImageViewer) sender).StopAnimation();
            }
        }

        static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            var ImpImageViewer = (ImpImageViewer) obj;
            if (ImpImageViewer.ImageSource.Animated)
            {
                ImpImageViewer.Source = ImpImageViewer.ImageSource.Frames[(int) ev.NewValue];
            }
        }

        private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue)
                (sender as ImpImageViewer).StartAnimation();
        }

        private static void ImageSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ImpImageViewer).InitializeGifAnimation();
        }

        /// <summary>
        ///     Starts the animation
        /// </summary>
        public void StartAnimation()
        {
            if (!this._isInitialized)
                InitializeGifAnimation();

            BeginAnimation(FrameIndexProperty, this._animation);
        }

        /// <summary>
        ///     Stops the animation
        /// </summary>
        public void StopAnimation()
        {
            BeginAnimation(FrameIndexProperty, null);
        }

        protected override void OnRender(DrawingContext dc)
        {
            
            if (this._animation != null)
            {
                var temp = this.Source;
                for (int i = 0; i < this.FrameIndex + 1; i++)
                {
                    this.Source = this.ImageSource.Frames[i];
                    base.OnRender(dc);
                }
            }
            else
            {
                base.OnRender(dc);
            }
        }

        #endregion
    }
}