
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
    public class HeatmapTelemetryRequest : APIRequest
    {
        public HeatmapTelemetryRequest(APIBaseClient _baseClient) : base(_baseClient)
        {
            endpoint = (byte)Endpoint.Telemetry;
        }

        public async Task<TelemetryResponse> send(string  username,string type,string extraData)
        {
            APIargs.Add(username);
            APIargs.Add(type);
            APIargs.Add(extraData);

            TelemetryResponse response = new TelemetryResponse(await execute());

            if (response.result.errorOcurred == false)
            {
                
            }

            return response;
        }

    }

    public class TelemetryResponse : BaseResponse
    {
        public TelemetryResponse(RequestResult _result)
        {
            result = _result;
        }    

        public override string ToString()
        {
            return "Telemetry{}";
        }
           
    }
}
