using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Enums.Sekai
{
    public static class Members
    {
        public static Dictionary<string, int> characters = new Dictionary<string, int>()
        {
            {"星乃一歌",1 },
            {"一歌",1 },
            {"hoshinoichika",1 },
            {"ichika",1 },
            {"ick",1 },
            {"天马咲希",2 },
            {"咲希",2 },
            {"tenmasaki",2 },
            {"saki",2 },
            {"sk",2 },
            {"望月穗波",3 },
            {"穗波",3 },
            {"穗妈",3 },
            {"mochizukihonami",3 },
            {"honami",3 },
            {"hnm",3 },
            {"日野森志步",4 },
            {"志步",4 },
            {"honomorishiho",4 },
            {"shiho",4 },
            {"sh",4 },
            {"lisa",4 },
            {"lisane",4 },
            {"花里实乃里",5 },
            {"花里实乃理",5 },
            {"实乃里",5 },
            {"实乃理",5 },
            {"hanasatominori",5 },
            {"minori",5 },
            {"mnr",5 },
            {"桐谷遥",6 },
            {"遥",6 },
            {"kiritaniharuka",6 },
            {"haruka",6 },
            {"hrk",6 },
            {"桃井爱莉",7 },
            {"爱莉",7 },
            {"momoiairi",7 },
            {"airi",7 },
            {"ar",7 },
            {"日野森雫",8 },
            {"雫",8},
            {"hinomorishizuku",8 },
            {"shizuku",8 },
            {"szk",8 }
        };
        /// <summary>
        /// 根据昵称查找角色ID
        /// </summary>
        /// <param name="name">角色昵称</param>
        /// <returns>0：未找到；1-26：角色ID</returns>
        public static int NameToId(string name)
        {
            if (characters.ContainsKey(name))
            {
                return characters[name];    
            }
            else
            {
                return 0;
            }
        }
    }
}
