#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using Base.FileData;

#endregion

namespace Base.Libraries
{
    public static class LibImp
    {
        #region Static Fields and Constants

        public const double MilliToSecond = 1d / 1000d;
        public const long NanoToMilli = 10000;
        public const long SecondToTicks = 10000000;
        public const double TicksToSecond = 1d / SecondToTicks;
        public const int PanelHighHeight = 25;
        public const int PanelLowHeight = 25;
        public static OperatingSystem OsInfo = Environment.OSVersion;
        public static Random Rnd = new Random();

        #endregion

        public static double TicksToSeconds(long ticks)
        {
            return ticks * TicksToSecond;
        }

        public static long SecondsToTicks(double seconds)
        {
            return (long) (seconds * SecondToTicks);
        }

        public static string GetBetween(string text, string start, string end)
        {
            var index = text.IndexOf(start, StringComparison.Ordinal);
            var endIndex = text.IndexOf(end, StringComparison.Ordinal);

            if (index > -1 && endIndex > -1 && endIndex > index)
            {
                return text.Substring(index + start.Length, endIndex - start.Length - 1);
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

        /// <summary>
        /// Filter files by their extensions and convert them to imp fileinfos
        /// </summary>
        /// <param name="infos">Files to be filtered</param>
        /// <param name="extensions">extensions to filter by. Null or empty list gives all</param>
        /// <returns> Filtered files </returns>
        public static FileImpInfo[] FilterFiles(FileInfo[] infos, List<string> extensions)
        {
            if (extensions == null || extensions.Count < 1)
            {
                var files = new FileImpInfo[infos.Length];
                for (var j = 0; j < infos.Length; j++)
                {
                    files[j] = new FileImpInfo(infos[j]);
                }
                return files;
            }

            var fileList = new List<FileImpInfo>();

            foreach (var fileInfo in infos)
            {
                var extension = Path.GetExtension(fileInfo.Name);
                if (extensions.All(x => String.Compare(extension, x, StringComparison.OrdinalIgnoreCase) != 0)) continue;
                fileList.Add(new FileImpInfo(fileInfo));
            }

            return fileList.ToArray();
        }

        /// <summary>
        /// Does events, and allows updating of the form.
        /// Use Dispatcher.CurrentDispatcher for the current thread.
        /// </summary>
        public static void DoEvents(Dispatcher dispatch)
        {
            dispatch.Invoke(DoEventsHandler, DispatcherPriority.Background);
        }

        private static void DoEventsHandler() {}

        public static double KeepInsideBounds(double value, double min, double max)
        {
            return Math.Max(Math.Min(value, max), min);
        }
    }
}