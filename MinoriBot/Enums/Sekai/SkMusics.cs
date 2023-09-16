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
        public List<int> GetDifficulties()
        {
            List<int> difficulties = new List<int>();
            int left = 0;
            int right = SkDataBase.skMusicDifficulties.Count;
            int mid = 0;
            while (left <= right)
            {
                mid = (left + right) / 2;
                if (id > SkDataBase.skMusicDifficulties[mid].musicId)
                {
                    left = mid + 1;
                }
                else if(id < SkDataBase.skMusicDifficulties[mid].musicId)
                {
                    right = mid - 1;
                }
                else
                {
                    difficulties.Add(SkDataBase.skMusicDifficulties[mid].playLevel);
                    break;
                }
            }
            left = mid - 1;
            right = mid + 1;
            while (true)
            {
                if (id == SkDataBase.skMusicDifficulties[left].musicId)
                {
                    difficulties.Add(SkDataBase.skMusicDifficulties[left].playLevel);
                    left--;
                }
                if (id == SkDataBase.skMusicDifficulties[right].musicId)
                {
                    difficulties.Add(SkDataBase.skMusicDifficulties[right].playLevel);
                    right++;
                }
                if (difficulties.Count > 4) break;
            }
            difficulties.Sort();
            return difficulties;
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
                        return null;
                    }
                }
                SKBitmap sKBitmap = SKBitmap.Decode(filePath);
                return sKBitmap;
            }
        }
    }
}
