using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib.TransferClasses
{
    public class HeatmapData
    {
        public HeatmapPlayer[] players;
    }

    public class HeatmapPlayer
    {
        public string name = "";
        public int gamesPlayed = 0;
        public List<HeatmapDataPoints> dataPoints;

    }

    public class HeatmapDataPoints
    {
        public int x; //Between 0-1000 ALWAYS!!!!    |   MAP-INDEPENDENT
        public int y;
    }
}
