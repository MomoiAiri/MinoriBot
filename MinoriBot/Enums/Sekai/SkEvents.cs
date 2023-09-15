using MinoriBot.Utils.StaticFilesLoader;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Enums.Sekai
{
    public class SkEvents
    {
        public int id;
        public string eventType;
        public string name;
        public string assetbundleName;
        public string bgmAssetbundleName;
        public string eventPointAssetbundleName;
        public long startAt;//开始时间
        public long aggregateAt;//活动结束时间
        public long rankingAnnounceAt;//排名宣布时间
        public long distributionStartAt;//奖励发放时间
        public long closedAt;//下次活动时间
        public long distributionEndAt;
        public int virtualLiveId;
        public string unit;
        public List<EventRankingRewardRanges> eventRankingRewardRanges;
        public class EventRankingRewardRanges
        {
            public int id;
            public int eventId;
            public int fromRank;
            public int toRank;
            public List<EventRankingRewards> eventRankingRewards;
            public class EventRankingRewards
            {
                public int id;
                public int eventRankingRewardRangeId;
                public int resourceBoxId;
            }
        }
        /// <summary>
        /// 获取活动banner
        /// </summary>
        /// <returns></returns>
        public async Task<SKBitmap> GetEventBanner()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = basePath + $"asset/event/banner";
            string filePath = fileDirectory + $"/{assetbundleName}.png";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            if (File.Exists(filePath))
            {
                SKBitmap sKBitmap = SKBitmap.Decode(filePath);
                return sKBitmap;
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        byte[] imageBytes = await client.GetByteArrayAsync($"https://storage.sekai.best/sekai-assets/home/banner/{assetbundleName}_rip/{assetbundleName}.png");

                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                            Console.WriteLine($"保存图片{assetbundleName}成功");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Download Image Failed" + e);
                        return null;
                    }
                }
                SKBitmap sKBitmap = SKBitmap.Decode(filePath);
                return sKBitmap;
            }
        }
        public async Task<SKBitmap> GetEventDegreeMain()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string assetName = GetHonorAssetName();
            string fileDirectory = basePath + $"asset/event/degree/{assetName}";
            string filePath = fileDirectory + "/degree_main.png";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            if (File.Exists(filePath))
            {
                SKBitmap sKBitmap = SKBitmap.Decode(filePath);
                return sKBitmap;
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        byte[] imageBytes = await client.GetByteArrayAsync($"https://storage.sekai.best/sekai-assets/honor/{assetName}/degree_main.png");

                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                            Console.WriteLine($"保存图片{assetName}成功");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Download Image Failed" + e);
                        return null;
                    }
                }
                SKBitmap sKBitmap = SKBitmap.Decode(filePath);
                return sKBitmap;
            }
        }
        /// <summary>
        /// 获取当期加成角色
        /// </summary>
        /// <returns></returns>
        public List<int> GetBunusCharacters()
        {
            List<int> bunusCharacter = new List<int>();
            for(int i = 0; i < SkDataBase.skEventDeckBonuses.Count; i++)
            {
                if (id == SkDataBase.skEventDeckBonuses[i].eventId && SkDataBase.skEventDeckBonuses[i].cardAttr == null)
                {
                    int charcterid = CharacterUnitIdToCharacterId(SkDataBase.skEventDeckBonuses[i].gameCharacterUnitId);
                    if (charcterid > 0)
                    {
                        bunusCharacter.Add(charcterid);
                    }
                }
            }
            return bunusCharacter;
        }
        public double GetBunusCharacterRate()
        {
            for (int i = 0; i < SkDataBase.skEventDeckBonuses.Count; i++)
            {
                if (id == SkDataBase.skEventDeckBonuses[i].eventId && SkDataBase.skEventDeckBonuses[i].cardAttr == null)
                {
                    return SkDataBase.skEventDeckBonuses[i].bonusRate;
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取当期加成颜色
        /// </summary>
        /// <returns></returns>
        public string GetBunusAttr()
        {
            for (int i = 0; i < SkDataBase.skEventDeckBonuses.Count; i++)
            {
                if (id == SkDataBase.skEventDeckBonuses[i].eventId&& SkDataBase.skEventDeckBonuses[i].cardAttr!=null)
                {
                    return SkDataBase.skEventDeckBonuses[i].cardAttr;
                }
            }
            return "";
        }
        public double GetBunusAttRate()
        {
            for (int i = 0; i < SkDataBase.skEventDeckBonuses.Count; i++)
            {
                if (id == SkDataBase.skEventDeckBonuses[i].eventId && SkDataBase.skEventDeckBonuses[i].cardAttr != null&& SkDataBase.skEventDeckBonuses[i].gameCharacterUnitId==0)
                {
                    return SkDataBase.skEventDeckBonuses[i].bonusRate;
                }
            }
            return 0;
        }
        public int CharacterUnitIdToCharacterId(int characterUnitId)
        {
            for (int i = 0; i < SkDataBase.skGameCharacterUnits.Count; i++)
            {
                if(characterUnitId== SkDataBase.skGameCharacterUnits[i].id)
                {
                    return SkDataBase.skGameCharacterUnits[i].gameCharacterId;
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取活动团队种类（厢活/混活）
        /// </summary>
        /// <returns></returns>
        public string GetEventGroupType()
        {
            List<int> bunusCharacters = GetBunusCharacters();
            List<int> compareList = new List<int>() {1,2,3,4 };
            if (compareList.All(item => bunusCharacters.Contains(item)))
            {
                return "ln";
            }
            compareList = new List<int>() { 5,6,7,8 };
            if (compareList.All(item => bunusCharacters.Contains(item)))
            {
                return "mmj";
            }
            compareList = new List<int>() { 9,10,11,12};
            if (compareList.All(item => bunusCharacters.Contains(item)))
            {
                return "vbs";
            }
            compareList = new List<int>() { 13,14,15,16 };
            if (compareList.All(item => bunusCharacters.Contains(item)))
            {
                return "ws";
            }
            compareList = new List<int>() { 17,18,19,20 };
            if (compareList.All(item => bunusCharacters.Contains(item)))
            {
                return "25";
            }
            compareList = new List<int>() { 21,22,23,24,25,26 };
            if (compareList.All(item => bunusCharacters.Contains(item))&&bunusCharacters.Count==6)
            {
                return "vs";
            }
            return "mix";
        }
        /// <summary>
        /// 获取当期卡牌
        /// </summary>
        /// <returns></returns>
        public List<SkCard> GetCurrentCards()
        {
            List<SkCard> cards = new List<SkCard>();
            List<int> cardIds = new List<int>();
            for(int i = 0; i < SkDataBase.skEventCards.Count; i++)
            {
                if (SkDataBase.skEventCards[i].eventId == id)
                {
                    cardIds.Add(SkDataBase.skEventCards[i].cardId);
                }
            }
            cards = SkDataBase.skCards.Where(card => cardIds.Contains(card.id)).ToList();
            return cards;
        }
        public string GetEventType()
        {
            return eventType=="marathon"?"协力":"5v5";
        }
        string GetHonorAssetName()
        {
            for(int i =0;i<SkDataBase.skHonorGroups.Count;i++)
            {
                if(name == SkDataBase.skHonorGroups[i].name)
                {
                    return SkDataBase.skHonorGroups[i].backgroundAssetbundleName + "_rip";
                }
            }
            return "";
        }
    }
}
