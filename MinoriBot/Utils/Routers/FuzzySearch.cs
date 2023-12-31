﻿using MinoriBot.Enums.Sekai;
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
                if (word.Contains(":")||word.Contains("："))
                {
                    string[] items = word.Split(':','：');
                    
                    if(items.All(i => i != "")&&items.Length==2)
                    {
                        if (NickName.groups.ContainsKey(items[0]) && NickName.groups[items[0]]!= "piapro")
                        {
                            if (NickName.groups.ContainsKey(items[1]) && NickName.groups[items[1]] == "piapro")
                            {
                                if (!result.ContainsKey("group:group"))
                                {
                                    result.Add("group:group", new List<string>());
                                }
                                result["group:group"].Add($"{NickName.groups[items[0]]}:{NickName.groups[items[1]]}");
                            }
                            else if (NickName.characters.ContainsKey(items[1]) && NickName.characters[items[1]] > 20 && NickName.characters[items[1]] < 27)
                            {
                                if (!result.ContainsKey("group:character"))
                                {
                                    result.Add("group:character", new List<string>());
                                }
                                result["group:character"].Add($"{NickName.groups[items[0]]}:{NickName.characters[items[1]]}");
                            }
                        }
                    }
                    continue;
                }
                if (word.Contains("+"))
                {
                    string[] items = word.Split('+');

                    if (items.All(i => i != "") && items.Length == 2)
                    {
                        if (NickName.groups.ContainsKey(items[0]) && NickName.groups[items[0]] != "piapro")
                        {
                            if (NickName.groups.ContainsKey(items[1]) && NickName.groups[items[1]] == "piapro")
                            {
                                if (!result.ContainsKey("group+group"))
                                {
                                    result.Add("group+group", new List<string>());
                                }
                                result["group+group"].Add($"{NickName.groups[items[0]]}:{NickName.groups[items[1]]}");
                            }
                            else if (NickName.characters.ContainsKey(items[1]) && NickName.characters[items[1]] > 20 && NickName.characters[items[1]] < 27)
                            {
                                if (!result.ContainsKey("group+character"))
                                {
                                    result.Add("group+character", new List<string>());
                                }
                                result["group+character"].Add($"{NickName.groups[items[0]]}:{NickName.characters[items[1]]}");
                            }
                        }
                    }
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
                if (NickName.trainingState.ContainsKey(word))
                {
                    if (!result.ContainsKey("train"))
                    {
                        result.Add("train", new List<string>());
                    }
                    result["train"].Add(NickName.trainingState[word].ToString());
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
        public static Dictionary<string, List<string>> FuzzySearchMusic(string[] keywords)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (string word in keywords)
            {
                if (NickName.diffType.ContainsKey(word))
                {
                    if (!result.ContainsKey("diffType"))
                    {
                        result.Add("diffType", new List<string>());
                    }
                    result["diffType"].Add(NickName.diffType[word]);
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
                if (word.Contains("lv") || word.Contains("难度") || word.Contains("等级"))
                {
                    if (!result.ContainsKey("diffLevel"))
                    {
                        result.Add("diffLevel", new List<string>());
                    }
                    result["diffLevel"].Add(word.Substring(2));
                    continue;
                }
                else
                {
                    if (!result.ContainsKey("musicName"))
                    {
                        result.Add("musicName", new List<string>());
                    }
                    result["musicName"].Add(word);
                    continue;
                }
            }
            return result;
        }
    }
}
