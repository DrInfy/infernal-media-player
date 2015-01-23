using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Base;
using Base.Libraries;
using Ipv.Image;

namespace Imp.Image
{
    public class ImageLoader : FileLoader<BitmapSource>
    {
        
        public ImageLoader(Dispatcher dispatcher) : base(dispatcher)
        {
        }


        protected override BitmapSource Load(string path, out ImpError error)
        {
            error = null;
            Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
            ImageType type = GetType(path);

            BitmapDecoder decoder = null;

            try
            {
                switch (type)
                {
                    case ImageType.Jpg:
                        try
                        {
                            decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat,
                                                         BitmapCacheOption.OnLoad);
                        }
                        catch (Exception)
                        {
                            var impJpgDecoder = new ImpJpgDecoder(myUri);
                            return impJpgDecoder.Source;
                        }
                        break;
                    case ImageType.Png:
                        try
                        {
                            decoder = new PngBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat,
                                                             BitmapCacheOption.OnLoad);
                        }
                        catch (Exception)
                        {
                            var impPngDecoder = new ImpPngDecoder(myUri, BitmapCreateOptions.PreservePixelFormat,
                                                         BitmapCacheOption.OnLoad);
                            return impPngDecoder.Source;
                        }
                        break;
                    case ImageType.Gif:
                        decoder = new GifBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat,
                                                         BitmapCacheOption.OnLoad);
                        break;
                    case ImageType.Bmp:
                        decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat,
                                                        BitmapCacheOption.OnLoad);
                        break;
                    case ImageType.Tiff:
                        decoder = new TiffBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat,
                                                         BitmapCacheOption.OnLoad);
                        break;
                    case ImageType.Tga:
                        error = new ImpError(ErrorType.NotSupportedFile);
                        break;
                    case ImageType.Icon:
                        decoder = new IconBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat,
                                                         BitmapCacheOption.OnLoad);
                        break;
                    case ImageType.WindowsMediaPhoto:
                        decoder = new WmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat,
                                                         BitmapCacheOption.OnLoad);
                        break;
                    default:
                        error = new ImpError(ErrorType.UnknownFileType);
                        break;
                }
            }
            catch (Exception)
            {
                error = new ImpError(ErrorType.FailedToOpenFile);
            }
            
            if (error != null)
                return null;

            return decoder.Frames[0];
        }


        protected ImageType GetType(string path)
        {
            var s = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(s)) return ImageType.Unknown;

            var extension = s.ToLowerInvariant();

            if (System.String.Compare(extension, ".jpg", System.StringComparison.OrdinalIgnoreCase) == 0 ||
                System.String.Compare(extension, ".jpeg", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Jpg;
            }
            if (System.String.Compare(extension, ".bmp", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Bmp;
            }
            if (System.String.Compare(extension, ".gif", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Gif;
            }
            if (System.String.Compare(extension, ".tiff", System.StringComparison.OrdinalIgnoreCase) == 0 ||
                System.String.Compare(extension, ".tif", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Tiff;
            }
            if (System.String.Compare(extension, ".icon", System.StringComparison.OrdinalIgnoreCase) == 0 ||
                System.String.Compare(extension, ".ico", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Icon;
            }
            if (System.String.Compare(extension, ".png", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Png;
            }
            if (System.String.Compare(extension, ".tga", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Tga;
            }
            if (System.String.Compare(extension, ".jxr", System.StringComparison.OrdinalIgnoreCase) == 0 ||
                System.String.Compare(extension, ".hdp", System.StringComparison.OrdinalIgnoreCase) == 0 ||
                System.String.Compare(extension, ".wdp", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.WindowsMediaPhoto;
            }
            return ImageType.Unknown;
        }

    }
}
