using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.Routers
{
    internal static class SearchInfo
    {
        public static async Task SearchCharacter(string messgae)
        {
            string[] keywords = messgae.Split(' ');
            Dictionary<string, string> keys = FuzzySearch.FuzzySearchCharacter(keywords);
            List<SkCard> cards = FindMatchingCards(SkDataBase.skCards, keys);
            
        }
        private static List<SkCard> FindMatchingCards(List<SkCard> cards,Dictionary<string,string> searchConditions)
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
                        query = query.Where(card => card.id == int.Parse(condition.Value));
                        break;
                        // 添加其他条件
                }
            }

            return query.ToList();
        }
        
    }
}
