using MinoriBot.Enums;
using MinoriBot.Utils.StaticFilesLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace MinoriBot
{
    internal static class Init
    {
        public static async Task Start()
        {
            CreatConfigYaml();
            LoadConfig();
            await SekaiImageDownload.InitNormalImage();
            await SkDataBase.Start();
        }
        public static void CreatConfigYaml()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = path + "config.yaml";
            if (!File.Exists(fileName))
            {
                string configtxt = "#连接方式 1.正向ws  2.反向ws\r\nsocketMode: 1\r\n#监听端口，使用反向ws时填写\r\nlistenPort: 12001\r\n#ws地址，使用正向ws时填写 格式例子: \"ws://kohane.com:11451\"\r\nwsAddr: \"";
                File.WriteAllText(fileName, configtxt);
            }
        }
        private static void LoadConfig()
        {
            string yaml = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/config.yaml");
            IDeserializer deserializer = new DeserializerBuilder().Build();
            TempConfig tempConfig = deserializer.Deserialize<TempConfig>(yaml);
            tempConfig.Init();
        }
    }
}
