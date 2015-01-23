using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Base;

namespace Imp.Image
{
    public class ImpJpgDecoder
    {
        public BitmapSource Source;

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
