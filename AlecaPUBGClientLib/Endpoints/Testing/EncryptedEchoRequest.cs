
using AlecaPUBGSharedLib;
using APIProxifierBaseClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGClientLib.Endpoints
{
    public class EncryptedEchoRequest:APIRequest
    {
        public EncryptedEchoRequest(APIBaseClient _baseClient) : base(_baseClient)
        {
            isEncrypted = true;
            endpoint = (byte)Endpoint.EchoEncrypted;
        }

        public async Task<EncryptedEchoResponse> send(string toGetEchoed)
        {           
            APIargs.Add(toGetEchoed);

            EncryptedEchoResponse response = new EncryptedEchoResponse(await execute());
            
            if(response.result.errorOcurred==false)
            {
                response.echoed = response.result.data[0];
            }

            return response;
        }

      
       
    }

    public class EncryptedEchoResponse : BaseResponse
    {         
        public EncryptedEchoResponse(RequestResult _result)
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
