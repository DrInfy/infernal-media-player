using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Imp.MpvPlayer
{
    public static class PointerReader
    {
        public static double ReadDouble(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
            {
                return 0;
            }

            return Marshal.PtrToStructure<double>(pointer);
        }
    }
}
