using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlecaPUBGSharedLib
{
    public static class EnumHelper
    {
        public static string getRankTitleByRank(int rank)
        {
            if (rank < 1) return "Unranked";
            if (rank < 1000) return "Beginner";
            if (rank < 2000) return "Novice";
            if (rank < 3000) return "Experienced";
            if (rank < 4000) return "Skilled";
            if (rank < 5000) return "Specialist";
            if (rank < 6000) return "Expert";
            return "Survivor";
        }
    }
}
