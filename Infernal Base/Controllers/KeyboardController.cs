#region Usings

using System.Collections.Generic;
using Imp.Base.Commands;
using Imp.Base.Interfaces;

#endregion

namespace Imp.Base.Controllers
{
    public abstract class KeyboardController<TCmdType> : IUpdateable, IBaseController<TCmdType>
    {
        #region Fields

        protected ImpKeyboard<TCmdType> keyboard;
        protected bool globalKeyboard;

        #endregion

        #region Properties

        public abstract bool Focused { get; }
        public abstract bool Selected { get; }
        public IEventController EventC { get; protected set; }
        public PlayerStyle AllowedStyles { get; protected set; }

        #endregion

        public abstract void Exec(TCmdType cmd, object arg = null);

        public virtual void Update()
        {
            var cmdList = keyboard.Update(Selected, AllowedStyles);
            GlobalKeyboard.SetModifierKeys(keyboard.ModKeys);
            if (Focused)
            {
                foreach (var command in cmdList)
                {
                    Exec(command.Command, command.Argument);
                }
            }
        }

        public abstract List<KeyCommand<TCmdType>> GenerateDefaultKeyCommands();

        public virtual void Initialize(bool isGlobalKeyboard)
        {
            keyboard = new ImpKeyboard<TCmdType>();
            keyboard.Add(GenerateDefaultKeyCommands());
            if (isGlobalKeyboard)
            {
                globalKeyboard = true;
                GlobalKeyboard.SetModifierKeys(keyboard.ModKeys);
            }
        }
    }
}