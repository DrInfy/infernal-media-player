#region Usings

using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        public void ReadPng(string path)
        {
            var origData = File.ReadAllBytes(path);
            var jpg = new NanoJpeg();
            var result = jpg.njDecode(origData);
            var data = jpg.njGetImage();
            Source = BitmapSource.Create(jpg.njGetWidth(), jpg.njGetHeight(), 100, 100, PixelFormats.Rgb24, null, data, jpg.njGetWidth() * 3);
            Source.Freeze();
        }
    }
}