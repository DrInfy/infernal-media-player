namespace Imp.Controllers
{
    public enum EventType
    {
        Instant,
        Delayed
    }

    public struct EventText
    {
        public string Text;
        public double Duration;
        public EventType EventType;


        public EventText(string text, double duration = 1, EventType type = EventType.Instant)
        {
            Text = text;
            Duration = duration;
            EventType = type;
        }
    }
}