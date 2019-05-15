using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib.PUBG_API_Classes
{

    public class UserDataResponse
    {
        public UserDataDatum[] data { get; set; }
        public UserDataLinks links { get; set; }
        public UserDataMeta meta { get; set; }
    }

    public class UserDataLinks
    {
        public string self { get; set; }
    }

    public class UserDataMeta
    {
    }

    public class UserDataDatum
    {
        public string type { get; set; }
        public string id { get; set; }
        public UserDataAttributes attributes { get; set; }
        public UserDataRelationships relationships { get; set; }
        public UserDataLinks1 links { get; set; }
    }

    public class UserDataAttributes
    {
        public object stats { get; set; }
        public string titleId { get; set; }
        public string shardId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string patchVersion { get; set; }
        public string name { get; set; }
    }

    public class UserDataRelationships
    {
        public UserDataAssets assets { get; set; }
        public UserDataMatches matches { get; set; }
    }

    public class UserDataAssets
    {
        public object[] data { get; set; }
    }

    public class UserDataMatches
    {
        public UserDataDatum1[] data { get; set; }
    }

    public class UserDataDatum1
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class UserDataLinks1
    {
        public string self { get; set; }
        public string schema { get; set; }
    }

}
