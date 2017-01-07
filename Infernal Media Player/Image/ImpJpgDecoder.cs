#region Usings

using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Edge.Tools.Images.Jpeg;

#endregion

namespace Imp.Player.Image
{
    public class ImpJpgDecoder
    {
        #region Fields

        public BitmapSource Source;

        #endregion

        public ImpJpgDecoder(Uri uri)
        {
            ReadPng(uri.LocalPath);
        }

        public bool ReadPng(string path)
        {
            var origData = File.ReadAllBytes(path);
            var jpg = new NanoJpeg();
            var result = jpg.njDecode(origData);

            if (result == NanoJpegResult.NJ_OK)
            {
                var data = jpg.GetImage();
                this.Source = BitmapSource.Create(jpg.GetWidth(), jpg.GetHeight(), 100, 100, PixelFormats.Rgb24, null, data, jpg.GetWidth() * 3);
                this.Source.Freeze();
                return true;
            }

            return false;
        }
    }
}