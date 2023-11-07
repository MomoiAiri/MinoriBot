using MinoriBot.Utils.StaticFilesLoader;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.BufferedDeserialization;

namespace MinoriBot.Enums.Sekai
{
    public class SkGachas
    {
        public int id;
        public string gachaType;
        public string name;
        public int seq;
        public string assetbundleName;
        public int gachaCardRarityRateGroupId;
        public long startAt;
        public long endAt;
        public bool isShowPeriod;
        public int wishSelectCount;
        public int wishFixedSelectCount;
        public int wishLimitedSelectCount;
        public List<GachaCardRarityRates> gachaCardRarityRates;
        public List<GachaDetails> gachaDetails;
        public List<GachaBehaviors> gachaBehaviors;
        public List<GachaPickups> gachaPickups;
        public List<GachaPickupCostumes> gachaPickupCostumes;
        public GachaInfomation gachaInformation;

        public class GachaCardRarityRates
        {
            public int id;
            public int groupId;
            public string cardRarityType;
            public string lotteryType;
            public double rate;
        }
        public class GachaDetails
        {
            public int id;
            public int gachaId;
            public int cardId;
            public int weight;
            public bool isWish;
        }
        public class GachaBehaviors
        {
            public int id;
            public int gachaId;
            public string gachaBehaviorType;
            public string costResourceType;
            public int costResourceQuantity;
            public int spinCount;
            public int executeLimit;
            public int groupId;
            public int priority;
            public string resourceCategory;
            public string gachaSpinnableType;
        }
        public class GachaPickups
        {
            public int id;
            public int gachaId;
            public int cardId;
            public string gachaPickupType;
        }
        public class GachaPickupCostumes
        {

        }
        public class GachaInfomation
        {
            public int gachaId;
            public string summary;
            public string description;
        }

        public async Task<SKBitmap> GetGachaBanner()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = basePath + $"asset/gacha/banner";
            string filePath = fileDirectory + $"/banner_gacha{id}.png";
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
                        byte[] imageBytes;
                        //string type = GetGachaType();
                        //if (type != "生日" || type != "")
                        //{
                        //    imageBytes = await client.GetByteArrayAsync($"https://storage.sekai.best/sekai-assets/home/banner/banner_gacha{id}_rip/banner_gacha{id}.png");
                        //}
                        //else
                        //{
                            imageBytes = await client.GetByteArrayAsync($"https://storage.sekai.best/sekai-assets/gacha/{assetbundleName}/logo_rip/logo.png");
                        //}

                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                            Console.WriteLine($"保存图片banner_gacha{id}成功");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Download Image Failed" + e);
                        return SKBitmap.Decode("./asset/normal/err.png");
                    }
                }
                SKBitmap sKBitmap = SKBitmap.Decode(filePath);
                return sKBitmap;
            }
        }
        /// <summary>
        /// 获取卡池类型（常驻/限定）
        /// </summary>
        /// <returns></returns>
        public string GetGachaPickUpType()
        {
            int left = 0;
            int right = SkDataBase.skGachaCeilItemscs.Count - 1;
            int mid;
            while (left <= right)
            {
                mid = (left + right) / 2;
                if (SkDataBase.skGachaCeilItemscs[mid].gachaId < id)
                {
                    left = mid + 1;
                }
                else if (SkDataBase.skGachaCeilItemscs[mid].gachaId > id)
                {
                    right = mid - 1;
                }
                else
                {
                    if (SkDataBase.skGachaCeilItemscs[mid].assetbundleName == "ceil_item") return "常驻";
                    if (SkDataBase.skGachaCeilItemscs[mid].assetbundleName == "ceil_item_limited") return "限定";
                    if (SkDataBase.skGachaCeilItemscs[mid].assetbundleName == "ceil_item_birthday") return "生日";
                }
            }
            return "none";
        }
        /// <summary>
        /// 获取卡池类型
        /// </summary>
        /// <returns></returns>
        public string GetGachaType()
        {
            switch(gachaType)
            {
                case "ceil":
                    return "天井";
                case "normal":
                    return "一般";
                case "gift":
                    return "礼物";
                case "beginner":
                    return "初心者";
            }
            return "";
        }
        /// <summary>
        /// 获取卡池分布概率
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,double> GetGachaRarityRates()
        {
            Dictionary<string,double> rarityRates = new Dictionary<string,double>();
            if (gachaCardRarityRates.Count > 0)
            {
                for (int i = 0; i < gachaCardRarityRates.Count; i++)
                {
                    rarityRates.Add(gachaCardRarityRates[i].cardRarityType, gachaCardRarityRates[i].rate);
                }
            }
            return rarityRates;
        }
    }
}
