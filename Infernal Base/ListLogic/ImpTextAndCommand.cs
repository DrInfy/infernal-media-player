using Base.Commands;

namespace Base.ListLogic
{
    public class TextAndCommand<T>
    {
        public readonly string Text;
        public readonly T Command;
        public readonly object Argument;


        public TextAndCommand(string text, T command, object argument = null)
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
        public ImpTextAndCommand(string text, ImpCommand command, object argument = null) : base(text, command, argument)
        {
        }
    }
}
