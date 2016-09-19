#region Usings

using System;
using System.IO;

#endregion

namespace Imp.Base.FileData.FileReading
{
    /// <summary>
    /// M4A tags, used in mp4 files and mp3 from iTunes
    /// </summary>
    internal class M4A : AudioFile
    {
        #region Static Fields and Constants

        private const uint identifier = 0x20;

        #endregion

        public M4A(string path)
        {
            long totalsize = 0;
            long headersize = 0;

            FileStream fs = null;
            BinaryReader br = null;
            try
            {
                fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                br = new BinaryReader(fs);
                var buf = br.ReadBytes(4);

                // Read Flac marker
                totalsize = fs.Length;
                headersize = Value_Calculation(buf);
                if (Value_Calculation(buf) != identifier | Tools.ReadString(br, 4, Tools.CharacterSet.UTF8) != "ftyp")
                {
                    throw new InvalidDataException("No header found");
                }
                // Skip to header size info now that we know we have the correct format
                fs.Seek(24, SeekOrigin.Current);
                buf = br.ReadBytes(4);
                headersize = Value_Calculation(buf);
                if (headersize > totalsize)
                {
                    throw new InvalidDataException("Invalid header");
                }
                fs.Seek(4, SeekOrigin.Current);
                do
                {
                    var blockSize = 0;
                    string tagType = null;
                    //Dim tagData As String
                    buf = br.ReadBytes(4);
                    blockSize = (int) Value_Calculation(buf);
                    tagType = Tools.ReadString(br, 4, Tools.CharacterSet.ISO88591);
                    switch (tagType)
                    {
                        case "©nam":
                            fs.Seek(8, SeekOrigin.Current);
                            Title = FixBrokenText(Tools.ReadString(br, blockSize - 16, Tools.CharacterSet.UTF8));
                            break;
                        case "©alb":
                            fs.Seek(8, SeekOrigin.Current);
                            Album = FixBrokenText(Tools.ReadString(br, blockSize - 16, Tools.CharacterSet.UTF8));
                            break;
                        case "©art":
                        case "aART":
                        case "©ART":
                            fs.Seek(8, SeekOrigin.Current);
                            Artist = FixBrokenText(Tools.ReadString(br, blockSize - 16, Tools.CharacterSet.UTF8));
                            break;
                        case "trkn":
                            fs.Seek(8, SeekOrigin.Current);
                            var text = FixBrokenText(Tools.ReadString(br, blockSize - 16, Tools.CharacterSet.UTF8));
                            if (text.Length > 0)
                                sTrack = text[0];
                            break;
                        case "udta":
                        case "ilst":
                            break;
                        case "meta":
                            fs.Seek(4, SeekOrigin.Current);
                            break;
                        default:
                            fs.Seek(blockSize - 8, SeekOrigin.Current);
                            break;
                    }
                } while (fs.Position < headersize);
            }
            catch (Exception ex)
            {
                var r = ex.Message;
            }

            fs?.Dispose();
        }

        private static string FixBrokenText(string text)
        {
            return text.Replace(Tools.ZeroChar, "");
        }

        /// <summary>
        ///     Converts bytes to framesize
        /// </summary>
        /// <param name="buf">
        /// </param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static uint Value_Calculation(byte[] buf)
        {
            // Use custom amount of bytes to do a size calculation
            if (buf.GetUpperBound(0) > -1)
            {
                uint size = buf[0];
                for (var i = 1; i <= buf.GetUpperBound(0); i++)
                {
                    size = size << 8 | buf[i];
                }
                return size;
            }
            else
            {
                return 0;
                // Invalid size buffer
            }
        }
    }
}