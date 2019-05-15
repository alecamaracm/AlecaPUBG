
using AlecaPUBGSharedLib;
using APIProxifierBaseClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGClientLib.Endpoints
{
    public class EchoRequest:APIRequest
    {
        public EchoRequest(APIBaseClient _baseClient) : base(_baseClient)
        {
            endpoint = (byte)Endpoint.Echo;
        }

        public async Task<EchoResponse> send(string toGetEchoed)
        {        
        
            APIargs.Add(toGetEchoed);

            EchoResponse response = new EchoResponse(await execute());
            
            if(response.result.errorOcurred==false)
            {
                response.echoed = response.result.data[0];
            }

            return response;
        }     
       
    }

    public class EchoResponse:BaseResponse
    {         
        public EchoResponse(RequestResult _result)
        {
            result = _result;
        }

        public override string ToString()
        {
            return result.ToString() + "-" + echoed;
        }

        public string echoed;
    }
}
