using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using MinoriBot.Utils.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace MinoriBot.Utils.Routers
{
    public static class SearchMusic
    {
        public static async Task<List<MessageObj>> SearchSkMusics(string message)
        {
            List<MessageObj> result = new List<MessageObj>();
            if (int.TryParse(message, out int musicId))
            {
                bool isFound = false;
                for (int i = 0; i < SkDataBase.skMusics.Count; i++)
                {
                    if (SkDataBase.skMusics[i].id == musicId)
                    {
                        isFound = true;
                        result.Add(await MusicDetail.DrawMusicDetails(SkDataBase.skMusics[i]));
                        return result;
                    }
                }
                if (!isFound)
                {
                    result.Add(new MessageObj() { type = "string", content = "没有查找到该ID的歌曲" });
                    return result;
                }
            }
            string[] keywords = message.Split(' ');
            //将模糊的关键词转换成唯一的关键词
            Dictionary<string, List<string>> keys = FuzzySearch.FuzzySearchMusic(keywords);
            SortSearchConditions(ref keys);
            if (keys.Count == 0)
            {
                result.Add(new MessageObj() { type = "string", content = "关键词有误" });
                return result;
            }
            foreach(KeyValuePair<string,List<string>> k in keys)
            {
                Console.WriteLine(k.Key+":");
                foreach(string s in k.Value)
                {
                    Console.Write("  "+s);
                }
                Console.WriteLine();
            }
            List<SkMusics> skMusics = GetMatchingMusics(SkDataBase.skMusics, keys);
            if (skMusics != null && skMusics.Count>0)
            {
                Console.WriteLine("一共查找到" + skMusics.Count + "首歌");
                if (skMusics.Count == 1 && keys.ContainsKey("musicName"))
                {
                    result.Add(await MusicDetail.DrawMusicDetails(skMusics[0]));
                    return result;
                }
                foreach (SkMusics music in skMusics)
                {
                    Console.WriteLine(music.id + " " + music.title + "    " + music.creator);
                }
                result.Add(await MusicList.DrawMusicList(skMusics));
                return result;
            }
            else
            {
                result.Add(new MessageObj() { type = "string", content = "没有查找到相关歌曲" });
                return result;
            }
        }
        static void SortSearchConditions(ref Dictionary<string, List<string>> input)
        {
            List<string> order = new List<string>() { "group", "character", "diffLevel", "diffType", "musicName" };
            input = input.OrderBy(item => order.IndexOf(item.Key)).ToDictionary(item => item.Key, item => item.Value);
        }
        static List<SkMusics> GetMatchingMusics(List<SkMusics> skMusics, Dictionary<string, List<string>> searchConditions)
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
                                string difficultyType = searchConditions["diffType"][0];
                                string level = searchConditions["diffLevel"][0];
                                query = query.Where(skMusics => skMusics.GetDifficulty(difficultyType).ToString() == level);
                            }
                        }
                        else
                        {
                            return null;
                        }
                        //try
                        //{
                        //    List<SkMusics> musics = query.ToList();
                        //    foreach (SkMusics m in musics)
                        //    {
                        //        Console.WriteLine(m.title);
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex.Message);
                        //}
                        break;
                    case "diffLevel":
                        query = query.Where(skMusics => condition.Value.Any(skMusics.GetDifficulties().Values.ToList().Select(x => x.ToString()).ToList().Contains));
                        //try
                        //{
                        //    List<SkMusics> musics = query.ToList();
                        //    foreach(SkMusics m in musics)
                        //    {
                        //        Console.WriteLine(m.title);
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex.Message);
                        //}
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
