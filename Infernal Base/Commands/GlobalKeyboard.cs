using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Base.Commands
{
    public static class GlobalKeyboard
    {
        private static ModifierKeys modKeys;

        public static void SetModifierKeys(ModifierKeys _modKeys)
        {
            modKeys = _modKeys;
        }

        public static ModifierKeys ModKeys
        {
            get { return modKeys; }
        }
    }
}
