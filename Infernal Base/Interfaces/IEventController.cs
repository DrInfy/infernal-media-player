#region Usings



#endregion

namespace Imp.Base.Interfaces
{
    public interface IEventController
    {
        void ShowError(ImpError error);
        void SetEvent(EventText eventText);
        void SetTitle(string title);
        void RefreshPosition();
    }
}