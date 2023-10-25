using MinoriBot.Utils.StaticFilesLoader;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Enums.Sekai
{
    public class SkMusics
    {
        public int id;
        public int seq;
        public int releaseConditionId;
        public List<string> categories;
        public string title;
        public string pronunciation;
        public string creator;
        public string lyricist;
        public string composer;
        public string arranger;
        public int dancerCount;
        public int selfDancerPosition;
        public string assetbundleName;
        public string liveTalkBackgroundAssetbundleName;
        public long publishedAt;
        public int liveStageId;
        public double fillerSec;
        /// <summary>
        /// 获取歌曲的五个难度，索引越大难度越高
        /// </summary>
        /// <returns></returns>
        public Dictionary<string ,int> GetDifficulties()
        {
            Dictionary<string, int> difficulties = new Dictionary<string, int>();
            var query = SkDataBase.skMusicDifficulties.AsQueryable();
            query = query.Where(diff => diff.musicId == id);
            List<SkMusicDifficulties> diffs = query.ToList();
            for(int i = 0; i < diffs.Count; i++)
            {
                difficulties.Add(diffs[i].musicDifficulty,diffs[i].playLevel);
            }
            SortDictionary(ref difficulties);
            return difficulties;
        }
        void SortDictionary(ref Dictionary<string, int> input)
        {
            List<string> order = new List<string>() {"easy","normal","hard","expert","master","append" };
            input = input.OrderBy(item => order.IndexOf(item.Key)).ToDictionary(item => item.Key, item => item.Value);
        }
        /// <summary>
        /// 获取特定难度的难度等级
        /// </summary>
        /// <param name="diffType"></param>
        /// <returns></returns>
        public int GetDifficulty(string diffType)
        {
            Dictionary<string, int> difficulties = GetDifficulties();
            if (difficulties.Count > 0)
            {
                return difficulties[diffType];
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取演唱者
        /// </summary>
        /// <returns></returns>
        public List<List<SkMusicVocals.Characters>> GetVocals()
        {
            List<SkMusicVocals> musicVocals = SkDataBase.skMusicVocals.Where(vocal => vocal.musicId == id).ToList();
            List<List<SkMusicVocals.Characters>> result = new List<List<SkMusicVocals.Characters>>(); 
            for(int i =0;i<musicVocals.Count;i++)
            {
                result.Add(musicVocals[i].characters);
            }
            return result;
        }
        /// <summary>
        /// 获取该曲所属团队
        /// </summary>
        /// <returns></returns>
        public List<string> GetGroups()
        {
            List<string> result = new List<string>();
            List<List<SkMusicVocals.Characters>> vocals = GetVocals();
            if(vocals.Count > 1)
            {
                for (int i = 0; i < vocals.Count; i++)
                {
                    if (vocals[i].Any(vocal => vocal.characterType == "game_character"))
                    {
                        int groupId = -1;
                        bool isOneGroup = true;
                        for (int j = 0; j < vocals[i].Count; j++)
                        {
                            if (groupId == -1)
                            {
                                if (vocals[i][j].characterId < 21 || vocals[i][j].characterId > 26)
                                {
                                    groupId = (vocals[i][j].characterId - 1) / 4;
                                }
                            }
                            else
                            {
                                if (groupId != (vocals[i][j].characterId - 1) / 4)
                                {
                                    if (vocals[i][j].characterId < 21 || vocals[i][j].characterId > 26)
                                    {
                                        result.Add("others");
                                        isOneGroup = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (isOneGroup)
                        {
                            switch (groupId)
                            {
                                case 0:
                                    result.Add("light_sound");
                                    break;
                                case 1:
                                    result.Add("idol");
                                    break;
                                case 2:
                                    result.Add("street");
                                    break;
                                case 3:
                                    result.Add("theme_park");
                                    break;
                                case 4:
                                    result.Add("school_refusal");
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (vocals[0].All(item => item.characterId >= 21 && item.characterId <= 26)) result.Add("piapro");
            }
            return result;
        }
        /// <summary>
        /// 获取音乐封面
        /// </summary>
        /// <returns></returns>
        public async Task<SKBitmap> GetMusicJacket()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = basePath + $"asset/music/jacket/{assetbundleName}_rip";
            string filePath = fileDirectory + $"/{assetbundleName}.png";
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
                        byte[] imageBytes = await client.GetByteArrayAsync($"https://storage.sekai.best/sekai-assets/music/jacket/{assetbundleName}_rip/{assetbundleName}.png");

                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                            Console.WriteLine($"保存图片{assetbundleName}成功");
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
    }
}
