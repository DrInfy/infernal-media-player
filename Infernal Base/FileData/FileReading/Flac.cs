﻿#region Usings

using System;
using System.IO;

#endregion

namespace Imp.Base.FileData.FileReading
{
    /// <summary>
    ///     Flac tag, generally flac and ogg vorbis files use these
    /// </summary>
    internal class FlacOgg : AudioFile
    {
        #region Static Fields and Constants

        private const string FLAC_MARKER = "fLaC";
        private const string OGG_MARKER = "OggS";

        #endregion

        public FlacOgg(string path)
        {
            byte startHeader = 0;
            long totalsize = 0;
            long songStart = 0;

            FileStream fs = null;
            BinaryReader br = null;

            string startstring = null;
            try
            {
                fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                br = new BinaryReader(fs);

                // Read Flac marker
                totalsize = fs.Length;

                //Dim flacmarker(3) As Byte
                //fs.Read(flacmarker, 0, 4)
                startstring = Tools.ReadString(br, 4, Tools.CharacterSet.ISO88591);
                if (startstring != FLAC_MARKER & startstring != OGG_MARKER)
                {
                    // (Tools.ByteCompare(flacmarker, FLAC_MARKER)) = False Then
                    throw new InvalidDataException("No header found");
                }


                // File is Flac
                if (startstring == FLAC_MARKER)
                {
                    // Skip straight to the METADATA_BLOCK_STREAMINFO and sample rate
                    fs.Seek(14, SeekOrigin.Current);

                    var buf = br.ReadBytes(3);


                    sFrequency = buf[0];
                    sFrequency = (sFrequency << 8) | buf[1];
                    sFrequency = (sFrequency << 4) | (buf[2] >> 4);

                    // 3 bits tell se amount of channels
                    sChannels = ((buf[2] >> 1) & 0x3) + 1;

                    // next 5 bits would tell us: 
                    // (bits per sample)-1. FLAC supports from 4 to 32 bits per sample. 
                    // Currently the reference encoder and decoders only support up to 24 bits per sample.
                    // We are discarding this information for now

                    buf = br.ReadBytes(5);
                    // Basically use 4+8+8+8+8 = 36 bits to find out amount of samples
                    const byte byte15 = 15;
                    buf[0] = (byte) (buf[0] & byte15);
                    sSamples = size_calculation(buf);
                    // Total samples in stream. 'Samples' means inter-channel sample, 
                    // i.e. one second of 44.1Khz audio will have 44100 samples regardless
                    // of the number of channels. A value of zero here means the number of
                    // total samples is unknown.

                    TotalSeconds = Convert.ToDouble(sSamples) / sFrequency;

                    //########## TAG #################

                    // Skip MD5 signature
                    fs.Position = 42;

                    do
                    {
                        startHeader = br.ReadByte();
                        var lastBlock = (startHeader >> 7) == 1;

                        var blockType = startHeader & 127;
                        // end of these blocks with no usable information
                        if (blockType == 4)
                            break; // TODO: might not be correct. Was : Exit Do

                        if (lastBlock)
                        {
                            throw new InvalidDataException();
                        }

                        fs.Position += size_calculation(br.ReadBytes(3)) + 3;
                        // fs.Position doesn't update fast enough , thus +3
                    } while (true);

                    br.ReadBytes(3);

                    // Read Vorbis comment block
                    dynamic vendorLength = br.ReadInt32();
                    buf = br.ReadBytes(vendorLength);
                    // We are not interested in vendor, so discard that information

                    // File is of ogg type instead
                }
                else if (startstring == OGG_MARKER)
                {
                    var i = 0;
                    do
                    {
                        string merkki = null;
                        merkki = Tools.ReadString(br, 1, Tools.CharacterSet.UTF8);
                        if (merkki == "v")
                        {
                            merkki += Tools.ReadString(br, 5, Tools.CharacterSet.UTF8);
                            if (merkki == "vorbis")
                            {
                                i += 1;
                                if (i > 1)
                                {
                                    fs.Position += br.ReadUInt32() + 4;
                                    break; // TODO: might not be correct. Was : Exit Do
                                }
                            }
                        }
                        if (fs.Position > 300)
                            return;
                    } while (true);
                }

                var commentNumber = br.ReadUInt32();
                // Read for Vorbis comments
                for (var i = 0; i <= commentNumber - 1; i++)
                {
                    var commentLength = br.ReadUInt32();
                    var comment2 = Tools.ReadString(br, (int) commentLength, Tools.CharacterSet.UTF8);
                    // Vorbis comments are all in a format like "TITLE=Best song ever"
                    var sepindex = comment2.IndexOf("=", StringComparison.Ordinal);
                    var name = comment2.Substring(0, sepindex);
                    var value = comment2.Substring(sepindex + 1);
                    AddTag(name, value);
                }

                songStart = fs.Position;
            }
            catch //(Exception ex)
            {
                //Throw New InvalidDataException(String.Format("Cannot read .Flac file '{0}'", path), ex)
            }
            finally
            {
                fs?.Close();
            }

            //########## Bit rate ############

            totalsize -= songStart;
            if (TotalSeconds <= 0)
            {
                Bitrate = 0;
            }
            else
            {
                Bitrate = totalsize / (TotalSeconds * 125);
            }
        }

        private void AddTag(string name, string value)
        {
            var key = name.Trim(' ', '\0').ToUpperInvariant();
            var val = value.Trim(' ', '\0');

            switch (key)
            {
                case "TITLE":
                    Title = val;
                    break;
                case "ARTIST":
                    Artist = val;
                    break;
                case "ALBUM":
                    Album = val;
                    break;
                case "TRACKNUMBER":
                    sTrack = int.Parse(val);
                    break;
            }
        }

        /// <summary>
        ///     Converts bytes to framesize
        /// </summary>
        private static uint size_calculation(byte[] buf)
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