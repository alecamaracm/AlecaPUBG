using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib.TransferClasses
{
    public class ReplayGameList
    {
        public MiniGameData[] games;
    }

    public class MiniGameData
    {
        public string ranking;
        public string mode;
        public string date;
        public string time;
        public string mapName;
        public string kills;
        public string distance;
        public string id;
        public bool detailed;
    }
}
