using AlecaPUBGClientLib.Endpoints;
using AlecaPUBGClientLib.InGame;
using APIProxifierBaseClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGClientLib
{
    public class AlecaPUBGClient
    {
        APIBaseClient APIclient;

        public AlecaPUBGClient()
        {
            APIclient = new APIBaseClient("173.249.27.233", 6968);
            //APIclient = new APIBaseClient("127.0.0.1", 6968);
        }

        public string test()
        {
            return APIclient.test();
        }

        public async Task<EchoResponse> echoRequest(string v)
        {
            EchoRequest request = new EchoRequest(APIclient);
            return await request.send(v);
        }

        public async Task<ConnectionStatusResponse> getConnectionStatus()
        {
            ConnectionStatusRequest request = new ConnectionStatusRequest(APIclient);
            return await request.send();
        }
     
        public async Task<EncryptedEchoResponse> encryptedEchoRequest(string v)
        {
            EncryptedEchoRequest request = new EncryptedEchoRequest(APIclient);
            return await request.send(v);
        }

        public async Task<RoasterInfoResponse> roasterInfoRequest(string gameMode,string map,string players)
        {
            RoasterInfoRequest request = new RoasterInfoRequest(APIclient);
            return await request.send(gameMode,map,players);
        }

        public async Task<TelemetryResponse> telemetryRequest(string username,string type,string extraData)
        {
            HeatmapTelemetryRequest request = new HeatmapTelemetryRequest(APIclient);
            return await request.send(username,type,extraData);
        }

        public async Task<GetRecentGamesResponse> getRecentGamesRequest(string data)
        {
            GetRecentGamesRequest request = new GetRecentGamesRequest(APIclient);
            return await request.send(data);
        }
    }
}
