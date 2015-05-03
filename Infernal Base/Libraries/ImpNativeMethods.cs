using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
// ReSharper disable InconsistentNaming

namespace Base.Libraries
{
    public class ImpNativeMethods
    {

        [DllImport("user32")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        public static class DllImporter
        {
            [DllImport("user32")]
            public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        }

        ///// <summary>
        ///// FxCop requires all Marshalled functions to be in a class called NativeMethods.
        ///// </summary>
        //internal static class NativeMethods
        //{
        //    [DllImport("gdi32.dll")]
        //    [return: MarshalAs(UnmanagedType.Bool)]
        //    internal static extern bool DeleteObject(IntPtr hObject);
        //}


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
                get { return Math.Abs(right - left); } // Abs needed for BIDI OS
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
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
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
            var handle = (new WindowInteropHelper(imp)).Handle;
            var monitor = MonitorFromWindow(handle, MONITOR_DEFAULTTONEAREST);
            if ((monitor != IntPtr.Zero))
            {
                var monitorData = new MONITORINFO();
                if (DllImporter.GetMonitorInfo(monitor, monitorData))
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
            var handle = (new WindowInteropHelper(imp)).Handle;
            var monitor = MonitorFromWindow(handle, MONITOR_DEFAULTTONEAREST);
            if ((monitor != IntPtr.Zero))
            {
                var monitorData = new MONITORINFO();
                if (DllImporter.GetMonitorInfo(monitor, monitorData))
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
            var r = new Rect();
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
    }
}
