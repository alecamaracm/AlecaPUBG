using APIProxifierBaseServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGServer.Endpoints.Testing
{
    public class EchoHandler : EndpointHandler
    {
        public EchoHandler(Logger logger) : base(logger, "Echo", EndpointLoggingMode.All, "/Testing", StaticServerData.debugTextBox)
        {

        }

        public override bool Handle(ref string[] data, out string[] returnData, ref string errorString, ServerConnection serverConnection)
        {
            returnData =new string[] { "Echo" };
            //dataToLog = UTF8Encoding.UTF8.GetString(data);
            return false;
        }

        public override void HandleLog(ServerConnection serverConnection)
        {
            AddLog(dataToLog, serverConnection);
        }
    }
}
