using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace MinoriBot.Enums
{
    public class Config
    {
        private static Config instance;

        public int listenPort;
        public string wsAddr;
        public int socketMode;
        public int httpListenPort;
        public bool proxy;
        public string githubproxy;
        public string sdcvproxy;

        private Config()
        {

        }

        public static Config Instance()
        {
            if (instance == null)
            {
                instance = new Config();
            }
            return instance;
        }
    }

    public class TempConfig
    {
        public int listenPort;
        public string wsAddr;
        public int socketMode;
        public int httpListenPort;
        public bool proxy;
        public string githubproxy;
        public string sdcvproxy;

        public void Init()
        {
            Config.Instance().listenPort = listenPort;
            Config.Instance().wsAddr = wsAddr;
            Config.Instance().socketMode = socketMode;
            Config.Instance().httpListenPort = httpListenPort;
            Config.Instance().proxy = proxy;
            Config.Instance().githubproxy = githubproxy;
            Config.Instance().sdcvproxy = sdcvproxy;
            Console.WriteLine("配置加载完成");
        }
    }
}
