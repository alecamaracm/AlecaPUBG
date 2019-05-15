using APIProxifierBaseServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGServer.Endpoints.Testing
{
    public class EncryptedEchoHandler : EndpointHandler
    {

        public EncryptedEchoHandler(Logger logger):base(logger,"EncryptedEcho",EndpointLoggingMode.All,"/Testing",StaticServerData.debugTextBox)
        {
           
        }

        public override bool Handle(ref string[] data, out string[] returnData, ref string errorString, ServerConnection serverConnection)
        {
            returnData =new string[] { "EncryptedEcho" };            
            dataToLog = data[0];
            return false;
        }

        public override void HandleLog(ServerConnection serverConnection)
        {
            AddLog(dataToLog, serverConnection);
        }
    }
}
