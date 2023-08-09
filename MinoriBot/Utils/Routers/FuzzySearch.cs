using MinoriBot.Enums.Sekai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.Routers
{
    internal static class FuzzySearch
    {
        public static Dictionary<string, string> FuzzySearchCharacter(string[] keywords)
        {
            Dictionary<string,string> result = new Dictionary<string,string>();
            foreach (string word in keywords)
            {
                if (!result.ContainsKey("attribute") && NickName.attribute.ContainsKey(word))
                {
                    result.Add("attribute", NickName.attribute[word]);
                    continue;
                }
                if(!result.ContainsKey("group") && NickName.groups.ContainsKey(word))
                {
                    result.Add("group", NickName.groups[word]);
                    continue;
                }
                if(!result.ContainsKey("character") && NickName.characters.ContainsKey(word))
                {
                    result.Add("character", NickName.characters[word].ToString());
                    continue;
                }
                if(!result.ContainsKey("star") && NickName.stars.ContainsKey(word))
                {
                    result.Add("star", NickName.stars[word]);
                    continue;
                }
            }
            return result;
        }
        public static Dictionary<string,string> FuzzySearchEvent(string[] keywords)
        {
            return null;
        }
    }
}
