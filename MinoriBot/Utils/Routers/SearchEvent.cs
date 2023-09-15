﻿using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.Routers
{
    internal static class SearchEvent
    {
        public static async Task<string> SearchSkEvents(string message)
        {
            if (int.TryParse(message, out int eventId))
            {
                bool isFound = false;
                for (int i = 0; i < SkDataBase.skEvents.Count; i++)
                {
                    if (SkDataBase.skEvents[i].id == eventId)
                    {
                        isFound = true;
                        ImageCreater creater = new ImageCreater();
                        return await creater.DrawEventInfo(SkDataBase.skEvents[i]);
                    }
                }
                if (!isFound)
                {
                    return "error";
                }
            }
            string[] keywords = message.Split(' ');
            //将模糊的关键词转换成唯一的关键词
            Dictionary<string, List<string>> keys = FuzzySearch.FuzzySearchEvent(keywords);
            if (keys.Count == 0)
            {
                return "error";
            }
            //打印一下转换后的关键词
            //StringBuilder matchwords = new StringBuilder();
            //foreach(var kvp in keys)
            //{
            //    matchwords.Append(kvp.Key+":");
            //    foreach(string s in kvp.Value)
            //    {
            //        matchwords.Append(s + " ");
            //    }
            //    matchwords.Append("\n");
            //}
            //Console.WriteLine(matchwords.ToString());

            List<SkEvents> skEvents = await GetMatchingEvents(SkDataBase.skEvents, keys);
            //StringBuilder sb = new StringBuilder();
            //foreach(SkEvents e in skEvents)
            //{
            //    sb.Append(e.id +":"+ e.name + "\n");
            //}
            ImageCreater imageCreater = new ImageCreater();
            string file = await imageCreater.DrawEventList(skEvents);
            return file;
        }
        static async Task<List<SkEvents>> GetMatchingEvents(List<SkEvents> skEvents, Dictionary<string, List<string>> searchConditions)
        {
            var query = skEvents.AsQueryable();

            foreach (var condition in searchConditions)
            {
                switch (condition.Key)
                {
                    case "attribute":
                        query = query.Where(skEvent => condition.Value.Contains(skEvent.GetBunusAttr()));
                        break;
                    case "group":
                        query = query.Where(skEvent => condition.Value.Contains(skEvent.GetEventGroupType()));
                        break;
                    case "character":
                        query = query.Where(skEvent => condition.Value.All(item => skEvent.GetBunusCharacters().Contains(int.Parse(item))));
                        break;
                    case "eventType":
                        query = query.Where(skEvent => condition.Value.Contains(skEvent.eventType));
                        break;
                }
            }

            return query.ToList();
        }
    }
}
