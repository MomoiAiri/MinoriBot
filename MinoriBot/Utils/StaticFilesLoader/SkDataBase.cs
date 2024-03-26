using Microsoft.Extensions.Logging;
using MinoriBot.Enums.Sekai;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using MinoriBot.Enums;

namespace MinoriBot.Utils.StaticFilesLoader
{
    internal static class SkDataBase
    {
        public static List<SkCard> skCards;
        public static List<SkSkills> skSkills;
        public static List<SkEvents> skEvents;
        public static List<SkEventCards> skEventCards;
        public static List<SkEventDeckBonuses> skEventDeckBonuses;
        public static List<SkGameCharacterUnits> skGameCharacterUnits;
        public static List<SkGachas> skGachas;
        public static List<SkGachaCeilItemscs> skGachaCeilItemscs;
        public static List<SkMusics> skMusics;
        public static List<SkMusicVocals> skMusicVocals;
        public static List<SkMusicDifficulties> skMusicDifficulties;
        public static List<SkHonorGroups> skHonorGroups;
        public static List<SkOutsideCharacters> skOutsideCharacters;
        static string datebaseUri = "https://sekai-world.github.io";
        static string cardsUri = $"{datebaseUri}/sekai-master-db-diff/cards.json";
        static string cardSkillsUri = $"{datebaseUri}/sekai-master-db-diff/skills.json";
        static string eventsUri = $"{datebaseUri}/sekai-master-db-diff/events.json";
        static string eventCardsUri = $"{datebaseUri}/sekai-master-db-diff/eventCards.json";
        static string eventDeckBonusesUri = $"{datebaseUri}/sekai-master-db-diff/eventDeckBonuses.json";
        static string gameCharacterUnitsUri = $"{datebaseUri}/sekai-master-db-diff/gameCharacterUnits.json";
        static string gachasUri = $"{datebaseUri}/sekai-master-db-diff/gachas.json";
        static string gachaCeilItemsUri = $"{datebaseUri}/sekai-master-db-diff/gachaCeilItems.json";
        static string musicsUri = $"{datebaseUri}/sekai-master-db-diff/musics.json";
        static string musicVocalsUri = $"{datebaseUri}/sekai-master-db-diff/musicVocals.json";
        static string musicDifficultiesUri = $"{datebaseUri}/sekai-master-db-diff/musicDifficulties.json";
        static string honorGroupsUri = $"{datebaseUri}/sekai-master-db-diff/honorGroups.json";
        static string outsideCharactersUri = $"{datebaseUri}/sekai-master-db-diff/outsideCharacters.json";
        static System.Timers.Timer timer;
        //static Dictionary<Type, List<object>> classDic = new Dictionary<Type, List<object>>() { { typeof(SkCard), new List<object> {skCards ,cardsUri ,"cards.json" } } };
        static SkDataBase()
        {
            //检查是否有数据库存放的路径
            string datebasePath = AppDomain.CurrentDomain.BaseDirectory + "asset/db";
            if (!Directory.Exists(datebasePath))
            {
                Directory.CreateDirectory(datebasePath);
            }
            Console.WriteLine("资源地址"+datebaseUri);
        }
        /// <summary>
        /// 启用SkDataBase
        /// </summary>
        /// <returns></returns>
        public static async Task Start()
        {
            await UpdateDB_Command(Mode.Load);
            UpdateDB(Mode.Update);
        }
        /// <summary>
        /// 从sekai.best获取cards数据
        /// </summary>
        /// <returns></returns>
        static async Task GetCardsDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/cards.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/cards.json");
                skCards = JsonConvert.DeserializeObject<List<SkCard>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
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
                                Console.WriteLine("DateBase:获取cards.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 卡牌技能数据
        /// </summary>
        /// <returns></returns>
        static async Task GetCardSkillDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/skills.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/skills.json");
                skSkills = JsonConvert.DeserializeObject<List<SkSkills>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
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
                                Console.WriteLine("DateBase:获取skill.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 活动数据
        /// </summary>
        /// <returns></returns>
        static async Task GetEventDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/events.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/events.json");
                skEvents = JsonConvert.DeserializeObject<List<SkEvents>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
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
                                Console.WriteLine("DateBase:获取event.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 活动卡牌
        /// </summary>
        /// <returns></returns>
        static async Task GetEventCardsDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/eventCards.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/eventCards.json");
                skEventCards = JsonConvert.DeserializeObject<List<SkEventCards>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
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
                                Console.WriteLine("DateBase:获取eventCards.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 活动加成
        /// </summary>
        /// <returns></returns>
        static async Task GetEventDeckBonusesDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/eventDeckBonuses.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/eventDeckBonuses.json");
                skEventDeckBonuses = JsonConvert.DeserializeObject<List<SkEventDeckBonuses>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            string json = await client.GetStringAsync(eventDeckBonusesUri);
                            if (json != null)
                            {
                                File.WriteAllText("./asset/db/eventDeckBonuses.json", json);
                                skEventDeckBonuses = JsonConvert.DeserializeObject<List<SkEventDeckBonuses>>(json);
                                Console.WriteLine("DateBase:获取eventDeckBonuses.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        static async Task GetGameCharacterUnitsDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/gameCharacterUnits.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/gameCharacterUnits.json");
                skGameCharacterUnits = JsonConvert.DeserializeObject<List<SkGameCharacterUnits>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            string json = await client.GetStringAsync(gameCharacterUnitsUri);
                            if (json != null)
                            {
                                File.WriteAllText("./asset/db/gameCharacterUnits.json", json);
                                skGameCharacterUnits = JsonConvert.DeserializeObject<List<SkGameCharacterUnits>>(json);
                                Console.WriteLine("DateBase:获取gameCharacterUnits.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取卡池数据
        /// </summary>
        /// <returns></returns>
        static async Task GetGachasDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/gachas.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/gachas.json");
                skGachas = JsonConvert.DeserializeObject<List<SkGachas>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            string json = await client.GetStringAsync(gachasUri);
                            if (json != null)
                            {
                                File.WriteAllText("./asset/db/gachas.json", json);
                                skGachas = JsonConvert.DeserializeObject<List<SkGachas>>(json);
                                Console.WriteLine("DateBase:gachas.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 卡池是否限定
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        static async Task GetGachaCeilItemsDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/gachaCeilItems.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/gachaCeilItems.json");
                skGachaCeilItemscs = JsonConvert.DeserializeObject<List<SkGachaCeilItemscs>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            string json = await client.GetStringAsync(gachaCeilItemsUri);
                            if (json != null)
                            {
                                File.WriteAllText("./asset/db/gachaCeilItems.json", json);
                                skGachaCeilItemscs = JsonConvert.DeserializeObject<List<SkGachaCeilItemscs>>(json);
                                Console.WriteLine("DateBase:gachaCeilItems.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取音乐数据
        /// </summary>
        /// <returns></returns>
        static async Task GetMusicsDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/musics.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/musics.json");
                skMusics = JsonConvert.DeserializeObject<List<SkMusics>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            string json = await client.GetStringAsync(musicsUri);
                            if (json != null)
                            {
                                File.WriteAllText("./asset/db/musics.json", json);
                                skMusics = JsonConvert.DeserializeObject<List<SkMusics>>(json);
                                Console.WriteLine("DateBase:musics.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        static async Task GetMusicVocalsDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/musicVocals.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/musicVocals.json");
                skMusicVocals = JsonConvert.DeserializeObject<List<SkMusicVocals>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            string json = await client.GetStringAsync(musicVocalsUri);
                            if (json != null)
                            {
                                File.WriteAllText("./asset/db/musicVocals.json", json);
                                skMusicVocals = JsonConvert.DeserializeObject<List<SkMusicVocals>>(json);
                                Console.WriteLine("DateBase:musicVocals.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        static async Task GetMusicDifficultiesDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/musicDifficulties.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/musicDifficulties.json");
                skMusicDifficulties = JsonConvert.DeserializeObject<List<SkMusicDifficulties>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            string json = await client.GetStringAsync(musicDifficultiesUri);
                            if (json != null)
                            {
                                File.WriteAllText("./asset/db/musicDifficulties.json", json);
                                skMusicDifficulties = JsonConvert.DeserializeObject<List<SkMusicDifficulties>>(json);
                                Console.WriteLine("DateBase:musicDifficulties.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        static async Task GetHonorGroupsDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/honorGroups.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/honorGroups.json");
                skHonorGroups = JsonConvert.DeserializeObject<List<SkHonorGroups>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            string json = await client.GetStringAsync(honorGroupsUri);
                            if (json != null)
                            {
                                File.WriteAllText("./asset/db/honorGroups.json", json);
                                skHonorGroups = JsonConvert.DeserializeObject<List<SkHonorGroups>>(json);
                                Console.WriteLine("DateBase:honorGroups.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        static async Task GetOutsideCharactersDB(Mode mode)
        {
            bool isExit = false;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "asset/db/outsideCharacters.json") && mode == Mode.Load)
            {
                string json = await File.ReadAllTextAsync("./asset/db/outsideCharacters.json");
                skOutsideCharacters = JsonConvert.DeserializeObject<List<SkOutsideCharacters>>(json);
                isExit = true;
            }
            else
            {
                if (mode == Mode.Update || !isExit)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            string json = await client.GetStringAsync(outsideCharactersUri);
                            if (json != null)
                            {
                                File.WriteAllText("./asset/db/outsideCharacters.json", json);
                                skOutsideCharacters = JsonConvert.DeserializeObject<List<SkOutsideCharacters>>(json);
                                Console.WriteLine("DateBase:获取outsideCharacters.json成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("出现异常\n" + ex);
                        }
                    }
                }
            }
        }
        //static async Task GetDB<T>(Mode mode,T type)
        //{
        //    bool isExit = false;
        //    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"asset/db/{classDic[typeof(T)][2]}") && mode == Mode.Load)
        //    {
        //        string json = await File.ReadAllTextAsync($"./asset/db/{classDic[typeof(T)][2]}");
        //        classDic[typeof(T)][0] = JsonConvert.DeserializeObject<List<T>>(json);
        //        isExit = true;
        //    }
        //    else
        //    {
        //        if (mode == Mode.Update || !isExit)
        //        {
        //            using (HttpClient client = new HttpClient())
        //            {
        //                try
        //                {
        //                    string json = await client.GetStringAsync(classDic[typeof(T)][1].ToString());
        //                    if (json != null)
        //                    {
        //                        File.WriteAllText($"./asset/db/{classDic[typeof(T)][2]}", json);
        //                        classDic[typeof(T)][0] = JsonConvert.DeserializeObject<List<T>>(json);
        //                        Console.WriteLine($"DateBase:获取{classDic[typeof(T)][2]}成功");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine("出现异常\n" + ex);
        //                }
        //            }
        //        }
        //    }
        ////}
        /// <summary>
        /// 定时更新数据
        /// </summary>
        static void UpdateDB(Mode mode)
        {
            timer = new System.Timers.Timer();
            int intervalInMilliseconds = 60 * 60 * 1000;
            timer.Interval = intervalInMilliseconds;
            timer.Elapsed += async (sender, e) => await UpdateDB(sender, e, mode); 
            timer.Start();
        }
        static async Task UpdateDB(object sender,ElapsedEventArgs e, Mode mode)
        {
            Console.WriteLine("DataBase:正在更新数据库");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await GetCardsDB(mode);
            await GetCardSkillDB(mode);
            await GetEventDB(mode);
            await GetEventCardsDB(mode);
            await GetEventDeckBonusesDB(mode);
            await GetGameCharacterUnitsDB(mode);
            await GetGachasDB(mode);
            await GetGachaCeilItemsDB(mode);
            await GetMusicsDB(mode);
            await GetMusicVocalsDB(mode);
            await GetMusicDifficultiesDB(mode);
            await GetHonorGroupsDB(mode);
            await GetOutsideCharactersDB(mode);
            stopwatch.Stop();
            Console.WriteLine($"DataBase:更新数据库执行完成，花费时间{stopwatch.ElapsedMilliseconds/1000}秒");
        }
        public static async Task<string> UpdateDB_Command(Mode mode)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await GetCardsDB(mode);
            await GetCardSkillDB(mode);
            await GetEventDB(mode);
            await GetEventCardsDB(mode);
            await GetEventDeckBonusesDB(mode);
            await GetGameCharacterUnitsDB(mode);
            await GetGachasDB(mode);
            await GetGachaCeilItemsDB(mode);
            await GetMusicsDB(mode);
            await GetMusicVocalsDB(mode);
            await GetMusicDifficultiesDB(mode);
            await GetHonorGroupsDB(mode);
            await GetOutsideCharactersDB(mode);
            stopwatch.Stop();
            return $"更新数据库执行完成，花费时间{stopwatch.ElapsedMilliseconds / 1000}秒";
        }
    }
    enum Mode
    {
        Load,
        Update,
    }
}
