using MinoriBot.Utils.StaticFilesLoader;
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

            public string GetOutsideCharacterName()
            {
                for(int i =0;i<SkDataBase.skOutsideCharacters.Count;i++)
                {
                    if(characterType== "outside_character" && characterId == SkDataBase.skOutsideCharacters[i].id)
                    {
                        return SkDataBase.skOutsideCharacters[i].name;
                    }
                }
                return "";
            }
        }
        public string assetbundleName;
        public long archivePublishedAt;

    }
}
