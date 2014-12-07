using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Base.ListLogic;

namespace ImpControls.Lists
{
    public class ContextMenuList : ImpListBox<ImpTextAndCommand>
    {
        public ContextMenuList() : base(false, false)
        {
        }


        public new Size DesiredSize()
        {
            return new Size(165, controller.VisibleCount * ISize + 3);
        }

    }
}
