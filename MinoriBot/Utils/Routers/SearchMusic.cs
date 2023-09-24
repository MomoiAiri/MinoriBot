using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.Routers
{
    public static class SearchMusic
    {
        public static async Task<string> SearchSkMusics(string message)
        {
            if (int.TryParse(message, out int musicId))
            {
                bool isFound = false;
                for (int i = 0; i < SkDataBase.skMusics.Count; i++)
                {
                    if (SkDataBase.skMusics[i].id == musicId)
                    {
                        return await ImageCreater.DrawMusicInfo(SkDataBase.skMusics[i]);
                    }
                }
                if (!isFound)
                {
                    return "error";
                }
            }
            string[] keywords = message.Split(' ');
            //将模糊的关键词转换成唯一的关键词
            Dictionary<string, List<string>> keys = FuzzySearch.FuzzySearchMusic(keywords);
            if (keys.Count == 0)
            {
                return "error";
            }
            List<SkMusics> skMusics = await GetMatchingMusics(SkDataBase.skMusics, keys);
            Console.WriteLine("一共查找到" + skMusics.Count + "首歌");
            if (skMusics.Count == 1 && keys.ContainsKey("musicName"))
            {
                return await ImageCreater.DrawMusicInfo(skMusics[0]);
            }
            foreach(SkMusics music in skMusics)
            {
                Console.WriteLine(music.id + " "+ music.title + "    " +music.creator);
            }
            if (skMusics.Count == 0) return "none";
            string file = await ImageCreater.DrawMusicList(skMusics);
            return file;
        }
        static async Task<List<SkMusics>> GetMatchingMusics(List<SkMusics> skMusics, Dictionary<string, List<string>> searchConditions)
        {
            var query = skMusics.AsQueryable();

            foreach (var condition in searchConditions)
            {
                switch (condition.Key)
                {
                    case "diffType":
                        if (searchConditions.ContainsKey("diffLevel") && searchConditions["diffType"].Count == 1)//难度种类与难度等级只能是唯一量，且需要使用难度种类时必须有难度等级参数
                        {
                            if (searchConditions["diffLevel"].Count == 1)
                            {
                                query = query.Where(skMusics => skMusics.GetDifficulty(searchConditions["diffType"][0]).ToString() == searchConditions["diffLevel"][0]);
                            }
                        }
                        break;
                    case "diffLevel":
                        query = query.Where(skMusics => condition.Value.Any(skMusics.GetDifficulties().Select(x=>x.ToString()).ToList().Contains));
                        break;
                    case "group":
                        query = query.Where(skMusics => condition.Value.Any(group => skMusics.GetGroups().Contains(group)));
                        break;
                    case "character":
                        query = query.Where(skMusics => skMusics.GetVocals().Any(vocal => condition.Value.All(condition=>vocal.Any(chara => chara.characterId.ToString() == condition))));
                        break;
                    case "musicName":
                        query = query.Where(skMusics => condition.Value.Any(name => skMusics.title.ToLower().Contains(name)));
                        break;
                }
            }
            return query.ToList();
        }
    }
}
