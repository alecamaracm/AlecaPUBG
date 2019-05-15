using AlecaPUBGSharedLib.PUBG_API_Classes;
using AlecaPUBGSharedLib.TransferClasses;
using APIProxifierBaseServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGServer.Endpoints.Telemetry
{
    public class TelemetryHandler : EndpointHandler
    {
        public TelemetryHandler(Logger logger) : base(logger, "Telemetry", EndpointLoggingMode.All, "/Telemetry", StaticServerData.debugTextBox)
        {

        }

        public override bool Handle(ref string[] data, out string[] returnData, ref string errorString, ServerConnection serverConnection)
        {
            returnData =new string[] {  };

            dataToLog = String.Format("Username: {0}. | Action: {1}. | Extra data: {2}",data[0],data[1],data[2]);

            return false;
        }

        public override void HandleLog(ServerConnection serverConnection)
        {
            AddLog(dataToLog, serverConnection);
        }
    }
}
