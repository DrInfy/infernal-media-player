using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using SEdge.Core.Maths;

namespace SEdge.Core.Texts
{
    public static class TextHelper
    {
        public static readonly CultureInfo Ci = CultureInfo.InvariantCulture;

        public static Array GetValues<T>()
        {
            IEnumerable<T> tx = (from x in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public)
                                 select (T)x.GetValue(null));
            //List<T> rx = tx.ToList();
            //List<string> s = new List<string>();
            //foreach (T t in rx)
            //    s.Add(t.ToString());
            return tx.ToArray();
        }

        private static readonly char[] separatorChars = { '\n', '\r' };
        public static string FirstLine([CanBeNull] string text)
        {
            if (text == null) return null;
            var index = text.IndexOfAny(separatorChars);
            return index > 0 ? text.Substring(0, index) : text;
        }

        /// <summary>
        /// Adds the nulls to string so that it has at least 2 numbers.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string AddNulls([NotNull] string text)
        {
            if (text.Length == 1)
                text = "0" + text;
            return text;
        }


       

        /// <summary>
        /// Adds the nulls to string so that it has at least x numbers.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="amount">amount</param>
        /// <returns></returns>
        public static string AddNulls([NotNull] string text, int amount)
        {
            for (int i = text.Length; i < amount; i++)
            {
                text = "0" + text;
            }
            return text;
        }

        /// <summary>
        /// Adds the texts to the end of the string.
        /// </summary>
        public static string AddTexts(string originalText, string text, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                originalText += text;
            }
            return originalText;
        }

        public static string IntToText(int value)
        {
            var text = value.ToString("#,0", CultureInfo.InvariantCulture);
            //if (text == "")
            //    text = "0";
            return text;
        }

        /// <summary>
        /// Dates the time to text as hours and minutes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string DateTimeToTextHours(long value)
        {
            if (value == 0)
                return "-";
            long minutes;
            long hours;
            hours = (long)Math.Floor(value * MathHelper.TicksToSeconds / 3600);
            minutes = (long)(value * MathHelper.TicksToSeconds / 60) - hours * 60;

            return hours.ToString() + ":" + AddNulls(minutes.ToString());
            //return hours.ToString() + " hours " + AddNulls(minutes.ToString()) + " minutes";
        }


        public static string IntToText(long value)
        {
            var text = value.ToString("#,0", CultureInfo.InvariantCulture);
            //if (text == "")
            //    text = "0";
            return text;
        }

        public static float ParseFloat(string number)
        {
            return float.Parse(number, Ci.NumberFormat);
        }

        /// <summary>
        /// Gets the number of lines base on the amount of '\n'.
        /// </summary>
        public static int GetNumberOfLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            int lines = 1;
            int index = text.IndexOf('\n');
            while (index >= 0)
            {
                lines++;
                index = text.IndexOf('\n', index + 1);
            }
            return lines;
        }

        public static string WrapText(string text, int maxLineWidth, int minimumLines)
        {
            //string[] lines = text.Split(@"/n");
            string[] words = text.Split(' ');

            StringBuilder sb = new StringBuilder();

            int totalLenght = 0;
            int totalHeight = 1;
            foreach (string word in words)
            {


                int wLength = word.Length;
                int index = word.LastIndexOf('\n');

                if (index >= 0)
                {
                    var firstIndex = word.IndexOf('\n');
                    int count = (word.Length - word.Replace('\n', ' ').Length) / 2;


                    if (totalLenght + firstIndex > maxLineWidth)
                    {
                        sb.Append("\n" + word.Substring(0, firstIndex) + " ");
                        sb.Append(word.Substring(firstIndex, wLength - firstIndex) + " ");
                        totalHeight++;
                    }
                    else
                    {
                        sb.Append(word + " ");
                    }
                    totalLenght = wLength - index;

                    totalHeight += Math.Max(count, 1);
                }

                else if (totalLenght + wLength < maxLineWidth)
                {
                    sb.Append(word + " ");
                    totalLenght += wLength + 1;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    totalLenght = wLength + 1;
                    totalHeight++;
                }
            }
            for (int i = totalHeight; i < minimumLines; i++)
            {
                sb.Append("\n" + " ");
            }
            return sb.ToString();
        }


        public static void WrapText(StringBuilder sb, int maxLineWidth, int minimumLines)
        {
            int totalHeight = 1;
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == '\n')
                    totalHeight++;
            }

            for (int i = totalHeight; i < minimumLines; i++)
            {
                sb.Append("\n ");
            }
            sb.Append(" ");
        }

        public static string HitPointText(float percentage)
        {
            if (percentage > 0.99f && percentage < 1f)
                return "99%";
            if (percentage <= 0)
                return "0%";
            if (percentage < 0.001f)
                return @"<0.001%";
            if (percentage < 0.01f)
                return String.Format("{0:0.00}", Math.Round(100 * percentage, 2)) + "%";
            if (percentage < 0.095f)
                return String.Format("{0:0.0}", Math.Round(100 * percentage, 1)) + "%";
            return Math.Round(100 * percentage) + "%";
        }


        public static string PercentText(float percentage)
        {
            if (percentage <= 0)
                return "0%";
            if (percentage < 0.095f)
                return String.Format("{0:0}", Math.Round(100 * percentage, 1)) + "%";
            return Math.Round(100 * percentage) + "%";
        }


        public static string FloatToText(float value)
        {
            return string.Format("{0:0.00}", Math.Round(value, 2));
        }

        public static string DateToGeneralText(DateTime date)
        {
            return date.Year + "_" + AddNulls(date.Month.ToString()) + "_" + AddNulls(date.Day.ToString());
        }

        public static string GetBetween(string text, string start, string end)
        {
            var index = text.IndexOf(start, StringComparison.Ordinal);
            var endIndex = text.IndexOf(end, index + start.Length, StringComparison.Ordinal);

            if (index > -1 && endIndex > -1 && endIndex > index)
            {
                return text.Substring(index + start.Length, endIndex - start.Length - index);
            }

            return null;
        }

        public static string GetAfter(string text, string start)
        {
            var index = text.IndexOf(start, StringComparison.Ordinal);

            if (index > -1 && text.Length > start.Length + index)
            {
                return text.Substring(index + start.Length, text.Length - index - start.Length);
            }

            return null;
        }

        public static List<int> ConvertToIntegers(string[] array)
        {
            var list = new List<int>();
            foreach (var text in array)
            {
                int val;
                if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out val))
                {
                    list.Add(val);
                }
                else
                {
                    list.Add(0);
                }
            }

            return list;
        }
    }
}
