using System;
using System.Globalization;

namespace SEdge.Core.Texts
{
    public static class ColorTranslator
    {
        #region Common

        public static CustomColor FromHtml(string hexColor)
        {
            uint argb = UInt32.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber);
            return new CustomColor(argb);
        }

        public static CustomColor FromHex(string redHex, string greenHex, string blueHex)
        {
            return CustomColor.FromRgb(
                int.Parse(redHex, NumberStyles.HexNumber),
                int.Parse(greenHex, NumberStyles.HexNumber),
                int.Parse(blueHex, NumberStyles.HexNumber));
        }

        #endregion
    }
}