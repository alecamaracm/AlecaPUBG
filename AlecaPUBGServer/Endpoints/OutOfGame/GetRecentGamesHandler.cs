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
    public class GetRecentGamesHandler : EndpointHandler
    {
        public GetRecentGamesHandler(Logger logger) : base(logger, "GetRecentGames", EndpointLoggingMode.All, "/OutOfGame", StaticServerData.debugTextBox)
        {

        }

        public override bool Handle(ref string[] data, out string[] returnData, ref string errorString, ServerConnection serverConnection)
        {
            returnData = new string[1];
            dataToLog = "Player name: " + data[0];

            
            UserDataResponse response = PUBGAPIHandler.getSinglePlayerDataByName(data[0]);
            if(response==null)
            {
                errorString = "UserDataResponse is null (API error?)";
                return true;
            }

            try
            {
                List<string> idList = new List<string>();
                foreach(var game in response.data[0].relationships.matches.data)
                {
                    if (idList.Count > 50) break; //Don't allow more than 50 IDs to be sent
                    idList.Add(game.id); 
                }
                returnData[0] = String.Join("|",idList.ToArray());
            }catch(Exception ex)
            {
                errorString = "Error: " + ex.Message;
                return true;
            }

            return false;
        }

        public override void HandleLog(ServerConnection serverConnection)
        {
            AddLog(dataToLog, serverConnection);
        }
    }
}
