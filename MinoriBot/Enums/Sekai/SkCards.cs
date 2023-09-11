using MinoriBot.Utils.StaticFilesLoader;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        /// 获取卡面插图
        /// </summary>
        /// <returns></returns>
        public async Task<List<SKBitmap>> GetCardIllustrationImage()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = basePath + $"asset/character/member/{assetbundleName}_rip";
            string filePath1 = fileDirectory + "/card_normal.png";
            string filePath2 = fileDirectory + "/card_after_training.png";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            List<SKBitmap> sKBitmap = new List<SKBitmap>();
            if (File.Exists(filePath1))
            {
                sKBitmap.Add(SKBitmap.Decode(filePath1));
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        byte[] imageBytes = await client.GetByteArrayAsync($"https://storage.sekai.best/sekai-assets/character/member/{assetbundleName}_rip/card_normal.png");

                        using (FileStream fileStream = new FileStream(filePath1, FileMode.Create))
                        {
                            await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                            Console.WriteLine($"保存图片{assetbundleName}_normal成功");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Download Image Failed" + e);
                        return null;
                    }
                }
                sKBitmap.Add(SKBitmap.Decode(filePath1));
            }
            if (GetStarsCount() >= 3)
            {
                if (File.Exists(filePath2))
                {
                    sKBitmap.Add(SKBitmap.Decode(filePath2));
                }
                else
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            byte[] imageBytes = await client.GetByteArrayAsync($"https://storage.sekai.best/sekai-assets/character/member/{assetbundleName}_rip/card_after_training.png");

                            using (FileStream fileStream = new FileStream(filePath2, FileMode.Create))
                            {
                                await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                                Console.WriteLine($"保存图片{assetbundleName}_after_training成功");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Download Image Failed" + e);
                            return null;
                        }
                    }
                    sKBitmap.Add(SKBitmap.Decode(filePath2));
                }
            }
            return sKBitmap;
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
        public int GetMaxLevel()
        {
            switch (cardRarityType)
            {
                case "rarity_1":
                    return 20;
                case "rarity_2":
                    return 30;
                case "rarity_3":
                    return 50;
                case "rarity_4":
                    return 60;
                case "rarity_birthday":
                    return 60;
            }
            return -1;
        }
        public string GetSkillDescription()
        {
            string description = string.Empty;
            foreach(SkSkills skill in SkDataBase.skSkills)
            {
                if(skillId == skill.id)
                {
                    description = skill.description;
                    MatchCollection matches = Regex.Matches(description, @"\{\{([^{}]+)\}\}");
                    foreach(Match match in matches)
                    {
                        string[] code = match.Groups[1].Value.Split(';');
                        int id = int.Parse(code[0]);
                        string parameter = code[1];
                        int index = 0;
                        string replaceStr = string.Empty;
                        for (int i = 0; i < skill.skillEffects.Count; i++)
                        {
                            if (skill.skillEffects[i].id == id)
                            {
                                index = i;
                                break;
                            }
                        }
                        switch(parameter)
                        {
                            case "d":
                                for(int i =0;i< skill.skillEffects[index].skillEffectDetails.Count; i++)
                                {
                                    replaceStr += skill.skillEffects[index].skillEffectDetails[i].activateEffectDuration.ToString();
                                    if(i< skill.skillEffects[index].skillEffectDetails.Count - 1)
                                    {
                                        replaceStr += "/";
                                    }
                                }
                                //replaceStr = skill.skillEffects[index].skillEffectDetails[skill.skillEffects[index].skillEffectDetails.Count - 1].activateEffectDuration.ToString();
                                break;
                            case "v":
                                for (int i = 0; i < skill.skillEffects[index].skillEffectDetails.Count; i++)
                                {
                                    replaceStr += skill.skillEffects[index].skillEffectDetails[i].activateEffectValue.ToString();
                                    if (i < skill.skillEffects[index].skillEffectDetails.Count - 1)
                                    {
                                        replaceStr += "/";
                                    }
                                }
                                //replaceStr = skill.skillEffects[index].skillEffectDetails[skill.skillEffects[index].skillEffectDetails.Count - 1].activateEffectValue.ToString();
                                break;
                            case "e":
                                replaceStr = skill.skillEffects[index].skillEnhance.activateEffectValue.ToString();
                                break;
                            case "m":
                                replaceStr = "150";
                                break;
                        }
                        description = description.Replace($"{match.Value}", replaceStr);
                        description = description.Replace("  ", " ");
                    }
                    break;
                }
            }
            return description;
        }
        /// <summary>
        /// 获取综合力
        /// </summary>
        /// <returns></returns>
        public int[] GetPower()
        {
            //四星 小故事 （250+600）*3 突破200*3*5
            //三星 小故事 （200 + 500）*3 突破150 * 3 * 5
            //二星 小故事 （150 + 300）*3 突破100 * 3 * 5
            //一星 小故事 （100 + 200）*3 突破50 * 3 * 5
            //生日 小故事 （240 + 550）*3 突破180 * 3 * 5
            int levelCount = GetMaxLevel();
            int[] power = new int[]
            { 
                cardParameters[levelCount - 1].power + specialTrainingPower1BonusFixed,
                cardParameters[levelCount * 2 - 1].power + specialTrainingPower2BonusFixed, 
                cardParameters[levelCount * 3 - 1].power + specialTrainingPower3BonusFixed,
                0
            };
            int starCount = GetStarsCount();
            //增加小故事的综合力提升 数组最后一位是每种属性突破一级的提升
            switch (starCount)
            {
                case 0:
                    for (int i = 0; i < power.Length - 1; i++)
                    {
                        power[i] += 240 + 550;
                    }
                    power[3] = 180;
                    break;
                case 1:
                    for (int i = 0; i < power.Length - 1; i++)
                    {
                        power[i] += 100 + 200;
                    }
                    power[3] = 50;
                    break;
                case 2:
                    for (int i = 0; i < power.Length - 1; i++)
                    {
                        power[i] += 150 + 300;
                    }
                    power[3] = 100;
                    break;
                case 3:
                    for (int i = 0; i < power.Length - 1; i++)
                    {
                        power[i] += 200 + 500;
                    }
                    power[3] = 150;
                    break;
                case 4:
                    for (int i = 0; i < power.Length - 1; i++)
                    {
                        power[i] += 250 + 600;
                    }
                    power[3] = 200;
                    break;
            }
            return power;
        }
        /// <summary>
        /// 获取卡牌活动出处，返回值为null表示开服卡
        /// </summary>
        /// <returns></returns>
        public SkEvents GetEvent()
        {
            int eventId = 0;
            for (int i = 0; i < SkDataBase.skEventCards.Count; i++)
            {
                if(id == SkDataBase.skEventCards[i].cardId)
                {
                    eventId = SkDataBase.skEventCards[i].cardId;
                    break;
                }
            }
            if (eventId != 0)
            {
                for(int i = 0; i < SkDataBase.skEvents.Count; i++)
                {
                    if(eventId == SkDataBase.skEvents[i].id)
                    {
                        return SkDataBase.skEvents[i];
                    }
                }
            }
            return null;
        }
    }
    
}
