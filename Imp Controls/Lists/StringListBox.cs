using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpControls.Lists
{
    public class StringListBox: ImpListBox<String>
    {
        public StringListBox()
            : base(false, false)
        {
        }
    }
}
