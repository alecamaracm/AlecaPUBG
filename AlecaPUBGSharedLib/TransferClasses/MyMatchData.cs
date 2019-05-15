using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib.TransferClasses
{
    public class MyMatchData
    {
        public Dictionary<string, RawLandingLocations> rawLandingLocationsByPlayer = new Dictionary<string, RawLandingLocations>();
        public string matchID = "";
        public string mapID = "";
    }

    public class RawLandingLocations
    {
        public float x;
        public float y;
        public float heigth;

        public RawLandingLocations()
        {

        }

        public RawLandingLocations(float lastX, float lastY)
        {
            this.x = lastX;
            this.y = lastY;
        }

        public HeatmapDataPoints ToDataPoint(string mapID)
        {
            float mapSize = getMapSizeByID(mapID);
            return new HeatmapDataPoints() { x = (int)Math.Round((x/mapSize)*1000), y=(int)Math.Round((y / mapSize) * 1000) };
        }

        public static float getMapSizeByID(string mapID)  //Number of meters
        {
            switch (mapID)
            {
                case "Desert_Main": //Miramar
                    return 8160;
                case "DihorOtok_Main": //Vikendi
                    return 6120;
                case "Erangel_Main": //Erangel
                    return 8160;
                case "Savage_Main": //Sanhok
                    return 4080;
                default:
                    return -1;  //Camp Jackal(Range_Main) and any others
            }
        }
    }

}
