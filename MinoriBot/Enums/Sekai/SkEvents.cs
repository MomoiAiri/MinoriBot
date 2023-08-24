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
    }
}
