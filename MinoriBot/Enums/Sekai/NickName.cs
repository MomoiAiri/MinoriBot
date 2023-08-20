using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Enums.Sekai
{
    public static class NickName
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
            {"szk",8 },
            {"小豆沢心羽",9 },
            {"khn",9 },
            {"白石杏",10 },
            {"an",10 },
            {"an酱",10 },
            {"东云彰人",11 },
            {"akt",11 },
            {"张仁",11 },
            {"青柳冬弥",12 },
            {"toy",12 },
            {"toya",12 },
            {"董秘",12 },
            {"天马司",13 },
            {"tks",13 },
            {"凤笑梦",14 },
            {"emu",14 },
            {"旺大吼",14 },
            {"草薙宁宁",15 },
            {"nene",15 },
            {"神代类",16 },
            {"rui",16 },
            {"沈大雷",16 },
            {"宵崎奏",17 },
            {"knd",17 },
            {"朝比奈真冬",18 },
            {"mhy",18 },
            {"马忽悠",18 },
            {"东云绘名",19 },
            {"ena",19 },
            {"enana",19 },
            {"晓山瑞希",20 },
            {"mzk",20 },
            {"瑞希哥",20 }
        };
        public static Dictionary<string, string> attribute = new Dictionary<string, string>
        {
            {"mysterious","mysterious" },
            {"紫月","mysterious" },
            {"紫","mysterious" },
            {"月","mysterious" },
            {"cool","cool" },
            {"蓝星","cool" },
            {"蓝","cool" },
            {"星","cool" },
            {"pure","pure" },
            {"绿草","pure" },
            {"绿","pure" },
            {"草","pure" },
            {"happy","happy" },
            {"橙心","happy" },
            {"黄心","happy" },
            {"橙","happy" },
            {"黄","happy" },
            {"心","happy" },
            {"cute","cute" },
            {"粉花","cute" },
            {"粉","cute" },
            {"花","cute" },
        };
        public static Dictionary<string, string> groups = new Dictionary<string, string>()
        {
            {"vs","vs" },
            {"virtual singer","vs" },
            {"leoneed","ln" },
            {"ln","ln" },
            {"more more jumm","mmj" },
            {"mmj","mmj" },
            {"vivid bad squad","vbs" },
            {"vbs","vbs" },
            {"ws","ws" },
            {"25时","25" },
            {"25","25" }
        };
        public static Dictionary<string, string> stars = new Dictionary<string, string>()
        {
            {"一星","rarity_1" },
            {"1x","rarity_1" },
            {"二星","rarity_2" },
            {"2x","rarity_2" },
            {"三星","rarity_3" },
            {"3x","rarity_3" },
            {"四星","rarity_4" },
            {"4x","rarity_4" },
            {"生日","rarity_birthday" },
            {"bd","rarity_birthday" }
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
