using AlecaPUBGSharedLib;
using APIProxifierBaseServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlecaPUBGServer
{
    public static class StaticServerData
    {
        public static TextBox debugTextBox;
        public static Logger logger;
               

        public static string PUBG_API_KEY = "apiKey";
        public static ServiceStatus serviceStatus = ServiceStatus.NonInitialized;
        public static string serviceStatusString = "";

        public static string currentSeason = "division.bro.official.pc-2018-03";

    }
}
