using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars.Hubs
{
    public enum LogsActionName
    {
        RUN,
        STOP,
        PAUSE,
        RESUME,
        RUN_DETECT,
        STOP_DETECT,
        PAUSE_DETECT,
        RESUME_DETECT,
        PARAMETER_CHANGED,
        PARAMETER_CHANGED_DETECT,
        CONNECTED,
        DISCONNECTED,
        CONNECTED_DETECT,
        DISCONNECTED_DETECT,
        WAITING,
        WAITING_DETECT,
        RUNOUT,
        RUNOUT_DETECT,
        REGISTER,
        ADDMORE,
        NOT_INIT,
        PLC_NOT_FOUND,
        RUNOUT_WAITING,
        RUNOUT_WAITING_DETECT,

    }
}
