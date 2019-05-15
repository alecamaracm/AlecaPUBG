﻿using APIProxifierBaseServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGServer.Endpoints.Testing
{
    public class ConnectionStatusHandler : EndpointHandler
    {
        public ConnectionStatusHandler(Logger logger) : base(logger, "ConnectionStatus", EndpointLoggingMode.All, "/Testing", StaticServerData.debugTextBox)
        {

        }

        public override bool Handle(ref string[] data, out string[] returnData, ref string errorString, ServerConnection serverConnection)
        {
            returnData = new string[] { StaticServerData.serviceStatus.ToString(),StaticServerData.serviceStatusString };
            dataToLog = "Status: " + returnData[0]+", statusString: " + returnData[1];
            return false;
        }

        public override void HandleLog(ServerConnection serverConnection)
        {
            AddLog(dataToLog, serverConnection);
        }
    }
}
