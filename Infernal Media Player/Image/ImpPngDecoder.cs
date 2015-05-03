#region Usings

using System;
using System.IO;
using System.Security;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hjg.Pngcs;

#endregion

namespace Imp.Image
{
    public class ImpPngDecoder
    {
        #region Fields

        public BitmapSource Source;

        #endregion

        [SecurityCritical]
        public ImpPngDecoder(Stream bitmapStream, BitmapCreateOptions createOptions, BitmapCacheOption cacheOption)
        {
            ReadPng(bitmapStream, createOptions, cacheOption);
        }

        [SecurityCritical]
        public ImpPngDecoder(Uri bitmapUri, BitmapCreateOptions createOptions, BitmapCacheOption cacheOption)
        {
            using (var bitmapStream = new FileStream(bitmapUri.LocalPath, FileMode.Open))
            {
                ReadPng(bitmapStream, createOptions, cacheOption);
            }
        }

        private unsafe void ReadPng(Stream bitmapStream, BitmapCreateOptions createOptions, BitmapCacheOption cacheOption)
        {
            var pngr = new PngReader(bitmapStream);

            if (pngr.ImgInfo.Channels > 4 || pngr.ImgInfo.Channels < 3 || pngr.ImgInfo.BitDepth
                != 8)
            {
                throw new NotSupportedException("PNG must be RGB8/RGBA8");
            }

            var channels = pngr.ImgInfo.Channels;
            var data = new byte[pngr.ImgInfo.Cols * pngr.ImgInfo.Rows * channels];
            var alpha = (channels == 4);

            if (alpha)
            {
                fixed (byte* dataStart = &data[0])
                {
                    var d = dataStart;

                    for (var i = 0; i < pngr.ImgInfo.Rows; i++)
                    {
                        var line = pngr.ReadRowByte(i);
                        for (var j = 0; j < pngr.ImgInfo.Cols; j++)
                        {
                            var j2 = j * channels;
                            *d++ = line.ScanlineB[j2 + 2];
                            *d++ = line.ScanlineB[j2 + 1];
                            *d++ = line.ScanlineB[j2];
                            *d++ = line.ScanlineB[j2 + 3];
                        }
                    }

                    Source = BitmapSource.Create(pngr.ImgInfo.Cols, pngr.ImgInfo.Rows, 100, 100, PixelFormats.Bgra32, null, (IntPtr) dataStart, pngr.ImgInfo.Cols * pngr.ImgInfo.Rows * channels,
                        pngr.ImgInfo.Cols * channels);
                }
                //for (int i = 0; i < pngr.ImgInfo.Rows; i++)
                //{
                //    var i1 = i * pngr.ImgInfo.Cols * channels;
                //    ImageLine line = pngr.ReadRowInt(i);
                //    for (int j = 0; j < pngr.ImgInfo.Cols; j++)
                //    {
                //        var j1 = i1 + j * channels;
                //        var j2 = j * channels;
                //        data[j1+2] = (byte)line.Scanline[j2++];
                //        data[j1 + 1] = (byte)line.Scanline[j2++];
                //        data[j1] = (byte)line.Scanline[j2++];
                //        data[j1 + 3] = (byte)line.Scanline[j2];
                //    }
                //}
                //Source = BitmapSource.Create(pngr.ImgInfo.Cols, pngr.ImgInfo.Rows, 100, 100, PixelFormats.Bgra32, null, data, pngr.ImgInfo.Cols * channels);
                //Source = BitmapSource.Create(pngr.ImgInfo.Cols, pngr.ImgInfo.Rows, 100, 100, PixelFormats.Bgra32, null, data, stride);
            }
            else
            {
                fixed (byte* dataStart = &data[0])
                {
                    var d = dataStart;
                    for (var i = 0; i < pngr.ImgInfo.Rows; i++)
                    {
                        var line = pngr.ReadRowInt(i);
                        for (var j = 0; j < pngr.ImgInfo.Cols; j++)
                        {
                            var j2 = j * channels;
                            *d++ = (byte) line.Scanline[j2 + 2];
                            *d++ = (byte) line.Scanline[j2 + 1];
                            *d++ = (byte) line.Scanline[j2];
                        }
                    }
                    Source = BitmapSource.Create(pngr.ImgInfo.Cols, pngr.ImgInfo.Rows, 100, 100, PixelFormats.Bgr24, null, data, pngr.ImgInfo.Cols * channels);
                }
            }
            pngr.End();

            //double dpi = 96;
            //int width = 128;
            //int height = 128;
            //byte[] pixelData = new byte[width * height];

            //for (int y = 0; y < height; ++y)
            //{
            //    int yIndex = y * width;
            //    for (int x = 0; x < width; ++x)
            //    {
            //        pixelData[x + yIndex] = (byte)(x + y);
            //    }
            //}
            //Source = BitmapSource.Create(width, height, dpi, dpi,
            //        PixelFormats.Gray8, null, pixelData, width);
            //Source = source;
            Source.Freeze();
        }
    }
}