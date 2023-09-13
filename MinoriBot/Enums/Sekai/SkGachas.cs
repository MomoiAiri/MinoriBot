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
        public GachaInfomation gachaInfomation;

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
            public int gachId;
            public string summary;
            public string description;
        }
    }
}
