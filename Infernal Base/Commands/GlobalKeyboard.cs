#region Usings

using System.Windows.Input;

#endregion

namespace Imp.Base.Commands
{
    public static class GlobalKeyboard
    {
        #region Static Fields and Constants

        private static ModifierKeys modKeys;

        #endregion

        #region Properties

        public static ModifierKeys ModKeys => modKeys;

        #endregion

        public static void SetModifierKeys(ModifierKeys _modKeys)
        {
            modKeys = _modKeys;
        }
    }
}