using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.View
{
    public static class CardList
    {
        public static async Task<MessageObj> DrawCardListImage(List<SkCard> cards,bool isTrained)
        {
            Console.WriteLine($"正在生成卡面列表");
            Dictionary<int, Dictionary<string, List<SkCard>>> classifiedCards = SortDictionary(ClassifiedCards(cards));

            List<SKBitmap> all = new List<SKBitmap>();

            //图片标题
            SKBitmap title = ImageCreater.DrawImageTitle("查询", "卡牌");
            all.Add(title);

            //卡牌列表
            SKBitmap cardList = await ImageCreater.DrawCardIconList(classifiedCards, isTrained);
            all.Add(ImageCreater.DrawInfoBlock(new List<SKBitmap>() { cardList }, cardList.Width + 100));

            SKBitmap final = ImageCreater.DrawALL(all,cardList.Width + 200);
            Console.WriteLine("卡面列表生成完毕");

            MessageObj ml = new MessageObj() { type = "image", content = ImageCreater.ConvertBitmapToBase64(final) };

            return ml;
        }
        /// <summary>
        /// 对字典中的内容根据角色，属性进行排序
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static Dictionary<int, Dictionary<string, List<SkCard>>> SortDictionary(Dictionary<int, Dictionary<string, List<SkCard>>> cards)
        {
            Dictionary<int, Dictionary<string, List<SkCard>>> result = new Dictionary<int, Dictionary<string, List<SkCard>>>();
            List<int> sortKeys = cards.Keys.ToList();
            sortKeys.Sort();
            foreach (int key in sortKeys)
            {
                result.Add(key, new Dictionary<string, List<SkCard>>());
                List<string> attrList = cards[key].Keys.ToList();
                attrList.Sort((attr1, attr2) => GetAttrId(attr1).CompareTo(GetAttrId(attr2)));
                foreach (string strkey in attrList)
                {
                    result[key].Add(strkey, cards[key][strkey]);
                }

            }
            return result;
        }
        /// <summary>
        /// 将卡牌列表转换为 角色：卡牌列表 的字典
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static Dictionary<int, Dictionary<string, List<SkCard>>> ClassifiedCards(List<SkCard> cards)
        {
            Dictionary<int, Dictionary<string, List<SkCard>>> classifiedCards = new Dictionary<int, Dictionary<string, List<SkCard>>>();
            foreach (SkCard card in cards)
            {
                if (!classifiedCards.ContainsKey(card.characterId))
                {
                    classifiedCards[card.characterId] = new Dictionary<string, List<SkCard>>();
                }
                if (!classifiedCards[card.characterId].ContainsKey(card.attr))
                {
                    classifiedCards[card.characterId][card.attr] = new List<SkCard>();
                }
                classifiedCards[card.characterId][card.attr].Add(card);
            }
            return classifiedCards;
        }
        public static int GetAttrId(string attr)
        {
            if (attr == "cool") return 1;
            if (attr == "cute") return 2;
            if (attr == "happy") return 3;
            if (attr == "mysterious") return 4;
            if (attr == "pure") return 5;
            return 0;
        }
    }
}
