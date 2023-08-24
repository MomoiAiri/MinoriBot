﻿using MinoriBot.Enums.Sekai;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MinoriBot.Utils.StaticFilesLoader
{
    internal static class SkDataBase
    {
        public static List<SkCard> skCards;
        public static List<SkSkills> skSkills;
        public static List<SkEvents> skEvents;
        public static List<SkEventCards> skEventCards;
        static string cardsUri = "https://sekai-world.github.io/sekai-master-db-diff/cards.json";
        static string cardSkillsUri = "https://sekai-world.github.io/sekai-master-db-diff/skills.json";
        static string eventsUri = "https://sekai-world.github.io/sekai-master-db-diff/events.json";
        static string eventCardsUri = "https://sekai-world.github.io/sekai-master-db-diff/eventCards.json";
        static System.Timers.Timer timer;

        static SkDataBase()
        {
            //检查是否有数据库存放的路径
            string datebasePath = AppDomain.CurrentDomain.BaseDirectory + "asset/db";
            if (!Directory.Exists(datebasePath))
            {
                Directory.CreateDirectory(datebasePath);
            }
        }
        /// <summary>
        /// 启用SkDataBase
        /// </summary>
        /// <returns></returns>
        public static async Task Start()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/cards.json"))
            {
                await GetCardsDB();
            }
            else
            {
                string json = await File.ReadAllTextAsync("./asset/db/cards.json");
                skCards = JsonConvert.DeserializeObject<List<SkCard>>(json);
            }
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/skills.json"))
            {
                await GetCardSkillDB();
            }
            else
            {
                string json = await File.ReadAllTextAsync("./asset/db/skills.json");
                skSkills = JsonConvert.DeserializeObject<List<SkSkills>>(json);
            }
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/events.json"))
            {
                await GetEventDB();
            }
            else
            {
                string json = await File.ReadAllTextAsync("./asset/db/events.json");
                skEvents = JsonConvert.DeserializeObject<List<SkEvents>>(json);
            }
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/eventCards.json"))
            {
                await GetEventCardsDB();
            }
            else
            {
                string json = await File.ReadAllTextAsync("./asset/db/eventCards.json");
                skEventCards = JsonConvert.DeserializeObject<List<SkEventCards>>(json);
            }

            UpdateDB();
        }
        /// <summary>
        /// 从sekai.best获取cards数据
        /// </summary>
        /// <returns></returns>
        static async Task GetCardsDB()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync(cardsUri);
                    if (json != null)
                    {
                        File.WriteAllText("./asset/db/cards.json", json);
                        skCards = JsonConvert.DeserializeObject<List<SkCard>>(json);
                        Console.WriteLine("获取数据成功");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("出现异常\n" + ex);
                }
            }
        }
        /// <summary>
        /// 卡牌技能数据
        /// </summary>
        /// <returns></returns>
        static async Task GetCardSkillDB()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync(cardSkillsUri);
                    if (json != null)
                    {
                        File.WriteAllText("./asset/db/skills.json", json);
                        skSkills = JsonConvert.DeserializeObject<List<SkSkills>>(json);
                        Console.WriteLine("获取数据成功");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("出现异常\n" + ex);
                }
            }
        }
        /// <summary>
        /// 活动数据
        /// </summary>
        /// <returns></returns>
        static async Task GetEventDB()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync(eventsUri);
                    if (json != null)
                    {
                        File.WriteAllText("./asset/db/events.json", json);
                        skEvents = JsonConvert.DeserializeObject<List<SkEvents>>(json);
                        Console.WriteLine("获取数据成功");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("出现异常\n" + ex);
                }
            }
        }
        /// <summary>
        /// 活动卡牌
        /// </summary>
        /// <returns></returns>
        static async Task GetEventCardsDB()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync(eventCardsUri);
                    if (json != null)
                    {
                        File.WriteAllText("./asset/db/eventCards.json", json);
                        skEventCards = JsonConvert.DeserializeObject<List<SkEventCards>>(json);
                        Console.WriteLine("获取数据成功");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("出现异常\n" + ex);
                }
            }
        }
        /// <summary>
        /// 定时更新数据
        /// </summary>
        static void UpdateDB()
        {
            timer = new System.Timers.Timer();
            int intervalInMilliseconds = 60 * 60 * 1000;
            timer.Interval = intervalInMilliseconds;
            timer.Elapsed += async (sender, e) => await UpdateCards(sender, e); 
            timer.Start();
        }
        static async Task UpdateCards(object sender,ElapsedEventArgs e)
        {
            await GetCardsDB();
        }
    }
}
