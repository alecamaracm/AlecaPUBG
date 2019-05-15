using AlecaPUBGSharedLib;
using AlecaPUBGSharedLib.PUBG_API_Classes;
using AlecaPUBGSharedLib.TransferClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlecaPUBGServer
{
    public static class PUBGAPIHandler
    {
        static DateTime rateLimitReset = DateTime.MinValue;

        static Dictionary<string, string> playerIDByName = new Dictionary<string, string>();


        public static UserDataResponse getSinglePlayerDataByName(string username)
        {
            if (rateLimitReset.Ticks > DateTime.Now.Ticks)  //We are over the rate limit already . Don´t allow the request to go through
            {
                return null;
            }

            //Have to request the ID
            int rertyLeft = 5;

            ret:

            while (rateLimitReset.Ticks > DateTime.Now.Ticks)
            {
                Thread.Sleep(1000); //Rate limit exceeded
            }

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.api+json");
                client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + StaticServerData.PUBG_API_KEY);
                UserDataResponse userData = JsonConvert.DeserializeObject<UserDataResponse>(client.DownloadString("https://api.pubg.com/shards/steam/players?filter[playerNames]=" + username));
                return userData;
            }
            catch (Exception ex)
            {
                if (rertyLeft > 0)
                {
                    rertyLeft--;
                    goto ret;
                }
                else
                {
                    Console.WriteLine("Exceeded max retry count. Error: " + ex.Message);

                    return null;
                }
            }
        }
                                          
        public static string[] getPlayerIDs(string[] usernames)
        {
            string[] toReturn = new string[usernames.Length];

            bool allCached = true;
            for(int i=0;i<usernames.Length;i++)
            {
                if (playerIDByName.ContainsKey(usernames[i]))  //Is in cache?
                {
                    toReturn[i] = playerIDByName[usernames[i]];
                }else
                {
                    allCached = false;
                }
            }

            if(allCached==false)  //We request ALL usernames if only 1 of them in not in the cache (it only takes 1 request, so its fine)
            {
                if (rateLimitReset.Ticks > DateTime.Now.Ticks)  //We are over the rate limit already . Don´t allow the request to go through
                {
                    for (int i = 0; i < usernames.Length; i++)
                    {
                        if (toReturn[i] == "") toReturn[i] = "RATE_LIMIT";  //If not in cache, write the rate limit error
                    }
                    return toReturn;
                }


                //Have to request the ID
                int rertyLeft = 5;

                ret:

                while (rateLimitReset.Ticks > DateTime.Now.Ticks)
                {
                    Thread.Sleep(1000); //Rate limit exceeded
                }

                try
                {
                    WebClient client = new WebClient();
                    client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.api+json");
                    client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + StaticServerData.PUBG_API_KEY);
                    UsernameLookupResponse userData = JsonConvert.DeserializeObject<UsernameLookupResponse>(client.DownloadString("https://api.pubg.com/shards/steam/players?filter[playerNames]=" + String.Join(",", usernames)));
                    for (int i = 0; i < usernames.Length; i++)  //The returned object is not ordered, so we search for the right one
                    {
                        for(int k=0;k<userData.data.Length;k++)
                        {
                            if(usernames[i]==userData.data[k].attributes.name)  //Found the right name
                            {
                                if (playerIDByName.ContainsKey(usernames[i]) == false) playerIDByName.Add(usernames[i], userData.data[k].id);
                                toReturn[i] = userData.data[k].id;  //We change to ERROR the id returned if not in cache
                            }
                        }

                    
                    }
                }
                catch (Exception ex)
                {
                    if (rertyLeft > 0)
                    {
                        rertyLeft--;
                        goto ret;
                    }
                    else
                    {
                        Console.WriteLine("Exceeded max retry count. Error: " + ex.Message);

                        for(int i=0;i<usernames.Length;i++)
                        {
                            if (toReturn[i] == "") toReturn[i] = "ERROR";  //We change to ERROR the id returned if not incache
                        }                       
                    }
                }
            }




            return toReturn;
           
        }



        public static MyPlayerData[] getPlayerSeasonData(string[] ids, string gamemode, bool thirdPerson, out bool rateLimited)
        {


            MyPlayerData[] toReturn = new MyPlayerData[ids.Length];

            if (rateLimitReset.Ticks > DateTime.Now.Ticks)  //We are over the rate limit already . Don´t allow the request to go through
            {
                for (int i = 0; i < toReturn.Length; i++)
                {
                    toReturn[i] = new MyPlayerData();
                }
                rateLimited = true;
                return toReturn;
            }

            Parallel.For(0, ids.Length, (i) => {

                try
                {
                    //Have to request the ID
                    int rertyLeft = 5;

                    ret:

                    while (rateLimitReset.Ticks > DateTime.Now.Ticks)
                    {
                        Thread.Sleep(1000); //Rate limit exceeded
                    }
                    StatsLookupResponse userData;
                    try
                    {
                        WebClient client = new WebClient();
                        client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.api+json");
                        client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + StaticServerData.PUBG_API_KEY);
                        userData = JsonConvert.DeserializeObject<StatsLookupResponse>(client.DownloadString("https://api.pubg.com/shards/steam/players/" + ids[i] + "/seasons/" + StaticServerData.currentSeason));


                    }
                    catch (WebException ex)
                    {
                        switch ((int)(ex.Response as HttpWebResponse).StatusCode)
                        {
                            case 429: //Rate limit exceded, set timeout and wait

                                try
                                {
                                    rateLimitReset = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(long.Parse(ex.Response.Headers["X-RateLimit-Reset"])).ToLocalTime();

                                    Console.WriteLine("PUBG API rate limit exceded, blocking requests until " + rateLimitReset.ToLongTimeString());

                                }
                                catch
                                {
                                    Console.WriteLine("Can´t set the rate limit reset time. Using 1 min instead");
                                    rateLimitReset = DateTime.Now.AddMinutes(1);
                                }


                                toReturn[i] = new MyPlayerData();
                                return;

                            default:
                                if (rertyLeft > 0)
                                {
                                    rertyLeft--;
                                    goto ret;
                                }
                                else
                                {
                                    Console.WriteLine("Exceeded max retry count. Error: " + ex.Message);

                                    toReturn[i] = new MyPlayerData();
                                    return;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        toReturn[i] = new MyPlayerData();
                        return;
                    }

                    StatsLookupStats stats = getRightStats(userData, gamemode, thirdPerson);

                    int totalGames = stats.wins + stats.losses;

                    toReturn[i] = new MyPlayerData();
                    toReturn[i].wins = stats.wins;
                    toReturn[i].losses = stats.losses;

                    if (totalGames == 0) totalGames = 1;
                    if (stats.losses == 0) stats.losses = 1;

                    toReturn[i].damagePerGame = (int)(stats.damageDealt / (totalGames));
                    toReturn[i].kda = (float)Math.Round((float)stats.kills / (stats.losses), 2);

                    toReturn[i].rankTitle = EnumHelper.getRankTitleByRank((int)stats.rankPoints);
                    toReturn[i].skinNumber = (int)(stats.rideDistance % 12) + 1;
                    toReturn[i].SP = (int)stats.rankPoints;
                    // toReturn[i].unsername Must be set outside of the method
                    toReturn[i].winPercentage = (float)Math.Round(stats.wins / (float)stats.losses, 2);
                    toReturn[i].myTitle = "UKNOWN";

                    StatsLookupMatches matches = getRightMatchList(userData, gamemode, thirdPerson);


                    toReturn[i].matchIDs = new string[matches.data.Length];
                    for (int k = 0; k < toReturn[i].matchIDs.Length; k++)
                    {
                        toReturn[i].matchIDs[k] = matches.data[k].id;
                    }
                } catch
                {
                    toReturn[i] = new MyPlayerData();
                    return;
                }


            });

            if (toReturn.Length == 4)
            {
                for(int i=0;i<3;i++)
                {
                    if(toReturn[i].SP== toReturn.Max(p => p.SP) && toReturn[i].myTitle == "UKNOWN")
                    {
                        toReturn[i].myTitle = "The best";
                        break;
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    if (toReturn[i].kda == toReturn.Max(p => p.kda) && toReturn[i].myTitle== "UKNOWN")
                    {
                        toReturn[i].myTitle = "The killer";
                        break;
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    if (toReturn[i].wins == toReturn.Max(p => p.wins) && toReturn[i].myTitle == "UKNOWN")
                    {
                        toReturn[i].myTitle = "The experienced";
                        break;
                    }
                }

                for (int i = 0; i < 3; i++)
                {
        
                    if (toReturn[i].damagePerGame == toReturn.Max(p => p.damagePerGame) && toReturn[i].myTitle == "UKNOWN")
                    {
                        toReturn[i].myTitle = "The dealer";
                        break;
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    if (toReturn[i].skinNumber == toReturn.Max(p => p.skinNumber) && toReturn[i].myTitle == "UKNOWN")
                    {
                        toReturn[i].myTitle = "The healer";
                        break;
                    }
                }
            }
            
            for(int i=0;i<toReturn.Length;i++)
            {
                if (i == 0 && toReturn[i].myTitle == "UKNOWN") toReturn[i].myTitle = "The bastard";
                if (i == 1 && toReturn[i].myTitle == "UKNOWN") toReturn[i].myTitle = "The sanitizer";
                if (i == 2 && toReturn[i].myTitle == "UKNOWN") toReturn[i].myTitle = "The runner";
                if (i == 3 && toReturn[i].myTitle == "UKNOWN") toReturn[i].myTitle = "The hipster";
            }
            

            rateLimited = (rateLimitReset.Ticks > DateTime.Now.Ticks);

            return toReturn;
        }

        private static StatsLookupStats getRightStats(StatsLookupResponse userData, string gamemode, bool thirdPerson)
        {
            switch(gamemode)
            {
                case "squad":
                    return thirdPerson ? userData.data.attributes.gameModeStats.squad : userData.data.attributes.gameModeStats.squadfpp;
                case "duo":
                    return thirdPerson ? userData.data.attributes.gameModeStats.duo : userData.data.attributes.gameModeStats.duofpp;
                case "solo":
                    return thirdPerson ? userData.data.attributes.gameModeStats.solo : userData.data.attributes.gameModeStats.solofpp;
                default:
                    return new StatsLookupStats();
            }
        }

        private static StatsLookupMatches getRightMatchList(StatsLookupResponse userData, string gamemode, bool thirdPerson)
        {
            switch (gamemode)
            {                
                case "squad":
                    return thirdPerson ? userData.data.relationships.matchesSquad : userData.data.relationships.matchesSquadFPP;
                case "duo":
                    return thirdPerson ? userData.data.relationships.matchesDuo : userData.data.relationships.matchesDuoFPP;
                case "solo":
                    return thirdPerson ? userData.data.relationships.matchesSolo : userData.data.relationships.matchesSoloFPP;
                default:
                    return new StatsLookupMatches();
            }
        }
    }
}
