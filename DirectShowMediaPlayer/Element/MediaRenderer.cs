#region Usings

using System;
using Imp.Base.Interfaces;
using Imp.Base.Libraries;
using Imp.DirectShow.Player;

#endregion

namespace Imp.DirectShow.Element
{
    public class MediaRenderer: D3DRenderer
    {
        /// <summary>
        /// Is executes when a new D3D surfaces has been allocated
        /// </summary>
        /// <param name="pSurface">The pointer to the D3D surface</param>
        internal void OnMediaPlayerNewAllocatorSurfacePrivate(object sender, IntPtr pSurface)
        {
            SetBackBuffer(pSurface);
        }

        internal void OnMediaPlayerNewAllocatorFramePrivate()
        {
            InvalidateVideoImage();
        }
    }
}