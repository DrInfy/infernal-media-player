#region Usings

using System.Collections.Generic;
using System.Windows.Input;

#endregion

namespace Base.Commands
{
    public class ImpKeyboard<TCmdType>
    {
        #region Fields

        private readonly List<KeyCommand<TCmdType>> activeCommands;
        private readonly List<KeyCommand<TCmdType>> commands;
        private readonly KeyboardState currentState;
        private readonly KeyboardState lastState;
        private ModifierKeys modifierKeys;

        #endregion

        #region Properties

        public ModifierKeys ModKeys => modifierKeys;

        #endregion

        public ImpKeyboard()
        {
            currentState = new KeyboardState();
            lastState = new KeyboardState();
            commands = new List<KeyCommand<TCmdType>>(50);
            activeCommands = new List<KeyCommand<TCmdType>>(5);
        }

        public void Add(KeyCommand<TCmdType> command)
        {
            commands.Add(command);
        }

        public List<KeyCommand<TCmdType>> Update(bool isWindowActive, PlayerStyle allowedStyles)
        {
            activeCommands.Clear();

            // save last to previous state
            currentState.CopyTo(lastState);
            currentState.Update(); // get new state

            modifierKeys = ModifierKeys.None;
            if (currentState.Get(Key.LeftShift) || currentState.Get(Key.RightShift))
                modifierKeys = ModifierKeys.Shift;
            else
                modifierKeys = ModifierKeys.None;

            if (currentState.Get(Key.LeftAlt) || currentState.Get(Key.RightAlt))
                modifierKeys |= ModifierKeys.Alt;

            if (currentState.Get(Key.LeftCtrl) || currentState.Get(Key.RightCtrl))
                modifierKeys |= ModifierKeys.Control;

            foreach (var impCommand in commands)
            {
                if (!HasAnyFlagInCommon(impCommand.AllowedStyle, allowedStyles)
                    || (!isWindowActive && !impCommand.Anywhere))
                    continue;

                if (IsNewKeyPressed(impCommand.Key) && impCommand.ModifierKeys == modifierKeys)
                {
                    activeCommands.Add(impCommand);
                }
                else if (!impCommand.NeedRelease && currentState.Get(impCommand.Key) &&
                         impCommand.ModifierKeys == modifierKeys)
                {
                    if (impCommand.TickPress())
                        activeCommands.Add(impCommand);
                }
            }

            return activeCommands;
        }

        public static bool HasAnyFlagInCommon(PlayerStyle type, PlayerStyle value)
        {
            return (type & value) != 0;
        }

        public bool IsNewKeyPressed(Key key)
        {
            return (!lastState.Get(key) && currentState.Get(key));
        }

        public void Add(List<KeyCommand<TCmdType>> keyCommands)
        {
            foreach (var command in keyCommands)
            {
                Add(command);
            }
        }
    }
}