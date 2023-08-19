using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Enums.Sekai
{
    public class SkSkills
    {
        public int id;
        public string shortDescription;
        public string description;
        public string descriptionSpriteName;
        public int skillFilterId;
        public List<SkillEffects> skillEffects;

        public class SkillEffects
        {
            public int id;
            public string skillEffectType;
            public string activateNotesJudgmentType;
            public List<SkillEffectDetails> skillEffectDetails;

            public class SkillEffectDetails
            {
                public int id;
                public int level;//技能等级
                public double activateEffectDuration;//技能持续时间
                public string activateEffectValueType;//判断类型
                public int activateEffectValue;//加成数值
            }
        }
    }
}
