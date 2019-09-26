using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using SEdge;


namespace Imp.MpvPlayer
{
    internal class MpvPlayerHwndHost : HwndHost
    {
        private Mpv.NET.API.Mpv mpv;

        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int HOST_ID = 0x00000002;
        private const int WS_CLIPCHILDREN = 0x02000000;
        

        public MpvPlayerHwndHost([NotNull]Mpv.NET.API.Mpv mpv)
        {
            this.mpv = mpv;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            // Create the child window that will host the
            // mpv player.
            var playerHostPtr = WinFunctions.CreateWindowEx(0,
                "static",
                "",
                WS_CHILD | WS_VISIBLE | WS_CLIPCHILDREN,
                0,
                0,
                100,
                100,
                hwndParent.Handle,
                (IntPtr)HOST_ID,
                IntPtr.Zero,
                0);

            // Set the mpv parent.
            var playerHostPtrLong = playerHostPtr.ToInt64();
            mpv.SetPropertyLong("wid", playerHostPtrLong);

            return new HandleRef(this, playerHostPtr);
        }

        protected override void OnWindowPositionChanged(Rect rcBoundingBox)
        {
            base.OnWindowPositionChanged(rcBoundingBox);
        }

        

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            WinFunctions.DestroyWindow(hwnd.Handle);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }
    }
}
