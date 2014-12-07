using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Libraries;

namespace Base.ListLogic
{
    public class DoubleString
    {
        public string Value;
        public string DisplayName;


        public DoubleString(string value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }


        public override bool Equals(object obj)
        {
            var dString = obj as DoubleString;
            return dString != null && String.Compare(Value, dString.Value, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
