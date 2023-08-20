using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.Routers
{
    internal static class SearchInfo
    {
        public static async Task<string> SearchCharacter(string messgae)
        {
            string[] keywords = messgae.Split(' ');
            //将模糊的关键词转换成唯一的关键词
            Dictionary<string, string> keys = FuzzySearch.FuzzySearchCharacter(keywords);
            foreach(KeyValuePair<string,string> dic in keys)
            {
                Console.WriteLine($"{dic.Key}:{dic.Value}");
            }
            List<SkCard> cards = await FindMatchingCards(SkDataBase.skCards, keys);
            Console.WriteLine($"查询到{cards.Count}个结果");
            //StringBuilder sb = new StringBuilder();
            //foreach (SkCard card in cards)
            //{
            //    sb.Append(card.prefix + "\n");
            //}
            ImageCreater imageCreater = new ImageCreater();
            string file = await imageCreater.DrawCardIconLine(cards,true);
            return file;
        }
        private static async Task<List<SkCard>> FindMatchingCards(List<SkCard> cards,Dictionary<string,string> searchConditions)
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
                        // 添加其他条件
                }
            }

            return query.ToList();
        }
        
    }
}
