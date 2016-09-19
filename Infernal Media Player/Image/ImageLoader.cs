#region Usings

using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Imp.Base;
using Imp.Base.FileLoading;

#endregion

namespace Imp.Player.Image
{
    public class ImageLoader : FileLoader<BitmapSource>
    {
        public ImageLoader(Dispatcher dispatcher) : base(dispatcher) {}

        protected override BitmapSource Load(string path, out ImpError error)
        {
            error = null;
            var myUri = new Uri(path, UriKind.RelativeOrAbsolute);
            var type = GetType(path);

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

            if (String.Compare(extension, ".jpg", StringComparison.OrdinalIgnoreCase) == 0 ||
                String.Compare(extension, ".jpeg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Jpg;
            }
            if (String.Compare(extension, ".bmp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Bmp;
            }
            if (String.Compare(extension, ".gif", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Gif;
            }
            if (String.Compare(extension, ".tiff", StringComparison.OrdinalIgnoreCase) == 0 ||
                String.Compare(extension, ".tif", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Tiff;
            }
            if (String.Compare(extension, ".icon", StringComparison.OrdinalIgnoreCase) == 0 ||
                String.Compare(extension, ".ico", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Icon;
            }
            if (String.Compare(extension, ".png", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Png;
            }
            if (String.Compare(extension, ".tga", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.Tga;
            }
            if (String.Compare(extension, ".jxr", StringComparison.OrdinalIgnoreCase) == 0 ||
                String.Compare(extension, ".hdp", StringComparison.OrdinalIgnoreCase) == 0 ||
                String.Compare(extension, ".wdp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ImageType.WindowsMediaPhoto;
            }
            return ImageType.Unknown;
        }
    }
}