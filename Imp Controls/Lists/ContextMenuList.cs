#region Usings

using System;
using System.Windows;
using System.Windows.Input;
using Base.ListLogic;

#endregion

namespace ImpControls.Lists
{
    public class ContextMenuList : ImpListBox<ImpTextAndCommand>
    {
        public ContextMenuList() : base(false, false)
        {
            //this.IsManipulationEnabled = false;
        }

        public new Size DesiredSize()
        {
            return new Size(165, controller.VisibleCount * sStyle.RowHeight + 3);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var tempIndex = LowIndex + (int)Math.Floor(e.GetPosition(this).Y / sStyle.RowHeight);
            this.Select(tempIndex);
        }

        protected override void OnPreviewTouchMove(TouchEventArgs e)
        {
            var tempIndex = LowIndex + (int)Math.Floor(e.GetTouchPoint(this).Position.Y / sStyle.RowHeight);
            this.Select(tempIndex);
            base.OnPreviewTouchMove(e);
        }
    }
}