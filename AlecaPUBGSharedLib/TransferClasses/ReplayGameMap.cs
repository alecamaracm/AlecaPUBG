using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib.TransferClasses
{
    public class ReplayGameMap
    {
        public List<HeatmapDataPoints> planelocations = new List<HeatmapDataPoints>();
        public List<HeatmapDataPoints> flyLocations = new List<HeatmapDataPoints>();
        public List<HeatmapDataPoints> locations = new List<HeatmapDataPoints>();
        public List<bool> locationsVehicle = new List<bool>();
        public List<BigEvent> bigEvents = new List<BigEvent>();
    }

    public class BigEvent
    {
        public string videoFile;
        public string type;
   
        public HeatmapDataPoints point;
    }

    
}
