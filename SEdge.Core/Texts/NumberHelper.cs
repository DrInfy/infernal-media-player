using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEdge.Core.Texts
{
    public static class NumberHelper
    {
        
        public static bool IsInteger(string text)
        {
            int i;
            return int.TryParse(text, out i);
        }

        public static bool IsNumber(string text, CultureInfo ci = null)
        {
            double d;
            return double.TryParse(text, NumberStyles.Number, ci ?? TextHelper.Ci, out d);
        }

        public static double? AsDouble(string text, CultureInfo ci = null)
        {
            double d;
            if (text != null && double.TryParse(text, NumberStyles.Number, ci ?? TextHelper.Ci, out d))
            {
                return d;
            }

            return default(double);
        }

        public static double? AsNullableDouble(string text, CultureInfo ci = null)
        {
            double d;
            if (text != null && double.TryParse(text, NumberStyles.Number, ci ?? TextHelper.Ci, out d))
            {
                return d;
            }

            return null;
        }

        public static double? AsNullableInt32(string text, CultureInfo ci = null)
        {
            int i;
            if (text != null && int.TryParse(text, NumberStyles.Integer, ci ?? TextHelper.Ci, out i))
            {
                return i;
            }

            return null;
        }
        
        public static double? AsInt32(string text, CultureInfo ci = null)
        {
            int i;
            if (text != null && int.TryParse(text, NumberStyles.Integer, ci ?? TextHelper.Ci, out i))
            {
                return i;
            }

            return null;
        }
    }
}
