using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using MinoriBot.Utils.View;
using SkiaSharp;
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
        public static async Task<List<MessageObj>> SearchCharacter(string message)
        {
            List<SkCard> allCards = SkDataBase.skCards;
            List<MessageObj> result = new List<MessageObj>();
            if (int.TryParse(message, out int cardId))
            {
                if (cardId > 1)
                {
                    bool isFound = false;
                    for (int i = 0; i < SkDataBase.skCards.Count; i++)
                    {
                        if (SkDataBase.skCards[i].id == cardId)
                        {
                            isFound = true;
                            result.Add(await CardDetail.DrawCardDetail(SkDataBase.skCards[i]));
                            return result;
                        }
                    }
                    if (!isFound)
                    {
                        result.Add(new MessageObj() { type = "string", content = "没有查找到该ID的卡牌" });
                        return result;
                    }
                }
            }
            string[] keywords = message.Split(' ');
            List<SkCard>? searchCardsList = null;
            bool isSifted = false;
            //检查关键词中是否有"-"的符号
            for (int i = 0; i < keywords.Length; i++)
            {
                if (keywords[i].Contains('-'))
                {
                    if (!isSifted)
                    {
                        searchCardsList = SiftByCardID(keywords[i]);
                        if (searchCardsList == null)
                        {
                            result.Add(new MessageObj() { type = "string", content = "筛选关键词不合法" });
                            return result;
                        }
                        isSifted = true;
                    }
                    else
                    {
                        result.Add(new MessageObj() { type = "string", content = "筛选关键词只能有一条" });
                        return result;
                    }
                }
                
            }
            bool trainingState = true;
            if (keywords.Length == 1 && searchCardsList != null)
            {
                MessageObj pic = await CardList.DrawCardListImage(searchCardsList, trainingState);
                result.Add(pic);
                return result;
            }
            //将模糊的关键词转换成唯一的关键词
            Dictionary<string, List<string>> keys = FuzzySearch.FuzzySearchCharacter(keywords);
            if (keys.Count == 0)
            {
                result.Add(new MessageObj() { type = "string", content = "关键词有误" });
                return result;
            }
            List<SkCard> cards = FindMatchingCards(searchCardsList == null? allCards:searchCardsList, keys);
            Console.WriteLine($"查询到{cards.Count}个结果");
            if (cards.Count == 0)
            {
                result.Add(new MessageObj() { type = "string", content = "没有查找到相关卡牌" });
                return result;
            }
            if (keys.ContainsKey("train")&& keys["train"].Count==1)
            {
                if (keys["train"][0] == "False")
                {
                    trainingState = false;
                }
            }
            MessageObj file = await CardList.DrawCardListImage(cards, trainingState);
            result.Add(file);
            return result;
        }
        public static async Task<List<MessageObj>> GetCardIllustrationImage(string message)
        {
            List<MessageObj> result = new List<MessageObj>();
            if (int.TryParse(message, out int cardId))
            {
                bool isFound = false;
                for (int i = 0; i < SkDataBase.skCards.Count; i++)
                {
                    if (SkDataBase.skCards[i].id == cardId)
                    {
                        isFound = true;
                        SkCard card = SkDataBase.skCards[i];
                        result.Add(new MessageObj { type = "image", content = ImageCreater.ConvertBitmapToBase64(await card.GetCardIllustrationImage(false)) });
                        if (card.GetStarsCount() > 2)
                        {
                            result.Add(new MessageObj { type = "image", content = ImageCreater.ConvertBitmapToBase64(await card.GetCardIllustrationImage(true)) });
                        }
                    }
                }
                if (!isFound)
                {
                    return null;
                }
            }
            return result;
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

        private static List<SkCard> FindMatchingCards(List<SkCard> cards, Dictionary<string, List<string>> searchConditions)
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
                    case "group:group":
                        List<string> group1 = new List<string>();
                        List<string> group2 = new List<string>();
                        for(int i = 0; i < condition.Value.Count; i++)
                        {
                            string[] items = condition.Value[0].Split(':');
                            group1.Add(items[0]);
                            group2.Add(items[1]);
                        }
                        query = query.Where(card => group1.Contains(card.supportUnit));
                        break;
                    case "group:character":
                        List<string> group3 = new List<string>();
                        List<string> character1 = new List<string>();
                        for (int i = 0; i < condition.Value.Count; i++)
                        {
                            string[] items = condition.Value[0].Split(':');
                            group3.Add(items[0]);
                            character1.Add(items[1]);
                        }
                        query = query.Where(card => character1.Contains(card.characterId.ToString()) && group3.Contains(card.supportUnit));
                        break;
                    case "group+group":
                        List<string> group4 = new List<string>();
                        List<string> group5 = new List<string>();
                        for (int i = 0; i < condition.Value.Count; i++)
                        {
                            string[] items = condition.Value[0].Split(':');
                            group4.Add(items[0]);
                            group5.Add(items[1]);
                        }
                        query = query.Where(card => group4.Contains(card.GetGroupNameById()) || group4.Contains(card.supportUnit));
                        break;
                    case "group+character":
                        List<string> group6 = new List<string>();
                        List<string> character2 = new List<string>();
                        for (int i = 0; i < condition.Value.Count; i++)
                        {
                            string[] items = condition.Value[0].Split(':');
                            group6.Add(items[0]);
                            character2.Add(items[1]);
                        }
                        query = query.Where(card => group6.Contains(card.GetGroupNameById()) || (character2.Contains(card.characterId.ToString()) && group6.Contains(card.supportUnit)));
                        break;
                }
            }

            return query.ToList();

        }
        private static List<SkCard>? SiftByCardID(string keywords)
        {
            List<SkCard> cards = SkDataBase.skCards;
            string[] range = keywords.Split('-',StringSplitOptions.RemoveEmptyEntries);
            if(range.Length == 0) return null;
            int[] irange = new int[range.Length];
            //判断参数是否合法
            for(int i =0;i<range.Length;i++)
            {
                if (int.TryParse(range[i], out int value))
                {
                    irange[i] = value;
                }
                else
                {
                    return null;
                }
            }
            if (irange.Length == 1)
            {
                if (keywords.StartsWith("-"))
                {
                    return cards.Where(card => card.id <= irange[0]).ToList();
                }
                else if (keywords.StartsWith(range[0]))
                {
                    return cards.Where(card => card.id >= irange[0]).ToList();
                }
            }
            else if(irange.Length == 2)
            {
                return cards.Where(card => card.id >= irange[0]&&card.id <= irange[1]).ToList();
            }
            return null;
        }
    }
}
