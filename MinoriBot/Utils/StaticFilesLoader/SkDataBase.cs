using MinoriBot.Enums.Sekai;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MinoriBot.Utils.StaticFilesLoader
{
    internal static class SkDataBase
    {
        public static List<SkCard> skCards;
        static string cardsUri = "https://sekai-world.github.io/sekai-master-db-diff/cards.json";
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
                string json =await File.ReadAllTextAsync("./asset/db/cards.json");
                skCards = JsonConvert.DeserializeObject<List<SkCard>>(json);
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
