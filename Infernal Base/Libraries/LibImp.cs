using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Base.FileData;

namespace Base.Libraries
{
    public static class LibImp
    {
        public const double MilliToSecond = 1d / 1000d;

        public const long NANOTOMILLI = 10000;
        public const long SecondToTicks = 10000000;
        public const double TicksToSecond = 1d / SecondToTicks;
        public const int PANELHIGHHEIGHT = 25;

        public const int PANELLOWHEIGHT = 25;


        public static OperatingSystem OsInfo = Environment.OSVersion;

        public static Random Rnd = new Random();


#region Complicated Monitor screen size stuff

        [DllImport("user32")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        public static class dllImporter
        {
            [DllImport("user32")]
            public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        }

        /// <summary>
        /// FxCop requires all Marshalled functions to be in a class called NativeMethods.
        /// </summary>
        internal static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }



        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;
            /// <summary> Win32 </summary>
            public int top;
            /// <summary> Win32 </summary>
            public int right;
            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty = new RECT();

            /// <summary> Win32 </summary>
            public int Width
            {
                get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
            }
            /// <summary> Win32 </summary>
            public int Height
            {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is RECT)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }

        public const int MONITOR_DEFAULTTONEAREST = 0x2;

        public static Rect GetWorkArea(Window imp)
        {
            IntPtr handle = (new WindowInteropHelper(imp)).Handle;
            IntPtr monitor = MonitorFromWindow(handle, MONITOR_DEFAULTTONEAREST);
            if ((monitor != IntPtr.Zero))
            {
                var monitorData = new MONITORINFO();
                if (dllImporter.GetMonitorInfo(monitor, monitorData))
                {
                    Marshal.Release(handle);
                    handle = IntPtr.Zero;
                }
                var area = new Rect(monitorData.rcWork.left, monitorData.rcWork.top,
                                     monitorData.rcWork.right - monitorData.rcWork.left,
                                     monitorData.rcWork.bottom - monitorData.rcWork.top);

                return area;
            }
            else
            {
                return new Rect();
            }
        }

        public static Rect GetMonitorArea(Window imp)
        {
            IntPtr handle = (new WindowInteropHelper(imp)).Handle;
            IntPtr monitor = MonitorFromWindow(handle, MONITOR_DEFAULTTONEAREST);
            if ((monitor != IntPtr.Zero))
            {
                var monitorData = new MONITORINFO();
                if (dllImporter.GetMonitorInfo(monitor, monitorData))
                {
                    Marshal.Release(handle);
                    handle = IntPtr.Zero;
                }

                var area = new Rect(monitorData.rcMonitor.left, monitorData.rcMonitor.top,
                                     monitorData.rcMonitor.right - monitorData.rcMonitor.left,
                                     monitorData.rcMonitor.bottom - monitorData.rcMonitor.top);

                return area;
            }
            else
            {
                return new Rect();
            }
        }


        public static Rect GetWindowRect(Window o)
        {
            Rect r = new Rect();
            r.X = o.Left;
            r.Y = o.Top;
            r.Height = o.Height;
            r.Width = o.Width;
            return r;
        }

        public static void SetWindowRect(Window o, Rect r)
        {
            o.Left = r.Left;
            o.Top = r.Top;
            o.Height = r.Height;
            o.Width = r.Width;
        }
#endregion


        public static double TicksToSeconds(long ticks)
        {
            return ticks * TicksToSecond;
        }

        public static long SecondsToTicks(double seconds)
        {
            return (long) (seconds * SecondToTicks);
        }

        public static FileImpInfo[] FilterFiles(FileInfo[] infos, List<string> extensions)
        {
            FileImpInfo[] files;
            if (extensions == null || extensions.Count < 1 || String.IsNullOrEmpty(extensions[0]))
            {
                files = new FileImpInfo[infos.Length];
                for (int j = 0; j < infos.Length; j++)
                {
                    files[j] = new FileImpInfo(infos[j]);

                }
                return files;
            }

            int foundItems = 0;
            foreach (FileInfo p in infos)
            {
                var e = Path.GetExtension(p.Name);
                if (!string.IsNullOrWhiteSpace(e))
                    if (extensions.Any(f => String.Compare(e, f, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        infos[foundItems] = p;
                        foundItems += 1;
                    }
            }
            // Array.Resize(ref paths, i + 1);

            files = new FileImpInfo[foundItems];
            for (int j = 0; j < files.Length; j++)
            {
                files[j] = new FileImpInfo(infos[j]);
            }

            return files;
        }


        /// <summary>
        /// Does events, and allows updating of the form.
        /// Use Dispatcher.CurrentDispatcher for the current thread.
        /// </summary>
        public static void DoEvents(Dispatcher dispatch)
        {
            dispatch.Invoke(new Action(DoEventsHandler), DispatcherPriority.Background);
        }

        private static void DoEventsHandler()
        {
        }


        public static double KeepInsideBounds(double value, double min, double max)
        {
            return Math.Max(Math.Min(value, max), min);
        }
    }
}