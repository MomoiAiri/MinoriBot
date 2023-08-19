using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MinoriBot.Utils.StaticFilesLoader
{
    public static class SekaiImageDownload
    {
        static Dictionary<string, string> images = new Dictionary<string, string>()
        {
            {"mysterious","https://sekai.best/assets/icon_attribute_mysterious.3e1ce6e3.png" },
            {"cool","https://sekai.best/assets/icon_attribute_cool.a557ef6d.png" },
            {"pure","https://sekai.best/assets/icon_attribute_pure.a6d18973.png" },
            {"happy","https://sekai.best/assets/icon_attribute_happy.5b5502eb.png" },
            {"cute","https://sekai.best/assets/icon_attribute_cute.f299ec08.png" },
            {"normal_star","https://sekai.best/assets/rarity_star_normal.11f97176.png" },
            {"after_training_star","https://sekai.best/assets/rarity_star_afterTraining.442b961e.png" },
            {"birthday_star","https://sekai.best/assets/rarity_birthday.e2366ea2.png" },
            {"cardIconFrame_4","https://sekai.best/assets/cardFrame_S_4.7bd58b3b.png" },
            {"cardIconFrame_3","" },
            {"cardIconFrame_2","" },
            {"cardIconFrame_1","" },
            {"cardIconFrame_bd","https://sekai.best/assets/cardFrame_S_bd.a4eb6504.png" },
            {"cardFrame_4","https://sekai.best/assets/cardFrame_L_4.53f11c10.png" },
            {"cardFrame_3","https://sekai.best/assets/cardFrame_L_3.e1570788.png" },
            {"cardFrame_2","https://sekai.best/assets/cardFrame_L_2.316ac3ff.png" },
            {"cardFrame_1","https://sekai.best/assets/cardFrame_L_1.d476763d.png" },
            {"cardFrame_bd","https://sekai.best/assets/cardFrame_L_bd.bd2a499c.png" },
            {"1","https://sekai.best/assets/chr_ts_1.401e8301.png" },
            {"2","https://sekai.best/assets/chr_ts_2.6d1e40cf.png" },
            {"3","https://sekai.best/assets/chr_ts_3.176d81e8.png" },
            {"4","https://sekai.best/assets/chr_ts_4.5caf27c5.png" },
            {"5","https://sekai.best/assets/chr_ts_5.3568c1d2.png" },
            {"6","https://sekai.best/assets/chr_ts_6.6409abaa.png" },
            {"7","https://sekai.best/assets/chr_ts_7.59adc6cf.png" },
            {"8","https://sekai.best/assets/chr_ts_8.503f2eda.png" },
            {"9","https://sekai.best/assets/chr_ts_9.fad2bcfb.png" },
            {"10","https://sekai.best/assets/chr_ts_10.1375c978.png" },
            {"11","https://sekai.best/assets/chr_ts_11.a0613678.png" },
            {"12","https://sekai.best/assets/chr_ts_12.74f77746.png" },
            {"13","https://sekai.best/assets/chr_ts_13.003213bc.png" },
            {"14","https://sekai.best/assets/chr_ts_14.38de5874.png" },
            {"15","https://sekai.best/assets/chr_ts_15.06a049a5.png" },
            {"16","https://sekai.best/assets/chr_ts_16.00b69576.png" },
            {"17","https://sekai.best/assets/chr_ts_17.a0cc7f22.png" },
            {"18","https://sekai.best/assets/chr_ts_18.dc5229c5.png" },
            {"19","https://sekai.best/assets/chr_ts_19.1457130a.png" },
            {"20","https://sekai.best/assets/chr_ts_20.999eb43d.png" },
            {"21","https://sekai.best/assets/chr_ts_21.024fabcc.png" },
            {"22","https://sekai.best/assets/chr_ts_22.4ab35893.png" },
            {"23","https://sekai.best/assets/chr_ts_23.e342f1b7.png" },
            {"24","https://sekai.best/assets/chr_ts_24.5b4a2497.png" },
            {"25","https://sekai.best/assets/chr_ts_25.83d109af.png" },
            {"26","https://sekai.best/assets/chr_ts_26.ce9e612d.png" },
        };
        static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        static List<String> pathList;
        static SekaiImageDownload()
        {
            pathList = new List<string>()
            {
                baseDirectory + "asset/normal",
                baseDirectory + "asset/character/member",
                baseDirectory + "asset/thumbnail/chara_rip",
                baseDirectory + "asset/temp"
            };
            //InitNormalImage();
        }
        public static async Task InitNormalImage()
        {
            //检查存储库路径是否存在
            foreach(string path in pathList)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            foreach (KeyValuePair<string, string> kvp in images)
            {
                //查看文件是否已经存在
                if (!System.IO.File.Exists(pathList[0] + $"/{kvp.Key}.png"))
                {
                    Console.WriteLine("缺少资源:" + kvp.Key);
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            byte[] imageBytes = await client.GetByteArrayAsync(kvp.Value);
                            using (FileStream fileStream = new FileStream(pathList[0] + $"/{kvp.Key}.png", FileMode.Create))
                            {
                                await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                                Console.WriteLine($"保存图片{kvp.Key}成功");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Download Image Failed");
                        }
                    }
                }
            }
        }
    }
}
