using AlecaPUBGServer.Endpoints.InGame;
using AlecaPUBGServer.Endpoints.Telemetry;
using AlecaPUBGServer.Endpoints.Testing;
using AlecaPUBGSharedLib;
using APIProxifierBaseServer;

namespace AlecaPUBGServer
{
    class AlecaPUBGServer : APIBaseServer
    {
        public AlecaPUBGServer()
        {
            StaticServerData.logger = new Logger();
            StaticServerData.serviceStatus = ServiceStatus.Working;
        }

        public void SaveData()
        {
           
        }

        public void ApplyEndpoints()
        {
            AddHandler((byte)Endpoint.Echo, new EchoHandler(StaticServerData.logger));
            AddHandler((byte)Endpoint.EchoEncrypted, new EncryptedEchoHandler(StaticServerData.logger));         
            AddHandler((byte)Endpoint.ConnectionStatus, new ConnectionStatusHandler(StaticServerData.logger));
            AddHandler((byte)Endpoint.RoasterInfoRequest, new RoasterInfoHandler(StaticServerData.logger));
            AddHandler((byte)Endpoint.Telemetry, new TelemetryHandler(StaticServerData.logger));
            AddHandler((byte)Endpoint.GetRecentGames, new GetRecentGamesHandler(StaticServerData.logger));
        }
    }
}
