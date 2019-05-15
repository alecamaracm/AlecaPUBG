using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib.PUBG_API_Classes
{
    public class TelemetryEvent
    {
        public string MatchId { get; set; }
        public string PingQuality { get; set; }
        public string SeasonState { get; set; }
        public DateTime _D { get; set; }
        public string _T { get; set; }
        public string accountId { get; set; }
        public TelemetryEventCommon common { get; set; }
        public TelemetryEventCharacter character { get; set; }
        public TelemetryEventItem item { get; set; }
        public TelemetryEventVehicle vehicle { get; set; }
        public int elapsedTime { get; set; }
        public int numAlivePlayers { get; set; }
        public int attackId { get; set; }
        public int fireWeaponStackCount { get; set; }
        public TelemetryEventAttacker attacker { get; set; }
        public string attackType { get; set; }
        public TelemetryEventItem weapon { get; set; }
        public string weaponId { get; set; }
        public int fireCount { get; set; }
        public int seatIndex { get; set; }
        public string mapName { get; set; }
        public string weatherId { get; set; }
        public TelemetryEventCharacter[] characters { get; set; }
        public string cameraViewBehaviour { get; set; }
        public int teamSize { get; set; }
        public bool isCustomGame { get; set; }
        public bool isEventMode { get; set; }
        public string blueZoneCustomOptions { get; set; }
        public TelemetryEventGamestate gameState { get; set; }
        public float rideDistance { get; set; }
        public float maxSpeed { get; set; }
        public float distance { get; set; }
        public TelemetryEventVictim victim { get; set; }
        public string damageTypeCategory { get; set; }
        public string damageReason { get; set; }
        public float damage { get; set; }
        public string damageCauserName { get; set; }
        public TelemetryEventItem parentItem { get; set; }
        public TelemetryEventItem childItem { get; set; }
        public string objectType { get; set; }
        public TelemetryEventLocation objectLocation { get; set; }
        public string[] damageCauserAdditionalInfo { get; set; }
        public bool isAttackerInVehicle { get; set; }
        public int dBNOId { get; set; }
        public TelemetryEventKillAssistant killer { get; set; }
        public TelemetryEventKillAssistant assistant { get; set; }
        public TelemetryEventVictimgameresult victimGameResult { get; set; }
        public float healAmount { get; set; }
        public int ownerTeamId { get; set; }
        public float swimDistance { get; set; }
        public float maxSwimDepthOfWater { get; set; }
        public TelemetryEventReviver reviver { get; set; }
        public TelemetryEventItempackage itemPackage { get; set; }
        public object[] rewardDetail { get; set; }
        public TelemetryEventGameresultonfinished gameResultOnFinished { get; set; }

    }

    public class TelemetryEventCommon
    {
        public float isGame { get; set; }
    }

    public class TelemetryEventCharacter
    {
        public string name { get; set; }
        public int teamId { get; set; }
        public float health { get; set; }
        public TelemetryEventLocation location { get; set; }
        public int ranking { get; set; }
        public string accountId { get; set; }
        public bool isInBlueZone { get; set; }
        public bool isInRedZone { get; set; }
        public string[] zone { get; set; }
    }


    public class TelemetryEventLocation
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class TelemetryEventItem
    {
        public string itemId { get; set; }
        public int stackCount { get; set; }
        public string category { get; set; }
        public string subCategory { get; set; }
        public string[] attachedItems { get; set; }
    }



    public class TelemetryEventVehicle
    {
        public string vehicleType { get; set; }
        public string vehicleId { get; set; }
        public float healthPercent { get; set; }
        public float feulPercent { get; set; }
    }

    public class TelemetryEventAttacker
    {
        public string name { get; set; }
        public int teamId { get; set; }
        public float health { get; set; }
        public TelemetryEvent location { get; set; }
        public int ranking { get; set; }
        public string accountId { get; set; }
        public bool isInBlueZone { get; set; }
        public bool isInRedZone { get; set; }
        public string[] zone { get; set; }
    }


    public class TelemetryEventGamestate
    {
        public int elapsedTime { get; set; }
        public int numAliveTeams { get; set; }
        public int numJoinPlayers { get; set; }
        public int numStartPlayers { get; set; }
        public int numAlivePlayers { get; set; }
        public TelemetryEventPosition safetyZonePosition { get; set; }
        public float safetyZoneRadius { get; set; }
        public TelemetryEventPosition poisonGasWarningPosition { get; set; }
        public float poisonGasWarningRadius { get; set; }
        public TelemetryEventPosition redZonePosition { get; set; }
        public float redZoneRadius { get; set; }
    }

    public class TelemetryEventPosition
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class TelemetryEventVictim
    {
        public string name { get; set; }
        public int teamId { get; set; }
        public float health { get; set; }
        public TelemetryEventPosition location { get; set; }
        public int ranking { get; set; }
        public string accountId { get; set; }
        public bool isInBlueZone { get; set; }
        public bool isInRedZone { get; set; }
        public string[] zone { get; set; }
    }

    


    public class TelemetryEventKillAssistant
    {
        public string name { get; set; }
        public int teamId { get; set; }
        public float health { get; set; }
        public TelemetryEventPosition location { get; set; }
        public int ranking { get; set; }
        public string accountId { get; set; }
        public bool isInBlueZone { get; set; }
        public bool isInRedZone { get; set; }
        public string[] zone { get; set; }
    }
       

    public class TelemetryEventVictimgameresult
    {
        public int rank { get; set; }
        public string gameResult { get; set; }
        public int teamId { get; set; }
        public TelemetryEventStats stats { get; set; }
        public string accountId { get; set; }
    }

    public class TelemetryEventStats
    {
        public int killCount { get; set; }
        public float distanceOnFoot { get; set; }
        public float distanceOnSwim { get; set; }
        public float distanceOnVehicle { get; set; }
        public float distanceOnParachute { get; set; }
        public float distanceOnFreefall { get; set; }
    }

    public class TelemetryEventReviver
    {
        public string name { get; set; }
        public int teamId { get; set; }
        public float health { get; set; }
        public TelemetryEventLocation location { get; set; }
        public int ranking { get; set; }
        public string accountId { get; set; }
        public bool isInBlueZone { get; set; }
        public bool isInRedZone { get; set; }
        public string[] zone { get; set; }
    }


    public class TelemetryEventItempackage
    {
        public string itemPackageId { get; set; }
        public TelemetryEventLocation location { get; set; }
        public TelemetryEventItem[] items { get; set; }
    }
    

    public class TelemetryEventGameresultonfinished
    {
        public object[] results { get; set; }
    }




}
