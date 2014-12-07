using System.Windows;
using Base;

namespace Imp.Controllers
{
    public interface IPanelController
    {
        void CommandPanelOpen(PanelCommand? panelCommand);
        void CommandPanelPlaylist(PanelCommand? panelCommand);

        void RememberThisPanelPosition(Point cursorPosition, Point cursorOnControl);

        void PanelPanLeft(Point cursorPosition);

        void PanelPanRight(Point cursorPosition);
        /// <summary>
        /// Ensures that grid doesn't break down with all columns being 0
        /// </summary>
        void CheckMainGrid();

        void Update();
        void CheckPanelHide(Point cursorPosition);
        void HideBottomAndTop();
        void ShowBottomAndTop();

        /// <summary>
        /// Called when window is resized to ensure no panel is getting too small to actually display
        /// </summary>
        void CheckResize();

        void HideRightPanel();
        void HideLeftPanel();
    }
}