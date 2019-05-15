using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib.PUBG_API_Classes
{

    public class ResponseMatch
    {
        public ResponseMatchData data { get; set; }
        public ResponseMatchIncluded[] included { get; set; }
    }

    public class ResponseMatchData
    {
        public string type { get; set; }
        public string id { get; set; }
        public ResponseMatchAttributes attributes { get; set; }
        public ResponseMatchRelationships relationships { get; set; }
    }

    public class ResponseMatchAttributes
    {
        public bool isCustomMatch { get; set; }
        public DateTime createdAt { get; set; }
        public object stats { get; set; }
        public string gameMode { get; set; }
        public string shardId { get; set; }
        public string mapName { get; set; }
        public int duration { get; set; }
        public string titleId { get; set; }
        public object tags { get; set; }
        public string seasonState { get; set; }
    }

    public class ResponseMatchRelationships
    {
        public ResponseMatchRosters rosters { get; set; }
        public ResponseMatchAssets assets { get; set; }
    }

    public class ResponseMatchRosters
    {
        public ResponseMatchDatum[] data { get; set; }
    }

    public class ResponseMatchDatum
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class ResponseMatchAssets
    {
        public ResponseMatchDatum1[] data { get; set; }
    }

    public class ResponseMatchDatum1
    {
        public string type { get; set; }
        public string id { get; set; }
    }
      

    public class ResponseMatchMeta
    {
    }

    public class ResponseMatchIncluded
    {
        public string type { get; set; }
        public string id { get; set; }
        public ResponseMatchAttributes1 attributes { get; set; }
        public ResponseMatchRelationships1 relationships { get; set; }
    }

    public class ResponseMatchAttributes1
    {
        public string shardId { get; set; }
        public ResponseMatchStats stats { get; set; }
        public string actor { get; set; }
        public string won { get; set; }
        public DateTime createdAt { get; set; }
        public string URL { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class ResponseMatchStats
    {
        public int DBNOs { get; set; }
        public int assists { get; set; }
        public int boosts { get; set; }
        public float damageDealt { get; set; }
        public string deathType { get; set; }
        public int headshotKills { get; set; }
        public int heals { get; set; }
        public int killPlace { get; set; }
        public int killPoints { get; set; }
        public int killPointsDelta { get; set; }
        public int killStreaks { get; set; }
        public int kills { get; set; }
        public int lastKillPoints { get; set; }
        public int lastWinPoints { get; set; }
        public float longestKill { get; set; }
        public int mostDamage { get; set; }
        public string name { get; set; }
        public string playerId { get; set; }
        public int rankPoints { get; set; }
        public int revives { get; set; }
        public float rideDistance { get; set; }
        public int roadKills { get; set; }
        public float swimDistance { get; set; }
        public int teamKills { get; set; }
        public float timeSurvived { get; set; }
        public int vehicleDestroys { get; set; }
        public float walkDistance { get; set; }
        public int weaponsAcquired { get; set; }
        public int winPlace { get; set; }
        public int winPoints { get; set; }
        public int winPointsDelta { get; set; }
        public int rank { get; set; }
        public int teamId { get; set; }
    }

    public class ResponseMatchRelationships1
    {
        public ResponseMatchTeam team { get; set; }
        public ResponseMatchParticipants participants { get; set; }
    }

    public class ResponseMatchTeam
    {
        public object data { get; set; }
    }

    public class ResponseMatchParticipants
    {
        public ResponseMatchDatum2[] data { get; set; }
    }

    public class ResponseMatchDatum2
    {
        public string type { get; set; }
        public string id { get; set; }
    }

}
