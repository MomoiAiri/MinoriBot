using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Enums.Bandori
{
    internal class LeaderBoard
    {
        /// <summary>
        /// 
        /// </summary>
        public List<PointsItem> points { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<UsersItem> users { get; set; }
    }
    public class PointsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public long time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long uid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int value { get; set; }
    }

    public class UsersItem
    {
        /// <summary>
        /// 
        /// </summary>
        public long uid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 轻轻回来不吵醒往事，就当我从来不曾远离
        /// </summary>
        public string introduction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int rank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int strained { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> degrees { get; set; }
    }
}
