using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib.PUBG_API_Classes
{

    public class StatsLookupResponse
    {
        public StatsLookupData data { get; set; }
    }

    public class StatsLookupData
    {
        public string type { get; set; }
        public StatsLookupAttributes attributes { get; set; }
        public StatsLookupRelationships relationships { get; set; }
    }

    public class StatsLookupAttributes
    {
        public StatsLookupGamemodeStats gameModeStats { get; set; }
    }

    public class StatsLookupGamemodeStats
    {
        public StatsLookupStats duo { get; set; }
        public StatsLookupStats duofpp { get; set; }
        public StatsLookupStats solo { get; set; }
        public StatsLookupStats solofpp { get; set; }
        public StatsLookupStats squad { get; set; }
        public StatsLookupStats squadfpp { get; set; }
    }

    public class StatsLookupStats
    {
        public int assists { get; set; }
        public float bestRankPoint { get; set; }
        public int boosts { get; set; }
        public int dBNOs { get; set; }
        public int dailyKills { get; set; }
        public int dailyWins { get; set; }
        public float damageDealt { get; set; }
        public int days { get; set; }
        public int headshotKills { get; set; }
        public int heals { get; set; }
        public int killPoints { get; set; }
        public int kills { get; set; }
        public float longestKill { get; set; }
        public float longestTimeSurvived { get; set; }
        public int losses { get; set; }
        public int maxKillStreaks { get; set; }
        public float mostSurvivalTime { get; set; }
        public float rankPoints { get; set; }
        public string rankPointsTitle { get; set; }
        public int revives { get; set; }
        public float rideDistance { get; set; }
        public int roadKills { get; set; }
        public int roundMostKills { get; set; }
        public int roundsPlayed { get; set; }
        public int suicides { get; set; }
        public float swimDistance { get; set; }
        public int teamKills { get; set; }
        public float timeSurvived { get; set; }
        public int top10s { get; set; }
        public int vehicleDestroys { get; set; }
        public float walkDistance { get; set; }
        public int weaponsAcquired { get; set; }
        public int weeklyKills { get; set; }
        public int weeklyWins { get; set; }
        public int winPoints { get; set; }
        public int wins { get; set; }
    }
    
    public class StatsLookupRelationships
    {
        public StatsLookupMatches matchesSolo { get; set; }
        public StatsLookupMatches matchesSoloFPP { get; set; }
        public StatsLookupMatches matchesDuo { get; set; }
        public StatsLookupMatches matchesDuoFPP { get; set; }
        public StatsLookupMatches matchesSquad { get; set; }
        public StatsLookupMatches matchesSquadFPP { get; set; }
    }
   
    public class StatsLookupMatches
    {
        public StatsLookupMatch[] data { get; set; }
    }

    public class StatsLookupMatch
    {
        public string type { get; set; }
        public string id { get; set; }
    }

}
