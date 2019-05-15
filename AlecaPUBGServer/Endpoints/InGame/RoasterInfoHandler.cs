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

namespace AlecaPUBGServer.Endpoints.InGame
{
    public class RoasterInfoHandler : EndpointHandler
    {
        public RoasterInfoHandler(Logger logger) : base(logger, "RoasterInfo", EndpointLoggingMode.All, "/InGame", StaticServerData.debugTextBox)
        {

        }

        public override bool Handle(ref string[] data, out string[] returnData, ref string errorString, ServerConnection serverConnection)
        {
            returnData =new string[] { data[0] };
            String[] usernames = JsonConvert.DeserializeObject<String[]>(data[2]);

            if(data[0]=="")
            {
                if (usernames.Length == 4) data[0] = "squad";
                if (usernames.Length == 3) data[0] = "squad";
                if (usernames.Length == 2) data[0] = "duo";
                if (usernames.Length == 1) data[0] = "solo";       
            }

            if (data[0] == "") data[0] = "squad";

            dataToLog = "Players: " + String.Join(",", usernames) + " | Mode: " + data[0] + " | Map: " + data[1];

            bool rateLimited = false;

            String[] ids = PUBGAPIHandler.getPlayerIDs(usernames);
            MyPlayerData[] datax = PUBGAPIHandler.getPlayerSeasonData(ids,data[0],true,out rateLimited);  //TODO: implemen the FPP mode (Now it is always TPP)

            if (rateLimited) dataToLog += " --- RATE LIMITED!";

            for(int i=0;i<usernames.Length;i++)
            {
                datax[i].unsername = usernames[i];
            }

            returnData = new string[usernames.Length];
            for (int i = 0; i < usernames.Length; i++)
            {
                returnData[i] = JsonConvert.SerializeObject(datax[i]);
            }

            return false;
        }

        public override void HandleLog(ServerConnection serverConnection)
        {
            AddLog(dataToLog, serverConnection);
        }
    }
}
