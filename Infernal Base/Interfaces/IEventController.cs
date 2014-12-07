using Imp.Controllers;

namespace Base.Interfaces
{
    public interface IEventController
    {
        void ShowError(ImpError error);
        void SetEvent(EventText eventText);
        void SetTitle(string title);
    }
}