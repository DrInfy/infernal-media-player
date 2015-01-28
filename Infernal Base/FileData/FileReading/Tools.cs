using System;
using System.IO;
using System.Text;

namespace Base.FileData.FileReading
{
    static internal class Tools
    {
        public static readonly string ZeroChar = '\0'.ToString();
        public static readonly string OneChar = '\u0001'.ToString();


        internal enum CharacterSet : byte
        {
            /// <summary>
            /// ISO-8859-1 [ISO-8859-1]. Terminated with $00.
            /// </summary>
            ISO88591 = 0,
            /// <summary>
            /// UTF-16 [UTF-16] encoded Unicode [UNICODE] with BOM. All
            /// strings in the same frame SHALL have the same byteorder.
            /// Terminated with $00 00.
            /// </summary>
            UTF16 = 1,
            /// <summary>
            /// UTF-16BE [UTF-16] encoded Unicode [UNICODE] without BOM.
            /// Terminated with $00 00.
            /// </summary>
            UTF16BE = 2,
            /// <summary>
            /// UTF-8 [UTF-8] encoded Unicode [UNICODE]. Terminated with $00.
            /// </summary>
            UTF8 = 3,
            /// <summary>
            /// Special case that can be used for reading numeric strings
            /// </summary>
            Numeric8 = 255
        }

        static readonly Encoding ISO88591 = Encoding.GetEncoding(28591);
        static readonly Encoding UTF16 = Encoding.Unicode;
        static readonly Encoding UTF8 = Encoding.UTF8;

        static readonly Encoding UTF16BE = Encoding.BigEndianUnicode;
        /// <summary>
        /// compares two array of bytes
        /// </summary>
        /// <returns>True if equal</returns>
        /// <remarks></remarks>
        public static object ByteCompare(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// compares two array of bytes with maximumlenght
        /// </summary>
        /// <returns>True if equal</returns>
        /// <remarks></remarks>
        public static object ByteCompare(byte[] a, byte[] b, int maxLength)
        {
            if (a == null & b == null)
            {
                return true;
            }
            else if (a == null | b == null)
            {
                return false;
            }

            int Length = Math.Min(Math.Min(a.Length, b.Length), maxLength) - 1;

            for (int i = 0; i <= Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reads bytes with a BinaryReader and converts then to a string
        /// </summary>
        static internal string ReadString(BinaryReader br, int length, CharacterSet CS)
        {
            if (length <= 0)
                return "";
            byte[] byteArray = null;
            string str = "";

            byteArray = new byte[length];
            br.Read(byteArray, 0, length);
            switch (CS)
            {
                case CharacterSet.ISO88591:
                    str = ISO88591.GetString(byteArray);

                    break;
                case CharacterSet.UTF16:
                    // FF FE defines little ending UTF16
                    if (byteArray.GetUpperBound(0) > 1 && byteArray[0] == 0xff & byteArray[1] == 0xfe)
                    {
                        byteArray[0] = 0;
                        byteArray[1] = 0;
                        str = UTF16.GetString(byteArray);
                        // FE FF defines big ending UTF16
                    }
                    else if (byteArray.GetUpperBound(0) > 1 && byteArray[0] == 0xfe & byteArray[1] == 0xff)
                    {
                        byteArray[0] = 0;
                        byteArray[1] = 0;
                        str = UTF16BE.GetString(byteArray);
                    }
                    else
                    {
                        str = UTF16.GetString(byteArray);
                    }
                    break;
                case CharacterSet.UTF16BE:
                    str = UTF16BE.GetString(byteArray);

                    break;
                case CharacterSet.UTF8:
                    str = UTF8.GetString(byteArray);

                    break;
                case CharacterSet.Numeric8:

                    for (int i = 0; i <= byteArray.GetUpperBound(0); i++)
                    {
                        if (byteArray[i] < 58 & byteArray[i] > 47)
                        {
                            str += (byteArray[i] - 48).ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(str))
                    {
                        str = "0";
                    }
                    break;
                default:
                    return "";
            }
            return str.Trim('\0').Trim();
        }

        public static bool Validify_ISO_Text(string text)
        {
            foreach (char c in text)
            {
                if (c >= 0 && c <= 0x1f )
                    return false;
                if (c >= 0x7F && c <= 0xBF )
                        return false;
            }
            return true;
        }
    }
}