using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Interfaces
{
    public interface IStateButton
    {
        bool IsEnabled { get; set; }
        int CheckStates { get; set; }
        int CurrentState { get; set; }
    }
}
