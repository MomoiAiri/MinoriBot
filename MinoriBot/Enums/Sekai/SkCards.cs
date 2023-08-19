using MinoriBot.Utils.StaticFilesLoader;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Enums.Sekai
{
    public class SkCard
    {
        public int id;
        public int seq;
        public int characterId;
        public string cardRarityType;//星级
        public int specialTrainingPower1BonusFixed;//特训后每项数值增长
        public int specialTrainingPower2BonusFixed;
        public int specialTrainingPower3BonusFixed;
        public string attr;
        public string supportUnit;
        public int skillId;
        public string cardSkillName;
        public string prefix;//卡牌名称
        public string assetbundleName;//资源名称
        public string gachaPhrase;
        public string flavorText;
        public long releaseAt;//发布时间 时间戳格式
        public long archivePublishedAt;
        public List<CardParameters> cardParameters = new List<CardParameters>();
        public List<SpecialTrainingCosts> specialTraining = new List<SpecialTrainingCosts>();
        public List<MasterLessonAchieveResources> masterLessonAchieveResources = new List<MasterLessonAchieveResources>();
        public class CardParameters
        {
            public int id;
            public int cardId;
            public int cardLevel;
            public string cardParameterType;
            public int power;
        }
        public class SpecialTrainingCosts
        {
            public int cardId;
            public int seq;
            public Cost cost;
        }
        public class Cost
        {
            public int resourceId;
            public string resourceType;
            public int resourceLevel;
            public int quantity;
        }
        public class MasterLessonAchieveResources
        {
            public int releaseConditionId;
            public int cardId;
            public int masterRank;
            public List<int> resources = new List<int>();
        }
        /// <summary>
        /// 获取卡面Icon
        /// </summary>
        /// <param name="isTrained"></param>
        /// <returns></returns>

        public async Task<SKBitmap> GetCardIcon(bool isTrained)
        {
            string trainingStatus = isTrained ? "after_training" : "normal";
            //三星以下及生日卡只有特训前
            if (GetStarsCount() < 3) trainingStatus = "normal";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = basePath + $"asset/thumbnail/chara_rip";
            string filePath = fileDirectory + $"/{assetbundleName}_{trainingStatus}.png";
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
                        byte[] imageBytes = await client.GetByteArrayAsync($"https://storage.sekai.best/sekai-assets/thumbnail/chara_rip/{assetbundleName}_{trainingStatus}.png");

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
        /// 获取卡牌的星级，0为生日卡
        /// </summary>
        /// <returns></returns>
        public int GetStarsCount()
        {
            switch (cardRarityType)
            {
                case "rarity_1":
                    return 1;
                case "rarity_2":
                    return 2;
                case "rarity_3":
                    return 3;
                case "rarity_4":
                    return 4;
                case "rarity_birthday":
                    return 0;
            }
            return -1;
        }
        /// <summary>
        /// 根据成员ID返回团队名
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public string GetGroupNameById()
        {
            if (characterId >= 1 && characterId <= 4) return "ln";
            if (characterId >= 5 && characterId <= 8) return "mmj";
            if (characterId >= 9 && characterId <= 12) return "vbs";
            if (characterId >= 13 && characterId <= 16) return "ws";
            if (characterId >= 17 && characterId <= 20) return "25";
            if (characterId >= 21 && characterId <= 26) return "vs";
            return "";
        }
        public string GetSkillDescription()
        {
            foreach(SkSkills skill in SkDataBase.skSkills)
            {
                if(skillId == skill.id)
                {
                    string description = skill.description;
                    
                }
            }
            return "";
        }

    }
    
}
