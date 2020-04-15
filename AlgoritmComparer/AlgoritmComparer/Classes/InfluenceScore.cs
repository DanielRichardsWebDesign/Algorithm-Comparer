using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmComparer.Classes
{
    public class InfluenceScore
    {
        public int user { get; set; }
        public string top_10_friend_recommendations { get; set; }

        public List<int> userFriendsList { get; set; }
    }
}
