﻿#region Usings

using System;
using System.Collections;
using System.IO;

#endregion

namespace Imp.Base.FileData.FileReading
{
    /// <summary>
    ///     Windows media and asf files are generallt tagged with this type
    /// </summary>
    internal class Asf : AudioFile
    {
        #region Helpers

        private enum ValueDataTypes : ushort
        {
            /// <summary>
            ///     Unicode string,lenght varies
            /// </summary>
            Unicode = 0,

            /// <summary>
            ///     Array of bytes, lenght varies
            BYTEarray = 1,

            /// <summary>
            ///     Boolean, 32 bits - 0 = false and the rest are true
            /// </summary>
            BOOL = 2,

            /// <summary>
            ///     Int32
            /// </summary>
            DWORD = 3,

            /// <summary>
            ///     Int64
            /// </summary>
            QWORD = 4,

            /// <summary>
            ///     Int16
            /// </summary>
            WORD = 5
        }

        private struct value
        {
            #region Fields

            public Int16 dataType;
            public int index;

            #endregion
        }

        #endregion

        #region Fields

        // ASF GUIDs
        private readonly Guid ASF_Header_Object = new Guid("75B22630-668E-11CF-A6D9-00AA0062CE6C");
        private readonly Guid ASF_Content_Description_Object = new Guid("75B22633-668E-11CF-A6D9-00AA0062CE6C");
        private readonly Guid ASF_Extended_Content_Description_Object = new Guid("D2D0A440-E307-11D2-97F0-00A0C95EA850");
        private readonly Guid ASF_File_Properties_Object = new Guid("8CABDCA1-A947-11CF-8EE4-00C00C205365");
        private readonly Hashtable attrs = new Hashtable();
        private readonly ArrayList attrValues = new ArrayList();

        #endregion

        public Asf(string path)
        {
            var g = default(Guid);
            var CBDone = false;
            var ECBDone = false;
            long sizeBlock = 0;
            string s = null;
            var i = 0;

            FileStream fs = null;
            BinaryReader br = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);

                ReadGuid(ref g, ref br);
                if (g != ASF_Header_Object)
                {
                    // throw an exception
                    return;
                    //Not a valid ASF file
                }
                // Skip
                fs.Position += 12;
                // br.ReadInt64() ' the size of the entire block
                // br.ReadInt32() ' the number of entries
                br.ReadByte();
                // two reserved bytes
                if (br.ReadByte() != 0x2)
                {
                    return;
                    //Not a valid ASF file
                }
                // two reserved bytes
                // preroll value

                // Process all the GUIDs until you get both the contextblock and the extendedcontextblock
                while (ReadGuid(ref g, ref br))
                {
                    sizeBlock = br.ReadInt64();
                    // this is the size of the block

                    // shouldn't happen, but at least fail gracefully
                    if (br.BaseStream.Position + sizeBlock > fs.Length)
                    {
                        break;
                    }
                    if (g == ASF_Content_Description_Object)
                    {
                        processContentBlock(ref br);
                        if (ECBDone)
                        {
                            break;
                        }
                        CBDone = true;
                    }
                    else if (g == ASF_Extended_Content_Description_Object)
                    {
                        processExtendedContentBlock(ref br);
                        if (CBDone)
                        {
                            break;
                        }
                        ECBDone = true;

                        // File properties are here
                    }
                    else if (g == ASF_File_Properties_Object)
                    {
                        long tempPos = 0;

                        sizeBlock -= 24;
                        // already read the guid header info
                        tempPos = br.BaseStream.Position + sizeBlock;

                        ulong temp64 = 0;
                        ulong temp64_2 = 0;

                        // Skip useless information
                        fs.Position += 40;

                        temp64 = br.ReadUInt64();
                        // Specifies the time needed to play the file in 100-nanosecond units. 

                        //Skip again
                        fs.Position += 8;

                        temp64_2 = (ulong) br.ReadInt64();
                        // Offset
                        TotalSeconds = Convert.ToDouble(temp64 - temp64_2) / 10000000;

                        fs.Position = tempPos;
                    }
                    else
                    {
                        // not one we're interested in, skip it
                        sizeBlock -= 24;
                        // already read the guid header info
                        br.BaseStream.Position += sizeBlock;
                    }
                }

                // Get the attributes we're interested in
                Album = getStringAttribute("WM/AlbumTitle");
                var genre = getStringAttribute("WM/Genre");
                s = getStringAttribute("WM/Year");
                int value;
                var result = int.TryParse(s, out value);
                if (result)
                {
                    // not used
                    // int Year = value;
                }
                s = getStringAttribute("WM/TrackNumber");
                // could be n/<total>
                i = s.IndexOf("/");
                if (!(i == -1))
                {
                    s = s.Substring(0, i);
                }

                result = int.TryParse(s, out value);
                if (result)
                {
                    sTrack = Convert.ToInt32(s);
                }
                else
                {
                    s = getStringAttribute("WM/Track");
                    i = s.IndexOf("/");

                    result = int.TryParse(s, out value);
                    if (!(i == -1))
                    {
                        s = s.Substring(0, i);
                    }
                    if (result)
                    {
                        sTrack = value;
                    }
                }
            }
            catch (Exception ex) {}
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        ///     Reads a 128 bit GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool ReadGuid(ref Guid guid, ref BinaryReader br)
        {
            var int1 = 0;
            short shrt1 = 0;
            short shrt2 = 0;
            var b = new byte[7];

            try
            {
                int1 = br.ReadInt32();
                if (int1 == -1)
                {
                    return false;
                }
                shrt1 = br.ReadInt16();
                shrt2 = br.ReadInt16();
                b = br.ReadBytes(8);
                guid = new Guid(int1, shrt1, shrt2, b);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid WMA format.", ex);
            }
        }

        /// <summary>
        ///     Gets information out of the Hashtable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string getStringAttribute(string name)
        {
            var v = default(value);

            if (!attrs.Contains(name))
            {
                return "";
            }
            v = (value) attrs[name];
            if (!(v.dataType == 0))
            {
                // it's not a string type
                return "";
            }
            else
            {
                return Convert.ToString(attrValues[v.index]);
            }
        }

        /// <summary>
        ///     The Content Description Object lets authors record well-known data
        ///     describing the file and its contents. This object is used to store
        ///     standard bibliographic information such as title, author, copyright,
        ///     description, and rating information. This information is pertinent
        ///     to the entire file.
        /// </summary>
        /// <param name="br"></param>
        /// <remarks></remarks>
        private void processContentBlock(ref BinaryReader br)
        {
            ushort lTitle = 0;
            ushort lAuthor = 0;
            ushort lCopyright = 0;
            ushort lDescription = 0;
            ushort lRating = 0;

            lTitle = br.ReadUInt16();
            lAuthor = br.ReadUInt16();
            lCopyright = br.ReadUInt16();
            lDescription = br.ReadUInt16();
            lRating = br.ReadUInt16();
            if (lTitle > 0)
            {
                Title = Tools.ReadString(br, lTitle, Tools.CharacterSet.UTF16);
            }
            if (lAuthor > 0)
            {
                Artist = Tools.ReadString(br, lAuthor, Tools.CharacterSet.UTF16);
            }
            if (lCopyright > 0)
            {
                var copyright = Tools.ReadString(br, lCopyright, Tools.CharacterSet.UTF16);
            }
            if (lDescription > 0)
            {
                var description = Tools.ReadString(br, lDescription, Tools.CharacterSet.UTF16);
            }
            if (lRating > 0)
            {
                var rating = Tools.ReadString(br, lRating, Tools.CharacterSet.UTF16);
            }
        }

        /// <summary>
        ///     The Extended Content Description Object lets authors record data describing
        ///     the file and its contents that is beyond the standard bibliographic
        ///     information such as title, author, copyright, description, or rating information.
        ///     This information is pertinent to the whole file.
        ///     Each Content Descriptor stored in this object uses a name/value pair metaphor.
        /// </summary>
        /// <param name="br"></param>
        /// <remarks></remarks>
        private void processExtendedContentBlock(ref BinaryReader br)
        {
            var numAttrs = default(Int16);
            var dataLen = default(Int16);
            var dataType = default(ValueDataTypes);
            string attrName = null;
            byte[] bValue = null;
            var index = 0;


            object Value = null;

            var valueObject = default(value);

            numAttrs = br.ReadInt16();
            for (var i = 0; i <= numAttrs - 1; i++)
            {
                attrName = readUnicodeString(ref br);
                dataType = (ValueDataTypes) br.ReadUInt16();
                dataLen = (short) br.ReadUInt16();

                switch (dataType)
                {
                    case ValueDataTypes.Unicode:
                        Tools.ReadString(br, dataLen, Tools.CharacterSet.UTF16);

                        break;
                    case ValueDataTypes.BYTEarray:
                        bValue = new byte[dataLen];
                        Value = br.ReadBytes(dataLen);

                        break;
                    case ValueDataTypes.BOOL:
                        Value = br.ReadInt32() != 0;
                        break;
                    case ValueDataTypes.DWORD:
                        Value = br.ReadInt32();

                        break;
                    case ValueDataTypes.QWORD:
                        Value = br.ReadInt64();

                        break;
                    case ValueDataTypes.WORD:
                        Value = br.ReadInt16();

                        break;
                    default:
                        throw new Exception("Bad value for datatype in Extended Content Block. Value = " + dataType);
                }

                valueObject.dataType = (short) dataType;
                valueObject.index = index;
                attrs.Add(attrName, valueObject);
                attrValues.Add(Value);
                index += 1;
            }
        }

        private string readUnicodeString(ref BinaryReader br)
        {
            var len = br.ReadUInt16();

            return Tools.ReadString(br, len, Tools.CharacterSet.UTF16);
        }
    }
}