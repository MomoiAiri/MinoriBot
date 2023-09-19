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
        //public static Dictionary<string, string> FuzzySearchCharacter(string[] keywords)
        //{
        //    Dictionary<string,string> result = new Dictionary<string,string>();
        //    foreach (string word in keywords)
        //    {
        //        if (!result.ContainsKey("attribute") && NickName.attribute.ContainsKey(word))
        //        {
        //            result.Add("attribute", NickName.attribute[word]);
        //            continue;
        //        }
        //        if(!result.ContainsKey("group") && NickName.groups.ContainsKey(word))
        //        {
        //            result.Add("group", NickName.groups[word]);
        //            continue;
        //        }
        //        if(!result.ContainsKey("character") && NickName.characters.ContainsKey(word))
        //        {
        //            result.Add("character", NickName.characters[word].ToString());
        //            continue;
        //        }
        //        if(!result.ContainsKey("star") && NickName.stars.ContainsKey(word))
        //        {
        //            result.Add("star", NickName.stars[word]);
        //            continue;
        //        }
        //    }
        //    return result;
        //}
        public static Dictionary<string, List<string>> FuzzySearchCharacter(string[] keywords)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (string word in keywords)
            {
                if (NickName.attribute.ContainsKey(word))
                {
                    if (!result.ContainsKey("attribute"))
                    {
                        result.Add("attribute", new List<string>());
                    }
                    result["attribute"].Add(NickName.attribute[word]);
                    continue;
                }
                if (NickName.groups.ContainsKey(word))
                {
                    if (!result.ContainsKey("group"))
                    {
                        result.Add("group", new List<string>());
                    }
                    result["group"].Add(NickName.groups[word]);
                    continue;
                }
                if ( NickName.characters.ContainsKey(word))
                {
                    if (!result.ContainsKey("character"))
                    {
                        result.Add("character", new List<string>());
                    }
                    result["character"].Add(NickName.characters[word].ToString());
                    continue;
                }
                if (NickName.stars.ContainsKey(word))
                {
                    if (!result.ContainsKey("star"))
                    {
                        result.Add("star", new List<string>());
                    }
                    result["star"].Add(NickName.stars[word]);
                    continue;
                }
                if (NickName.cardType.ContainsKey(word))
                {
                    if (!result.ContainsKey("type"))
                    {
                        result.Add("type", new List<string>());
                    }
                    result["type"].Add(NickName.cardType[word]);
                    continue;
                }
            }
            return result;
        }
        public static Dictionary<string,List<string>> FuzzySearchEvent(string[] keywords)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (string word in keywords)
            {
                if (NickName.attribute.ContainsKey(word))
                {
                    if (!result.ContainsKey("attribute"))
                    {
                        result.Add("attribute", new List<string>());
                    }
                    result["attribute"].Add(NickName.attribute[word]);
                    continue;
                }
                if (NickName.groups.ContainsKey(word))
                {
                    if (!result.ContainsKey("group"))
                    {
                        result.Add("group", new List<string>());
                    }
                    result["group"].Add(NickName.groups[word]);
                    continue;
                }
                if (NickName.characters.ContainsKey(word))
                {
                    if (!result.ContainsKey("character"))
                    {
                        result.Add("character", new List<string>());
                    }
                    result["character"].Add(NickName.characters[word].ToString());
                    continue;
                }
                if (NickName.eventType.ContainsKey(word))
                {
                    if (!result.ContainsKey("eventType"))
                    {
                        result.Add("eventType", new List<string>());
                    }
                    result["eventType"].Add(NickName.eventType[word]);
                    continue;
                }
            }
            return result;
        }
    }
}
