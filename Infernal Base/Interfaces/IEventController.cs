#region Usings

using Imp.Controllers;

#endregion

namespace Base.Interfaces
{
    public interface IEventController
    {
        void ShowError(ImpError error);
        void SetEvent(EventText eventText);
        void SetTitle(string title);
    }
}