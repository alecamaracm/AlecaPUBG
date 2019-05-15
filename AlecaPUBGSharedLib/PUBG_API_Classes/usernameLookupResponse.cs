using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib.PUBG_API_Classes
{
 

    public class UsernameLookupResponse
    {
        public UsernameLookupData[] data { get; set; }
    }


    public class UsernameLookupData
    {
        public string type { get; set; }
        public string id { get; set; }
        public UsernameLookupAttributes attributes { get; set; }
     }

    public class UsernameLookupAttributes
    {
        public string name { get; set; }
        public object stats { get; set; }
        public string titleId { get; set; }
        public string shardId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string patchVersion { get; set; }
    }    
}
