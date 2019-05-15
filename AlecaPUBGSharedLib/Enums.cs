using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib
{
    public enum Endpoint
    {
        Echo,
        EchoEncrypted,      
        ConnectionStatus,
        RoasterInfoRequest,
        Telemetry,
        GetRecentGames,
    }

    public enum ServiceStatus
    {
        NonInitialized,
        Working,
        Outage,
        Down,
    }
}
