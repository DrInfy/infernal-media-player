#region Usings

using System;
using Imp.Base.Commands;

#endregion

namespace Imp.Base.ListLogic
{
    public class TextAndCommand<T>
    {
        #region Fields

        public readonly string Text;
        public readonly T Command;
        public readonly Func<object> Argument;

        #endregion

        public TextAndCommand(string text, T command, Func<object> argument = null)
        {
            Text = text;
            Command = command;
            Argument = argument;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class ImpTextAndCommand : TextAndCommand<ImpCommand>
    {
        public ImpTextAndCommand(string text, ImpCommand command, Func<object> argument = null) : base(text, command, argument) {}
    }
}