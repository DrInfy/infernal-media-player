﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.ListLogic;

namespace Base.Libraries
{
    public static class StringHandler
    {
        //private static StringBuilder gBuilder = new StringBuilder(50);

        private static readonly char[] startBrackets = new char[] { '(', '[', '{'};
        private static readonly char[] endBrackets = new char[] { ')', ']', '}' };
        private static readonly char[] numeral = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private const int fillNumbersToLength = 3;
        
        /// <summary>
        /// returns time text based on seconds
        /// </summary>
        /// <param name="value">time value as seconds</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string SecondsToTimeText(int value)
        {
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            Math.DivRem(value, 60, out seconds);
            Math.DivRem(Convert.ToInt32(Math.Floor((float)value / 60)), 60, out minutes);
            hours = (int)Math.Floor((float)value / 3600);

            
            return hours.ToString() + ":" + FullFillText(minutes.ToString(), 2, "0") + ":" + FullFillText(seconds.ToString(), 2, "0");
            
        }



        public static bool IsSpecialFolder(string pathData)
        {
            return (System.String.Compare(pathData.Substring(0, 1), "$", System.StringComparison.Ordinal) == 0);
        }
        public static string FullFillText(string text, int length, string fillChar)
        {
            for (int i = text.Length; i <= length - 1; i++)
            {
                text = fillChar + text;
            }
            return text;
        }


        public static string GetFilename(string path)
        {
            string name = null;
            int index = path.LastIndexOf("\\", StringComparison.Ordinal);
            name = path.Substring(index + 1);
            return name;
        }

        //public static string GetExtension(string path)
        //{
        //    string extension = null;
        //    int index = path.LastIndexOf(".", StringComparison.Ordinal);
        //    extension = path.Substring(index + 1);
        //    return extension;
        //}

        public static string RemoveExtension(string path)
        {
            
            int index = path.LastIndexOf(".", StringComparison.Ordinal);
            if (index > 1)
                return path.Substring(0, index);
            return path;
        }

        public static string Get_Streamname(string path)
        {
            string name = null;
            int index = path.LastIndexOf("/");
            name = path.Substring(index + 1);
            return name;
        }

        public static string GetSmartName(string fileName)
        {
            var gBuilder = new StringBuilder();

            gBuilder.Append(RemoveExtension(fileName.ToLowerInvariant()));
            
            gBuilder.Replace(" - ", " ");
            gBuilder.Replace("-", " ");
            gBuilder.Replace(".", " ");
            gBuilder.Replace("'", " ");
            gBuilder.Replace("~", " ");
            gBuilder.Replace("_", " ");
            

            Removebrackets(gBuilder);
            RemoveWords(gBuilder);
            FillNumerals(gBuilder);

            gBuilder.Replace("  ", " ");
            gBuilder.Replace("\\ ", "\\");

            return gBuilder.ToString();
        }


        private static void FillNumerals(StringBuilder builder)
         {
            int numberCount = 0;
            for (int i = 0; i < builder.Length; i++)
            {
                if (IsNumeral(builder[i]))
                    numberCount++;
                else if (numberCount > 0)
                {
                    FillNumeral(builder, i, numberCount);
                    i += fillNumbersToLength - numberCount - 1;
                    numberCount = 0;
                }
            }

            if (numberCount > 0)
                FillNumeral(builder, builder.Length, numberCount);
        }

        private static void FillNumeral(StringBuilder builder, int endIndex, int length)
        {
            if (length < fillNumbersToLength)
                builder.Insert(Math.Max(endIndex - length,0), "0", fillNumbersToLength - length);
            
        }


        private static bool IsNumeral(char number)
        {
            for (int i = 0; i < numeral.Length; i++)
            {
                if (number == numeral[i])
                    return true;
            }
            return false;
        }


        private static void RemoveWords(StringBuilder builder)
        {
            builder.Replace("dvd", "");
            builder.Replace("h264", "");
            builder.Replace(" ep ", " ");
        }


        private static void Removebrackets(StringBuilder builder)
        {
            for (int i = 0; i < builder.Length; i++)
            {
                for (int j = 0; j < startBrackets.Length; j++)
                {
                    if (builder.Length > i && builder[i] == startBrackets[j])
                    {
                        // start of bracket found
                        for (int k = i; k < builder.Length; k++)
                        {
                            if (builder[k] == endBrackets[j])
                            {
                                // end of bracket found
                                RemoveSingleBracket(builder, i, k);

                                j = 0; // reset j, in case we have some double brackets
                                break;
                            }
                        }
                    }
                }
            }
        }


        private static void RemoveSingleBracket(StringBuilder builder, int startIndex, int endIndex)
        {
            
            // check for meaningful episode number, i.e. "song (14).mp3"
            bool isNumber;
            int result = 0;
            if (endIndex - startIndex <= 1)
            {
                isNumber = false;
            }
            else
            {
                var text = builder.ToString(startIndex + 1, endIndex - 1 - (startIndex + 1));
                isNumber = Int32.TryParse(text, out result);
            }
            // if number, then remove just the brackets
            // remove numbers larger than 1900, as they are likely years or some sort of serial number
            if (isNumber && result < 1900)
            {
                builder.Remove(endIndex, 1);
                builder.Remove(startIndex, 1);
            }
            else
            {
                // remove the whole thing i.e. remove: [235AEF5]
                builder.Remove(startIndex, endIndex - startIndex + 1);
            }
        }


        public static bool FindFound(string text, FindString[] findWords)
        {
            // if even a single word not found, item is no longer identified as found
            bool found = true;

            // Filter each find word separately
            foreach (FindString findString in findWords)
            {
                if (findString.Count == 1)
                {
                    if (text.IndexOf(findString.Text, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        found = false;
                        break;
                    }
                }
                else
                {
                    int lastIndex = text.IndexOf(findString.Text,
                        StringComparison.OrdinalIgnoreCase);
                    for (int j = 0; j < findString.Count; j++)
                    {
                        if (lastIndex < 0)
                        {
                            found = false;
                            break;
                        }
                        lastIndex = text.IndexOf(findString.Text, lastIndex + 1,
                            StringComparison.OrdinalIgnoreCase);
                    }

                    if (!found)
                        break;
                }
            }
            return found;
        }


        /// <summary>
        /// Breaks down a find string to find word pieces, with appearance counts
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static FindString[] GetFindWords(string text)
        {
            var findTexts = new List<FindString>();

            if (String.IsNullOrEmpty(text))
            {
                return null;
            }

            int index = 0;

            do
            {
                index = text.LastIndexOf(" ", StringComparison.Ordinal);
                string textPiece = text.Substring(index + 1);


                findTexts.Add(new FindString(textPiece, 1));

                if (index > 0)
                {
                    // remove found text piece and trim the end
                    text = text.Substring(0, index).TrimEnd();
                }
            } while (index > 0);

            // this filters duplicates
            for (int i = 0; i < findTexts.Count; i++)
            {
                // findtext == 0, means that it is identical to something else and has been disabled.
                if (findTexts[i].Count == 0)
                    continue;
                for (int j = 0; j < findTexts.Count; j++)
                {
                    if (i == j)
                        continue;
                    var compare = String.Compare(findTexts[i].Text, findTexts[j].Text, StringComparison.OrdinalIgnoreCase);

                    if (compare == 0)
                    {
                        findTexts[i].Count++;
                        findTexts[j].Count = 0;
                    }
                    else if (findTexts[j].Text.IndexOf(findTexts[i].Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        findTexts[i].Count++;
                    }
                }
            }

            // remove extra find strings
            for (int i = findTexts.Count - 1; i >= 0; i--)
            {
                if (findTexts[i].Count == 0)
                    findTexts.RemoveAt(i);
            }

            return findTexts.ToArray();
        }
    }
}
