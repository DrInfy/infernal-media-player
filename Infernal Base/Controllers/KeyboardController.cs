﻿using System.Collections.Generic;
using Base.Commands;
using Base.Interfaces;

namespace Base.Controllers
{
    public abstract class KeyboardController<TCmdType> : IUpdateable, IBaseController<TCmdType>
    {
        protected ImpKeyboard<TCmdType> keyboard;
        protected bool globalKeyboard;
        public abstract bool Focused { get; }
        public abstract bool Selected { get; }
        public IEventController EventC { get; protected set; }
        public PlayerStyle AllowedStyles { get; protected set; }

        public abstract void Exec(TCmdType cmd, object arg = null);

        public virtual void Update()
        {
            List<KeyCommand<TCmdType>> cmdList = keyboard.Update(Selected, AllowedStyles);
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