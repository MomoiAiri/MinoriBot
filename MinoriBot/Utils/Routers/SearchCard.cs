using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using MinoriBot.Utils.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.Routers
{
    internal static class SearchCard
    {
        public static async Task<string> SearchCharacter(string message)
        {
            if (int.TryParse(message, out int cardId))
            {
                bool isFound = false;
                for (int i = 0; i < SkDataBase.skCards.Count; i++)
                {
                    if (SkDataBase.skCards[i].id == cardId)
                    {
                        isFound = true;

                        return await CardDetail.DrawCardDetail(SkDataBase.skCards[i]);
                    }
                }
                if (!isFound)
                {
                    return "error";
                }
            }
            string[] keywords = message.Split(' ');
            //将模糊的关键词转换成唯一的关键词
            Dictionary<string, List<string>> keys = FuzzySearch.FuzzySearchCharacter(keywords);
            if (keys.Count == 0)
            {
                return "error";
            }
            //foreach (KeyValuePair<string, string> dic in keys)
            //{
            //    Console.WriteLine($"{dic.Key}:{dic.Value}");
            //}
            List<SkCard> cards = await FindMatchingCards(SkDataBase.skCards, keys);
            Console.WriteLine($"查询到{cards.Count}个结果");
            if(cards.Count == 0) return "none";
            string file = await ImageCreater.DrawCardIconList(cards, true);
            return file;
        }
        private static async Task<List<SkCard>> FindMatchingCards(List<SkCard> cards, Dictionary<string, string> searchConditions)
        {

            var query = cards.AsQueryable();

            foreach (var condition in searchConditions)
            {
                switch (condition.Key)
                {
                    case "attribute":
                        query = query.Where(card => card.attr == condition.Value);
                        break;
                    case "group":
                        query = query.Where(card => card.GetGroupNameById() == condition.Value);
                        break;
                    case "character":
                        query = query.Where(card => card.characterId == int.Parse(condition.Value));
                        break;
                    case "star":
                        query = query.Where(card => card.cardRarityType == condition.Value);
                        break;
                }
            }

            return query.ToList();
        }

        private static async Task<List<SkCard>> FindMatchingCards(List<SkCard> cards, Dictionary<string, List<string>> searchConditions)
        {

            var query = cards.AsQueryable();

            foreach (var condition in searchConditions)
            {
                switch (condition.Key)
                {
                    case "attribute":
                        query = query.Where(card => condition.Value.Contains(card.attr));
                        break;
                    case "group":
                        query = query.Where(card => condition.Value.Contains(card.GetGroupNameById()));
                        break;
                    case "character":
                        query = query.Where(card => condition.Value.Contains(card.characterId.ToString()));
                        break;
                    case "star":
                        query = query.Where(card => condition.Value.Contains(card.cardRarityType));
                        break;
                    case "type":
                        query = query.Where(card => condition.Value.Contains(card.GetCardLimitType()));
                        break;
                }
            }

            return query.ToList();

        }
    }
}
