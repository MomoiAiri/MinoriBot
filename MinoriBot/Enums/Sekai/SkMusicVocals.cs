using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Enums.Sekai
{
    public class SkMusicVocals
    {
        public int id;
        public int musicId;
        public string musicVocalType;
        public int seq;
        public int releaseConditionId;
        public string caption;
        public List<Characters> characters;
        public class Characters
        {
            public int id;
            public int musicId;
            public int musicVocalId;
            public string characterType;
            public int characterId;
            public int seq;
        }
        public string assetbundleName;
        public long archivePublishedAt;
    }
}
