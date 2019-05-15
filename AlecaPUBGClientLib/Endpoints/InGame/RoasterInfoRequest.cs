
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
    public class RoasterInfoRequest : APIRequest
    {
        public RoasterInfoRequest(APIBaseClient _baseClient) : base(_baseClient)
        {
            endpoint = (byte)Endpoint.RoasterInfoRequest;
        }

        public async Task<RoasterInfoResponse> send(string gameMode,string map,string team)
        {
            APIargs.Add(gameMode);
            APIargs.Add(map);
            APIargs.Add(team);

            RoasterInfoResponse response = new RoasterInfoResponse(await execute());

            if (response.result.errorOcurred == false)
            {
                response.playerData = new MyPlayerData[response.result.data.Length];

                for(int i=0;i<response.result.data.Length;i++)
                {
                    response.playerData[i] = JsonConvert.DeserializeObject<MyPlayerData>(response.result.data[i]);
                }
            }

            return response;
        }

    }

    public class RoasterInfoResponse : BaseResponse
    {
        public RoasterInfoResponse(RequestResult _result)
        {
            result = _result;
        }

        public MyPlayerData[] playerData;

        public override string ToString()
        {
            return String.Format("[dataCount={0}]",playerData.Length);
        }
           
    }
}
