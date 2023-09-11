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
        /// 获取活动logo
        /// </summary>
        /// <returns></returns>
        public async Task<SKBitmap> GetEventLogo()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = basePath + $"asset/event";
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
                        byte[] imageBytes = await client.GetByteArrayAsync($"https://storage.sekai.best/home/banner/{assetbundleName}_rip/{assetbundleName}.png");

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
        /// <summary>
        /// 获取当期加成角色
        /// </summary>
        /// <returns></returns>
        public List<int> GetBunusCharacters()
        {
            List<int> bunusCharacter = new List<int>();
            for(int i = 0; i < SkDataBase.skEventDeckBonuses.Count; i++)
            {
                if(id == SkDataBase.skEventDeckBonuses[i].eventId)
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
        /// <summary>
        /// 获取当期加成颜色
        /// </summary>
        /// <returns></returns>
        public string GetBunusAttr()
        {
            for (int i = 0; i < SkDataBase.skEventDeckBonuses.Count; i++)
            {
                if (id == SkDataBase.skEventDeckBonuses[i].eventId&& SkDataBase.skEventDeckBonuses[i].cardAttr!="")
                {
                    return SkDataBase.skEventDeckBonuses[i].cardAttr;
                }
            }
            return "";
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
    }
}
