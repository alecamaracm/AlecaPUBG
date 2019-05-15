using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib.TransferClasses
{
    public class MyPlayerData
    {
        public int skinNumber = 0;
        public string unsername = "uknown";

        public int wins = 0;
        public int losses = 0;
        public float winPercentage = 0.0f;

        public float kda = 0.0f;
        public int damagePerGame = 0;

        public string rankTitle = "Uknown";
        public int SP = 0;

        public string myTitle = "The uknown";

        public string[] matchIDs = new string[0];
    }
}
