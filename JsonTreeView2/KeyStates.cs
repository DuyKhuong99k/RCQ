using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTreeView2
{
    [Flags]
    enum KeyStates
    {
        LeftButton = 1,
        RightButton = 2,
        Shift = 4,
        Control = 8,
        MiddleButton = 16,
        Alt = 32
    }
}
