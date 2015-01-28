using System;
using System.IO;

namespace Base.FileData.FileReading
{
    internal class Mp3 : AudioFile
    {


        private enum ID3v2_versions : byte
        {
            ID3v2_2 = 2,
            ID3v2_3 = 3,
            ID3v2_4 = 4
        }

        /// <summary>
        /// Contains ID3v2.2 Header Strings
        /// </summary>
        /// <remarks>Enums in VB.NET do not support strings, so we need to use structure</remarks>
        internal struct Frames2
        {
            /// <summary>
            /// 4 Byte long text containing frameinfo converted to string
            /// </summary>
            public string Text;
            /// <summary>
            /// Size of the frame
            /// </summary>
            public uint Size;
            public Frame_flags flags;
            /// <summary>
            /// The 'Title/Songname/Content description' frame is the actual name of
            /// the piece (e.g. "Adagio", "Hurricane Donna").
            /// </summary>
            public static readonly string Title = "TT2";
            /// <summary>
            /// The 'Subtitle/Description refinement' frame is used for information
            /// directly related to the contents title (e.g. "Op. 16" or "Performed
            /// live at Wembley").
            /// </summary>
            public static readonly string SubTitle = "TT3";
            /// <summary>
            /// The 'Lead artist(s)/Lead performer(s)/Soloist(s)/Performing group' is
            /// used for the main artist(s). They are seperated with the "/"
            /// character.
            /// </summary>
            public static readonly string Artist = "TP1";
            /// <summary>
            /// The 'Band/Orchestra/Accompaniment' frame is used for additional
            /// information about the performers in the recording.
            /// </summary>
            public static readonly string Band = "TP2";
            /// <summary>
            /// The 'Conductor' frame is used for the name of the conductor.
            /// </summary>
            public static readonly string Conductor = "TP3";
            /// <summary>
            /// The 'Interpreted, remixed, or otherwise modified by' frame contains
            /// more information about the people behind a remix and similar
            /// interpretations of another existing piece.
            /// </summary>
            /// <remarks></remarks>
            public static readonly string Modified = "TP4";
            /// <summary>
            /// The 'Length' frame contains the length of the audiofile in
            /// milliseconds, represented as a numeric string.
            /// </summary>
            public static readonly string Length = "TLEN";
            /// <summary>
            /// The 'Year' frame is a numeric string with a year of the recording.
            /// This frames is always four characters long (until the year 10000).
            /// </summary>
            public static readonly string Year = "TYE";
            /// <summary>
            /// The 'Track number/Position in set' frame is a numeric string
            /// containing the order number of the audio-file on its original
            /// recording. This may be extended with a "/" character and a numeric
            /// string containing the total numer of tracks/elements on the original
            /// recording. E.g. "4/9".
            /// </summary>
            public static readonly string Track = "TRK";
            /// <summary>
            /// The 'Album/Movie/Show title' frame is intended for the title of the
            /// recording(/source of sound) which the audio in the file is taken
            /// from.
            /// </summary>
            public static readonly string Album = "TAL";
        }

        /// <summary>
        /// Contains some ID3v2.3 and ID3v2.4 Header Strings
        /// </summary>
        /// <remarks>Enums in VB.NET do not support strings, so we need to use structure</remarks>
        internal struct Frames3
        {
            /// <summary>
            /// 4 Byte long text containing frameinfo converted to string
            /// </summary>
            public string Text;
            /// <summary>
            /// Size of the frame
            /// </summary>
            public uint Size;
            public Frame_flags flags;
            /// <summary>
            /// The 'Title/Songname/Content description' frame is the actual name of
            /// the piece (e.g. "Adagio", "Hurricane Donna").
            /// </summary>
            public static readonly string Title = "TIT2";
            /// <summary>
            /// The 'Subtitle/Description refinement' frame is used for information
            /// directly related to the contents title (e.g. "Op. 16" or "Performed
            /// live at Wembley").
            /// </summary>
            public static readonly string SubTitle = "TIT3";
            /// <summary>
            /// The 'Lead artist(s)/Lead performer(s)/Soloist(s)/Performing group' is
            /// used for the main artist(s). They are seperated with the "/"
            /// character.
            /// </summary>
            public static readonly string Artist = "TPE1";
            /// <summary>
            /// The 'Band/Orchestra/Accompaniment' frame is used for additional
            /// information about the performers in the recording.
            /// </summary>
            public static readonly string Band = "TPE2";
            /// <summary>
            /// The 'Conductor' frame is used for the name of the conductor.
            /// </summary>
            public static readonly string Conductor = "TPE3";
            /// <summary>
            /// The 'Interpreted, remixed, or otherwise modified by' frame contains
            /// more information about the people behind a remix and similar
            /// interpretations of another existing piece.
            /// </summary>
            /// <remarks></remarks>
            public static readonly string Modified = "TPE5";
            /// <summary>
            /// The 'Length' frame contains the length of the audiofile in
            /// milliseconds, represented as a numeric string.
            /// </summary>
            public static readonly string Length = "TLEN";
            /// <summary>
            /// The 'Year' frame is a numeric string with a year of the recording.
            /// This frames is always four characters long (until the year 10000).
            /// </summary>
            public static readonly string Year = "TYER";
            /// <summary>
            /// The 'Track number/Position in set' frame is a numeric string
            /// containing the order number of the audio-file on its original
            /// recording. This may be extended with a "/" character and a numeric
            /// string containing the total numer of tracks/elements on the original
            /// recording. E.g. "4/9".
            /// </summary>
            public static readonly string Track = "TRCK";
            /// <summary>
            /// The 'Album/Movie/Show title' frame is intended for the title of the
            /// recording(/source of sound) which the audio in the file is taken
            /// from.
            /// </summary>
            public static readonly string Album = "TALB";
        }

        /// <summary>
        /// These flags appear in ID3v2.3 and ID3v2.4 tags
        /// </summary>
        /// <remarks></remarks>
        internal enum Frame_flags : ushort
        {
            /// <summary>
            /// This flag tells the software what to do with this frame if it is
            /// unknown and the tag is altered in any way.
            /// </summary>
            /// <remarks></remarks>
            Tag_alter_preservation = 32768,
            /// <summary>
            /// This flag tells the software what to do with this frame if it is
            /// unknown and the file, excluding the tag, is altered.
            /// </summary>
            /// <remarks></remarks>
            File_alter_preservation = 16384,
            /// <summary>
            /// This flag, if set, tells the software that the contents of this
            /// frame is intended to be read only.
            /// </summary>
            read_only = 8192,
            /// <summary>
            /// This flag indicates whether or not the frame is compressed.
            /// </summary>
            compression = 128,
            /// <summary>
            /// This flag indicates wether or not the frame is enrypted.
            /// </summary>
            Encyption = 64,
            /// <summary>
            /// This flag indicates whether or not this frame belongs in a group
            /// with other frames. If set a group identifier byte is added to the
            /// frame header. Every frame with the same group identifier belongs
            /// to the same group.
            /// </summary>
            Grouping_identity = 32
        }

        [Flags]
        internal enum ID3v2_3_flags : byte
        {
            Unsynchronisation = 128,
            // 7th bit
            Extended = 64,
            // 6th bit
            Experimental = 32,
            // 5th bit
            None = 0
            // no flags set
        }


        public Mp3(string path)
        {
            FileStream fs = null;
            BinaryReader br = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);
                // if no ID3v2 tag is found then look for ID3v1
                if (!get_ID3v2(ref fs, ref br))
                {
                    get_ID3v1(ref fs, ref br);
                }

            }
            catch (Exception ex)
            {
            }
            finally
            {
                if ((fs != null))
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }


        /// <summary>
        /// Find out if the File has ID3v1 Tag and reads it if there is one
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="br"></param>
        /// <returns>True is tag found</returns>
        /// <remarks></remarks>
        private bool get_ID3v1(ref FileStream fs, ref BinaryReader br)
        {
            if (fs.Length >= 128)
            {
                // ID3v1 Tag is always at the end of the file
                fs.Seek(-128, SeekOrigin.End);

                // Do we have a tag?
                if (Tools.ReadString(br, 3, Tools.Character_set.ISO88591) == "TAG")
                {
                    string Str = null;
                    Str = Tools.ReadString(br, 30, Tools.Character_set.ISO88591);
                    if (!Tools.Validify_ISO_Text(Str))
                        return false;
                    if (string.IsNullOrEmpty(Title))
                        Title = Str;
                    if (!Tools.Validify_ISO_Text(Str))
                        return false;
                    Str = Tools.ReadString(br, 30, Tools.Character_set.ISO88591);
                    if (string.IsNullOrEmpty(Artist))
                        Artist = Str;
                    Str = Tools.ReadString(br, 30, Tools.Character_set.ISO88591);
                    if (string.IsNullOrEmpty(Album))
                        Album = Str;
                    string year = Tools.ReadString(br, 4, Tools.Character_set.ISO88591);

                    byte[] buf = null;
                    buf = new byte[30];
                    br.Read(buf, 0, 30);
                    // This can be converted to comment string if required

                    // ID3v1.1
                    if (buf[28] == 0 & buf[29] != 0)
                    {
                        // comment is only 28 bytes now
                        sTrack = Convert.ToInt32(buf[29]);

                    }
                    else
                    {
                        sTrack = -1;
                        // Track number is not available in ID3v1.0
                    }

                    // Next byte would tell us the genre of the file, not interested in it though.
                }
                return Track != 0;
            }

            return false;
        }

        /// <summary>
        /// Find out if the File has ID3v2 Tag and reads it if there is one
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool get_ID3v2(ref FileStream fs, ref BinaryReader br)
        {
            // We have our tag if there is a ID3 marker
            if (Tools.ReadString(br, 3, Tools.Character_set.ISO88591) == "ID3")
            {
                ID3v2_versions version = default(ID3v2_versions);
                byte[] buf = null;
                buf = new byte[1];
                version = (ID3v2_versions) br.ReadByte();
                //read version number
                buf[0] = br.ReadByte();
                UInt32 tagsize = default(UInt32);

                // The first byte of ID3 version is it's major version, while the second byte
                // is its revision number. All revisions are backwards compatible while
                // major versions are not.
                byte tag;
                Frames3 frame;
                switch (version)
                {
                    case ID3v2_versions.ID3v2_3:
                    case ID3v2_versions.ID3v2_4:


                        tag = br.ReadByte();
                        // 6th bit is set to mark extended header
                        if (((ID3v2_3_flags)tag & ID3v2_3_flags.Extended) == ID3v2_3_flags.Extended)
                        {
                            return false;
                            // Not supported

                        }
                        else
                        {

                            tagsize = size_calculation(br.ReadBytes(4));
                            // read tag size

                            if (tagsize >= fs.Length)
                            {
                                // If tag is bigger than the file, then the tag certaintly is invalid
                                return false;
                            }

                            frame = default(Frames3);
                            // read tags as long as we have more ID3 tag to be read
                            while (fs.Position < tagsize - 10)
                            {
                                frame.Text = Tools.ReadString(br, 4, Tools.Character_set.ISO88591);
                                frame.Size = size_calculation(br.ReadBytes(4));
                                frame.flags = (Frame_flags) br.ReadUInt16();

                                if (frame.Size + fs.Position > tagsize)
                                {
                                    return true;
                                }

                                //If (frame.flags And Frame_flags.compression = Frame_flags.compression) _
                                //    Or (frame.flags And Frame_flags.Encyption = Frame_flags.Encyption) Then
                                //    Debug.Print("Compressed tag")
                                //    ' We have no method for reading ZLib compressed or encrypted text
                                //    'fs.Position += frame.Size
                                //End If
                                string str = "";
                                Tools.Character_set unibyte = (Tools.Character_set) br.ReadByte();

                                if (frame.Text == Frames3.Length | frame.Text == Frames3.Track)
                                {
                                    // we want to read numeric values from the string
                                    unibyte = Tools.Character_set.Numeric8;
                                }

                                str = Tools.ReadString(br, (int) (frame.Size - 1), unibyte);
                                if (frame.Size <= 100)
                                {
                                    str = str.Replace(Tools.ZeroChar, "");
                                    if (frame.Text == Frames3.Title)
                                    {
                                        if (unibyte == 0 && !Tools.Validify_ISO_Text(str))
                                            return false;
                                        Title = str;
                                    }
                                    else if (frame.Text == Frames3.Artist)
                                    {
                                        if (unibyte == 0 && !Tools.Validify_ISO_Text(str))
                                            return false;
                                        Artist = str;
                                    }
                                    else if (frame.Text == Frames3.Album)
                                    {
                                        Album = str;
                                    }
                                    else if (frame.Text == Frames3.Length)
                                    {
                                        TotalSeconds = Convert.ToDouble(long.Parse(str)) / 1000;
                                    }
                                    else if (frame.Text == Frames3.Track)
                                    {
                                        int.TryParse(str, out sTrack);
                                    }
                                    else
                                        return false;
                                }
                                
                                        
                                 

                                    //End If

                            }
                        }
                        break;
                    case ID3v2_versions.ID3v2_2:
                        tag = br.ReadByte();
                        tagsize = size_calculation(br.ReadBytes(4));

                        frame = default(Frames3);
                        // read tags as long as we have more ID3 tag to be read
                        while (fs.Position < tagsize - 10)
                        {
                            frame.Text = Tools.ReadString(br, 3, Tools.Character_set.ISO88591);
                            frame.Size = size_calculation(br.ReadBytes(3));

                            if (frame.Size + fs.Position > tagsize)
                            {
                                return true;
                            }

                            string str = "";
                            Tools.Character_set unibyte = (Tools.Character_set) br.ReadByte();

                            if (frame.Text == Frames3.Length | frame.Text == Frames3.Track)
                            {
                                // we want to read numeric values from the string
                                unibyte = Tools.Character_set.Numeric8;
                            }

                            str = Tools.ReadString(br, (int) (frame.Size - 1), unibyte);

                            if (frame.Text == Frames3.Title)
                            {
                                if (unibyte == 0 && !Tools.Validify_ISO_Text(str))
                                    return false;
                                Title = str;
                            }
                            else if (frame.Text == Frames3.Artist)
                            {
                                if (unibyte == 0 && !Tools.Validify_ISO_Text(str))
                                    return false;
                                Artist = str;
                            }
                            else if (frame.Text == Frames3.Album)
                            {
                                Album = str;
                            }
                            else if (frame.Text == Frames3.Length)
                            {
                                TotalSeconds = Convert.ToDouble(long.Parse(str)) / 1000;
                            }
                            else if (frame.Text == Frames3.Track)
                            {
                                sTrack = int.Parse(str);
                            }
                            else
                                return false;
                            //fs.Position += frame.Size
                        }

                        break;
                    default:
                        // Tag version not supported
                        return false;
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts bytes to UInt32 tag or framesize according to ID3v2 documentation
        /// </summary>
        /// <param name="buf">
        /// </param>
        /// <returns></returns>
        /// <remarks></remarks>
        private UInt32 size_calculation(byte[] buf)
        {
            UInt32 size = default(UInt32);
            // The ID3v2 tag size is encoded with four bytes where the most
            // significant bit (bit 7) is set to zero in every byte, making a total
            // of 28 bits. The zeroed bits are ignored, so a 257 bytes long tag is
            // represented as $00 00 02 01.
            if (buf.GetUpperBound(0) > -1)
            {
                size = buf[0];
                for (int i = 1; i <= buf.GetUpperBound(0); i++)
                {
                    size = size << 7 | buf[i];
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