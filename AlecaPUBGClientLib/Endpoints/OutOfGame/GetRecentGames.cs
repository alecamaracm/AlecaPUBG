
using AlecaPUBGSharedLib;
using AlecaPUBGSharedLib.TransferClasses;
using APIProxifierBaseClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGClientLib.InGame
{
    public class GetRecentGamesRequest : APIRequest
    {
        public GetRecentGamesRequest(APIBaseClient _baseClient) : base(_baseClient)
        {
            endpoint = (byte)Endpoint.GetRecentGames;
        }

        public async Task<GetRecentGamesResponse> send(string username)
        {
            APIargs.Add(username);

            GetRecentGamesResponse response = new GetRecentGamesResponse(await execute());

            if (response.result.errorOcurred == false)
            {
                if(response.result.data[0]==null || response.result.data[0]=="")
                {
                    response.gameIDs = new string[0];
                }else
                {
                    response.gameIDs = response.result.data[0].Split('|');
                }        
            }

            return response;
        }

    }

    public class GetRecentGamesResponse : BaseResponse
    {
        public GetRecentGamesResponse(RequestResult _result)
        {
            result = _result;
        }

        public String[] gameIDs;

        public override string ToString()
        {
            return String.Format("[gameIDs={0}]",gameIDs.Length);
        }
           
    }
}
