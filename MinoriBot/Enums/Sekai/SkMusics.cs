﻿using MinoriBot.Utils.StaticFilesLoader;
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
        /// 获取特定难度的难度等级
        /// </summary>
        /// <param name="diffType"></param>
        /// <returns></returns>
        public int GetDifficulty(string diffType)
        {
            List<int> difficulties = GetDifficulties();
            if (difficulties.Count > 0)
            {
                switch (diffType)
                {
                    case "easy":
                        return difficulties[0];
                    case "normal":
                        return difficulties[1];
                    case "hard":
                        return difficulties[2];
                    case "expert":
                        return difficulties[3];
                    case "master":
                        return difficulties[4];
                }
                return 0;
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
        public List<List<int>> GetVocals()
        {
            List<SkMusicVocals> musicVocals = SkDataBase.skMusicVocals.Where(vocal => vocal.musicId == id).ToList();
            List<List<int>> result = new List<List<int>>(); 
            for(int i =0;i<musicVocals.Count;i++)
            {
                List<int> characters = new List<int>();
                for(int j = 0; j < musicVocals[i].characters.Count; j++)
                {
                    if (musicVocals[i].characters[j].characterType == "game_character")
                    {
                        characters.Add(musicVocals[i].characters[j].characterId);
                    }
                }
                result.Add(characters);
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
            List<List<int>> vocals = GetVocals();
            if(vocals.Count > 1)
            {
                for (int i = 0; i < vocals.Count; i++)
                {
                    int groupId = -1;
                    bool isOneGroup = true;
                    for (int j = 0; j < vocals[i].Count; j++)
                    {
                        if (groupId == -1)
                        {
                            if (vocals[i][j] < 21 || vocals[i][j] > 26)
                            {
                                groupId = (vocals[i][j] - 1) / 4;
                            }
                        }
                        else
                        {
                            if (groupId != (vocals[i][j] - 1) / 4)
                            {
                                if (vocals[i][j] < 21 || vocals[i][j] > 26)
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
            else
            {
                if (vocals[0].All(item => item >= 21 && item <= 26)) result.Add("piapro");
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
