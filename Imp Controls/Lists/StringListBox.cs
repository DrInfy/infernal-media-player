#region Usings

using System;

#endregion

namespace Imp.Controls.Lists
{
    public class StringListBox : ImpListBox<String>
    {
        public StringListBox()
            : base(false, false) {}
    }
}