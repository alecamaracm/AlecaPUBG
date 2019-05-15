
using AlecaPUBGSharedLib;
using APIProxifierBaseClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGClientLib.Endpoints
{
    public class ConnectionStatusRequest:APIRequest
    {
        public ConnectionStatusRequest(APIBaseClient _baseClient) : base(_baseClient)
        {
            endpoint = (byte)Endpoint.ConnectionStatus;
        }

        public async Task<ConnectionStatusResponse> send()
        {
            ConnectionStatusResponse response = new ConnectionStatusResponse(await execute());
            
            if(response.result.errorOcurred==false)
            {
                response.serviceStatus = (ServiceStatus)Enum.Parse(typeof(ServiceStatus),response.result.data[0]);
                response.statusString = response.result.data[1];
            }

            return response;
        }     
       
    }

    public class ConnectionStatusResponse:BaseResponse
    {         
        public ConnectionStatusResponse(RequestResult _result)
        {
            result = _result;
        }

        

        public override string ToString()
        {
            return "status="+serviceStatus.ToString()+",statusString="+statusString;
        }

        public ServiceStatus serviceStatus;
        public string statusString;
    }
}
