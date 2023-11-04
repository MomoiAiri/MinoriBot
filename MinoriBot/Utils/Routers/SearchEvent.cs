using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using MinoriBot.Utils.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.Routers
{
    internal static class SearchEvent
    {
        public static async Task<List<MessageObj>> SearchSkEvents(string message)
        {
            List<MessageObj> result = new List<MessageObj>();
            if (int.TryParse(message, out int eventId))
            {
                bool isFound = false;
                for (int i = 0; i < SkDataBase.skEvents.Count; i++)
                {
                    if (SkDataBase.skEvents[i].id == eventId)
                    {
                        isFound = true;
                        result.Add(await EventDetail.DrawEventDetail(SkDataBase.skEvents[i]));
                        return result;
                    }
                }
                if (!isFound)
                {
                    result.Add(new MessageObj() { type = "string", content = "没有查找到该ID的活动" });
                    return result;
                }
            }
            string[] keywords = message.Split(' ');
            //将模糊的关键词转换成唯一的关键词
            Dictionary<string, List<string>> keys = FuzzySearch.FuzzySearchEvent(keywords);
            if (keys.Count == 0)
            {
                result.Add(new MessageObj() { type = "string", content = "关键词有误" });
                return result;
            }
            List<SkEvents> skEvents = await GetMatchingEvents(SkDataBase.skEvents, keys);
            if (skEvents.Count == 0)
            {
                result.Add(new MessageObj() { type = "string", content = "没有查找到相关活动" });
                return result;
            }
            MessageObj file = await EventList.DrawEventList(skEvents);
            result.Add(file);
            return result;
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
