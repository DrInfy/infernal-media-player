namespace Imp.Base
{
    public enum EventType
    {
        Instant,
        Delayed
    }

    public struct EventText
    {
        #region Fields

        public string Text;
        public double Duration;
        public EventType EventType;

        #endregion

        public EventText(string text, double duration = 1, EventType type = EventType.Instant)
        {
            Text = text;
            Duration = duration;
            EventType = type;
        }
    }
}