using System;
using System.Collections.Generic;
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
    }
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
}
