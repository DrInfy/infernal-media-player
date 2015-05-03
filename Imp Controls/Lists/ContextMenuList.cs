#region Usings

using System.Windows;
using Base.ListLogic;

#endregion

namespace ImpControls.Lists
{
    public class ContextMenuList : ImpListBox<ImpTextAndCommand>
    {
        public ContextMenuList() : base(false, false) {}

        public new Size DesiredSize()
        {
            return new Size(165, controller.VisibleCount * RowHeight + 3);
        }
    }
}