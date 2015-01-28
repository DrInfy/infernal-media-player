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

        public static ModifierKeys ModKeys => modKeys;
    }
}
