using System;
using System.Windows.Input;

namespace Base.Commands
{
    public class KeyCommand<T>
    {
        public T Command;
        public Key Key;
        public ModifierKeys ModifierKeys;
        public bool Anywhere;
        public bool NeedRelease;
        public bool Pressed;
        public long PressInternal;
        public long LastPressed;
        public PlayerStyle AllowedStyle = PlayerStyle.All;

        public object Argument = null;

        /// <summary>
        /// Gets a single tick press. and updates interval timing
        /// </summary>
        /// <returns> true if the press tick is ready.</returns>
        public bool TickPress()
        {
            if ( DateTime.Now.Ticks  - LastPressed > PressInternal)
            {
                LastPressed = DateTime.Now.Ticks;
                return true;
            }

            return false;
        }
    }
}