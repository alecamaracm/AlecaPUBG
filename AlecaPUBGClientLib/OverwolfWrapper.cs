using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using AlecaPUBGClientLib.InGame;
using AlecaPUBGSharedLib.TransferClasses;
using AlecaPUBGSharedLib.PUBG_API_Classes;
using System.IO.Compression;
using AlecaPUBGClientLib.Properties;
using AlecamarLIB;
using AlecaPUBGSharedLib;
using System.Globalization;

namespace AlecaPUBGClientLib
{
    public class OverwolfWrapper
    {
        AlecaPUBGClient client = new AlecaPUBGClient();
        
        public int currentGameUpdateID = 0;

        public LocalSavedData data;

        public event Action<object> gameDataReady;

        MyPlayerData[] lastRoasterInfo;
        MyPlayerData[] lastRoasterInfoToKeep;

        string lastMap = "";

        public event Action<object> heatmapDataReady;

        public event Action<object,object> heatmapProgressReady;

        public CurrentEventRecorder currentRecorder;

        public float myLastX, myLastY, myLastZ;

        public string currentGamePUBGID="";

        public OverwolfWrapper()
        {
           /* data = new LocalSavedData();
            data.Load();*/
        }        

        public void initialize(Action<object> callback)
        {
            callback("OK!");

            Task.Run(() => {
                try
                {
                    var aas = this.client.telemetryRequest(Settings.Default.username, "Initialized!","").Result;
                }
                catch { }
            });

        }
        

        public void test(string echo, Action<object> callback)
        {
            Task.Run(() => {
                callback("Echo: " + echo);
            });
        }
        
        public void requestNewGame(string gameMode,string map,string players,Action<object> callback)
        {
            Task.Run(() => {
                try
                {
                    lastMap = map;
                    var a = client.roasterInfoRequest(gameMode, map, players).Result;
                    gameDataReady(JsonConvert.SerializeObject(a.playerData));
                    lastRoasterInfo = a.playerData;
                    lastRoasterInfoToKeep = lastRoasterInfo;
                    Task.Run(() => {
                        Thread.Sleep(60*2*1000);  //Removes the lastRoasterInfo in 2 mins
                        lastRoasterInfo = null;
                    });

                    currentRecorder = new CurrentEventRecorder(DateTime.Now); //Start a new event recorder
                    callback("OK!");
                }catch(Exception ex)
                {
                    callback("Error: "+ex.Message);
                }
           
            });
        }

        public void addNewEvent(string videoPath,string eventType,string eventData,Action<object>callback)
        {
            switch(eventType)
            {
                case "id":
                    currentGamePUBGID = eventData;
                    break;
                default:
                    currentRecorder.events.Add(new RecorderEvent() { lastX=myLastX,lastY=myLastY,lastZ=myLastZ,videoPath=videoPath,eventType=eventType});
                    break;                
                case "endOfGame":
                    currentRecorder.events.Add(new RecorderEvent() { lastX = myLastX, lastY = myLastY, lastZ = myLastZ, videoPath = videoPath, eventType = eventType });
                    saveCurrentEvents();
                    currentRecorder = null;
                    break;
            }       
        }

        private void saveCurrentEvents()
        {
            string filePath =new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\SavedEvents\\game-" + currentGamePUBGID;
            if(File.Exists(filePath)==false)
            {
                StreamWriter writer = new StreamWriter(filePath);
                writer.Write(JsonConvert.SerializeObject(currentRecorder));
                writer.Close();
            }

            Task.Run(() => {
                try
                {
                    var aas = this.client.telemetryRequest(Settings.Default.username, "newReplayDataSaved", String.Format("GameID={0}", currentGamePUBGID)).Result;
                }
                catch { }
            });
        }

        public void setLastPosition(string x,string y,string z,Action<object> callback)
        {
            myLastX = float.Parse(x);
            myLastY = float.Parse(y);
            myLastZ = float.Parse(z);
            currentRecorder.events.Add(new RecorderEvent() { lastX = myLastX, lastY = myLastY, lastZ = myLastZ, videoPath = "", eventType = "location" });
        }


        public void getMapReplay(string gameID,Action<object,object> callback)
        {


            Task.Run(() => {
            try
            {
                    CurrentEventRecorder pastRecorder=null;

                    var fileList = Directory.GetFiles(new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\SavedEvents\\");

                    foreach (var entry in fileList)
                    {
                        if(entry.Contains(gameID))
                        {
                            using (StreamReader reader = new StreamReader(entry))
                            {
                                pastRecorder = JsonConvert.DeserializeObject<CurrentEventRecorder>(reader.ReadToEnd());
                            }                         
                            break;
                        }
                    }

                    Task.Run(() => {
                        try
                        {
                            var aas = this.client.telemetryRequest(Settings.Default.username, "replayRequest", String.Format("GameID={0}, FoundVideoData={1}",gameID,pastRecorder!=null)).Result;
                        }
                        catch { }
                    });

                    if (pastRecorder==null)
                    {
                        callback("Error", "");
                        return;
                    }

                    MyWebClient client = new MyWebClient();
                    client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.api+json");
                    ResponseMatch matchResponse = JsonConvert.DeserializeObject<ResponseMatch>(client.DownloadString("https://api.pubg.com/shards/steam/matches/" + gameID));

                    string assetID = matchResponse.data.relationships.assets.data[0].id;
                    string telemetryURL = matchResponse.included.Where(p => p.id == assetID).First().attributes.URL;

                    client = new MyWebClient();
                    client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.api+json");
                    client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
                    var responseStream = new GZipStream(client.OpenRead(telemetryURL), CompressionMode.Decompress);

                    TelemetryOnlyLandingEvent[] events;

                    using (StreamReader reader2 = new StreamReader(responseStream))
                    using (JsonReader reader = new JsonTextReader(reader2))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        events = serializer.Deserialize<TelemetryOnlyLandingEvent[]>(reader);
                    }

                    ReplayGameMap map = new ReplayGameMap();

                    float mLastX=0, mLastY=-12323.78f;

                    string currentMode = "plane";

                    bool inVehicle = false;

                    for(int i=2;i<pastRecorder.events.Count;i++)
                    {
                        if(pastRecorder.events[i].eventType=="location")
                        {
                            var e = pastRecorder.events[i];
                            if(getDistance(mLastX, e.lastX, mLastY, e.lastY)>10)
                            {
                                mLastX = e.lastX;
                                mLastY = e.lastY;

                                switch(currentMode)
                                {
                                    case "plane":
                                        map.planelocations.Add(new RawLandingLocations((int)e.lastX, (int)e.lastY).ToDataPoint(matchResponse.data.attributes.mapName));
                                        break;
                                    case "fly":
                                        map.flyLocations.Add(new RawLandingLocations((int)e.lastX, (int)e.lastY).ToDataPoint(matchResponse.data.attributes.mapName));
                                        break;
                                    case "normal":
                                        map.locations.Add(new RawLandingLocations((int)e.lastX, (int)e.lastY).ToDataPoint(matchResponse.data.attributes.mapName));
                                        map.locationsVehicle.Add(inVehicle);
                                        break;
                                }
                                  
                            }
                        }else
                        {
                            if(pastRecorder.events[i].eventType != "airJump" && pastRecorder.events[i].eventType != "inVehicle" && pastRecorder.events[i].eventType != "outVehicle")
                            {
                                if (Directory.Exists(new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\Videos\\" + gameID) == false)
                                    Directory.CreateDirectory(new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\Videos\\" + gameID);

                                if(System.IO.File.Exists(pastRecorder.events[i].videoPath))
                                {
                                    System.IO.File.Move(pastRecorder.events[i].videoPath,
                                        new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\Videos\\"+gameID+"/"+i+".mp4");
                                }

                                map.bigEvents.Add(new BigEvent()
                                {
                                    type = pastRecorder.events[i].eventType,
                                    videoFile = "../NET/Videos/"+gameID + "/" + i + ".mp4",
                                    point = new RawLandingLocations((int)pastRecorder.events[i].lastX, (int)pastRecorder.events[i].lastY).ToDataPoint(matchResponse.data.attributes.mapName)
                                });


                            }
                         
                            if ( pastRecorder.events[i].eventType == "landed")
                            {
                                currentMode = "normal";
                                map.locations.Add(new RawLandingLocations((int)mLastX, (int)mLastY).ToDataPoint(matchResponse.data.attributes.mapName));
                                map.locationsVehicle.Add(false);
                                map.flyLocations.Add(new RawLandingLocations((int)mLastX, (int)mLastY).ToDataPoint(matchResponse.data.attributes.mapName));
                            }

                            if (pastRecorder.events[i].eventType == "airJump")
                            {
                                currentMode = "fly";
                                map.planelocations.Add(new RawLandingLocations((int)mLastX, (int)mLastY).ToDataPoint(matchResponse.data.attributes.mapName));
                                map.flyLocations.Add(new RawLandingLocations((int)mLastX, (int)mLastY).ToDataPoint(matchResponse.data.attributes.mapName));

                            }

                            if (pastRecorder.events[i].eventType == "inVehicle") inVehicle = true;
                            if (pastRecorder.events[i].eventType == "outVehicle") inVehicle = false;
                        }
                    }

                    callback("OK!", JsonConvert.SerializeObject(map));
                    
            }
            catch
            {
                callback("Error", "");
            }

            });
        }

        public float getDistance(float x1,float x2,float y1,float y2)
        {
            return (float)Math.Sqrt(Math.Pow(x1-x2,2)+ Math.Pow(y1 - y2, 2));
        }

        public void setSettings(string newUsername,string lowSpec,Action<object>callback)
        {
            Settings.Default.username = newUsername;
            Settings.Default.lowSpec = bool.Parse(lowSpec);
            Settings.Default.Save();


            Task.Run(() => {
                try
                {
                    var a = client.telemetryRequest(Settings.Default.username, "settingsUpdated", "").Result;
                }
                catch { }
            });

            callback("OK!");
        }

        public void getSettings(Action<object,object,object>callback)
        {
            callback("OK!", Settings.Default.username, Settings.Default.lowSpec.ToString());
        }
  

        public void requestLastRoasterInfo(Action<object,object> callback)
        {
            Task.Run(() => {
                try
                {
                    var a = client.telemetryRequest(Settings.Default.username, "returnedRoasterInfo", "").Result;
                }
                catch { }
            });


            Task.Run(() => {
                try
                {
                    if (lastRoasterInfo == null)
                    {
                        callback("Error", "");
                    }
                    else
                    {
                        callback("OK!", JsonConvert.SerializeObject(lastRoasterInfo));
                        lastRoasterInfo = null;
                    }
                }
                catch
                {
                    callback("Error","");
                }
               
            });
        }

        public void getRecentGames(Action<object,object> callback)
        {

            Task.Run(() => {
                try
                {
                    var response = client.getRecentGamesRequest(Settings.Default.username).Result;
                    if (response.result.errorOcurred)
                    {
                        callback("Error", "");
                    }
                    else
                    {
                        ReplayGameList list = new ReplayGameList();

                        var fileList = Directory.GetFiles(new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\SavedEvents\\");
                                              

                        list.games = new MiniGameData[Math.Min(response.gameIDs.Length,25)];
                        
                      

                        Parallel.For(0, list.games.Length, (index) => {
                            MyWebClient client = new MyWebClient();
                            client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.api+json");
                            ResponseMatch matchResponse = JsonConvert.DeserializeObject<ResponseMatch>(client.DownloadString("https://api.pubg.com/shards/steam/matches/" +response.gameIDs[index]));
                            MiniGameData miniData= new MiniGameData();

                            miniData.date = matchResponse.data.attributes.createdAt.ToLocalTime().ToShortDateString();
                            miniData.time = matchResponse.data.attributes.createdAt.ToLocalTime().ToShortTimeString();
                            var participant = matchResponse.included.Where(p => p.type == "participant" && p.attributes.stats.name == Settings.Default.username).First();
                            miniData.ranking = "#"+participant.attributes.stats.winPlace;
                            miniData.mode = matchResponse.data.attributes.gameMode.OnlyFirstToUpper();
                            miniData.mapName = getRealMapName(matchResponse.data.attributes.mapName);
                            miniData.distance = Math.Round((float)(participant.attributes.stats.walkDistance + participant.attributes.stats.swimDistance + participant.attributes.stats.rideDistance) / 1000, 2) + " Km";
                            miniData.kills = participant.attributes.stats.kills+"";
                            miniData.id = response.gameIDs[index];

                            miniData.detailed = false;
                            foreach (var entry in fileList)
                            {
                                if (entry.Contains(miniData.id))
                                {
                                    miniData.detailed = true;
                                    break;
                                }
                            }                         
                            list.games[index] = miniData;
                        });
                                                                       
                        callback("OK!", JsonConvert.SerializeObject(list));
                    }

                    Task.Run(() => {
                        try
                        {
                            var a = client.telemetryRequest(Settings.Default.username, "replaysOpened", "Error="+ response.result.errorOcurred).Result;
                        }
                        catch { }
                    });
                }
                catch
                {
                    callback("Error", "");
                }

            });
        }





        public void getConnectionStatus(Action<object, object> callback)
        {
            Task.Run(() => {
                try
                {
                    var a = client.getConnectionStatus().Result;
                    callback(a.serviceStatus.ToString(), a.statusString);
                }
                catch
                {
                    callback("Error", "General error");
                }
            });
        }
                       

        public void requestHeatmapData(Action<object> callback)
        {
            Task.Run(() => {
                callback(getRealMapName(lastMap));
                createNewHeatmap();
            });
        }

        private void createNewHeatmap()
        {
            int maxGamesPerPlayer = 25;

          

            List<string> ids = new List<string>();
            for(int i=0;i<lastRoasterInfoToKeep.Length;i++)
            {
                for(int k=0;k<Math.Min(lastRoasterInfoToKeep[i].matchIDs.Length, maxGamesPerPlayer);k++)
                {
                    if(ids.Contains(lastRoasterInfoToKeep[i].matchIDs[k])==false) ids.Add(lastRoasterInfoToKeep[i].matchIDs[k]);
                }
            }
            ThreadPool.SetMinThreads(50, 50);
            ThreadPool.SetMaxThreads(50, 50);

            MyMatchData[] games=getMatchLandingData(ids.ToArray(),lastMap);

            HeatmapData heatmapData = new HeatmapData();
            heatmapData.players = new HeatmapPlayer[lastRoasterInfoToKeep.Length];
            for(int i=0;i<lastRoasterInfoToKeep.Length;i++)
            {
                heatmapData.players[i] = new HeatmapPlayer();
                heatmapData.players[i].name = lastRoasterInfoToKeep[i].unsername;
                heatmapData.players[i].gamesPlayed = lastRoasterInfoToKeep[i].matchIDs.Length;
                heatmapData.players[i].dataPoints = new List<HeatmapDataPoints>();
            }

            for(int i=0;i<games.Length;i++)
            {
                for(int u=0;u<heatmapData.players.Length;u++)
                {
                    if (games[i].rawLandingLocationsByPlayer.ContainsKey(heatmapData.players[u].name))
                        heatmapData.players[u].dataPoints.Add(games[i].rawLandingLocationsByPlayer[heatmapData.players[u].name].ToDataPoint(games[i].mapID));
                }
            }
            string logData = "";
            try
            {
               
                for (int i = 0; i < heatmapData.players.Length; i++)
                {
                    logData += String.Format("{0}={1}, ", heatmapData.players[i].name, heatmapData.players[i].dataPoints.Count);
                }
            }
            catch
            {

            }           

            Task.Run(() => {
                try
                {
                    var a = client.telemetryRequest(Settings.Default.username, "heatmapOpen",logData).Result;
                }
                catch { }
            });

            heatmapDataReady(JsonConvert.SerializeObject(heatmapData));
        }

        private MyMatchData[] getMatchLandingData(string[] ids,string mapID)
        {
            int count = 0;

            List<MyMatchData> toReturn = new List<MyMatchData>();
            Parallel.For(0,ids.Length, (index) => {
                Console.WriteLine("Starting index: " + index);
                string landingFileDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Cache/LandingMatchData/" + ids[index];
                if (File.Exists(landingFileDir))
                {
                    using (StreamReader reader = new StreamReader(landingFileDir))
                    {
                        MyMatchData data = JsonConvert.DeserializeObject<MyMatchData>(reader.ReadToEnd());
                        if(data.mapID==mapID)
                        {
                            toReturn.Add(data);
                        }
                    }
                }
                else
                {
                    try
                    {
                        MyWebClient client = new MyWebClient();
                        client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.api+json");
                        ResponseMatch response = JsonConvert.DeserializeObject<ResponseMatch>(client.DownloadString("https://api.pubg.com/shards/steam/matches/" + ids[index]));

                        if(response.data.attributes.mapName!=mapID)
                        {
                            return; //This match is not played in the desired map
                        }

                        string assetID = response.data.relationships.assets.data[0].id;
                        string telemetryURL = response.included.Where(p => p.id == assetID).First().attributes.URL;

                        client = new MyWebClient();
                        client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.api+json");
                        client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
                        var responseStream = new GZipStream(client.OpenRead(telemetryURL), CompressionMode.Decompress);

                        TelemetryOnlyLandingEvent[] events;
                        using (StreamReader reader2 = new StreamReader(responseStream))
                        using (JsonReader reader = new JsonTextReader(reader2))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            events = serializer.Deserialize<TelemetryOnlyLandingEvent[]>(reader);
                        }

                        MyMatchData matchData = new MyMatchData();
                        matchData.matchID = ids[index];
                        matchData.mapID = response.data.attributes.mapName;
                                               

                        for (int i = 0; i < events.Length; i++)
                        {
                            if (events[i]._T == "LogParachuteLanding")
                            {
                                if (matchData.rawLandingLocationsByPlayer.ContainsKey(events[i].character.name) == false)
                                {
                                    TelemetryEventCharacter character = events[i].character;
                                    matchData.rawLandingLocationsByPlayer.Add(character.name, new RawLandingLocations()
                                    {
                                        x = character.location.x / 100, //Now in m
                                        y = character.location.y / 100,
                                        heigth = character.location.z / 100,
                                    });
                                }
                            }
                        }

                        toReturn.Add(matchData);

                        //Writing the raw telemetry data as it is very large 20MB/game
                        using (StreamWriter writer = new StreamWriter(landingFileDir))
                        {
                            writer.Write(JsonConvert.SerializeObject(matchData));
                        }
                    }
                    catch
                    {

                    }
                }
                count++;
                heatmapProgressReady(count,ids.Length);
            });
            return toReturn.ToArray();
        }

        public string getRealMapName(string mapInternalName)
        {
            switch (mapInternalName)
            {
                case "Desert_Main":
                    return "Miramar";
                case "DihorOtok_Main":
                    return "Vikendi";
                case "Erangel_Main":
                    return "Erangel";
                case "Range_Main":
                    return "Camp Jackal";
                case "Savage_Main":
                    return "Sanhok";
                default:
                    return "UKNOWN";
            }
        }


    }

    class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 15 * 1000;  //15 secs timeout for the match data requests
            return w;
        }
    }

    public class LocalSavedData
    {
        public const string relativeFilePath = "saveData.txt";
               
        public void Load()
        {
            LocalSavedData datax = new LocalSavedData();

            try
            {
                if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/" + relativeFilePath))
                {
                    using (StreamReader reader = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/" + relativeFilePath))
                        datax = JsonConvert.DeserializeObject<LocalSavedData>(reader.ReadToEnd());
                }
            }
            catch
            {

            }
        }



        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/" + relativeFilePath))
                writer.Write(JsonConvert.SerializeObject(this));
        }

        

    }



    public enum LoLStatus
    {
        ClientClosed,
        ClientOpened,
        ChampSelect,
        InGame,
    }
}
