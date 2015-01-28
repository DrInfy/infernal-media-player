using System.Collections.Generic;
using System.Windows;
using Base.Interfaces;
using Base.ListLogic;

namespace Base.Controllers
{
    public interface IControllerContextMenu<TCmdType, in TMenuType> : IBaseController<TCmdType>
    {
        void ContextMenu(Point cursorPositionInDesktop, TMenuType menuPosition);

        void CreateContextMenu(Point cursorPositionInDesktop, List<TextAndCommand<TCmdType>> cmdList);
    }

    public interface IBaseController<in TCmdType>
    {
        IEventController EventC { get; }
        void Exec(TCmdType cmd, object arg = null);
    }

    public interface IUpdateable
    {
        void Update();
    }
}