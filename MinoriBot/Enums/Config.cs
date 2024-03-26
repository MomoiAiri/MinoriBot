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
        public string proxyaddr;
        public bool useGocq;
        public bool useKoishi;

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
        public string proxyaddr;
        public bool useGocq;
        public bool useKoishi;

        public void Init()
        {
            Config.Instance().listenPort = listenPort;
            Config.Instance().wsAddr = wsAddr;
            Config.Instance().socketMode = socketMode;
            Config.Instance().httpListenPort = httpListenPort;
            Config.Instance().proxy = proxy;
            Config.Instance().proxyaddr = proxyaddr;
            Config.Instance().useGocq = useGocq;
            Config.Instance().useKoishi = useKoishi;
            Console.WriteLine("配置加载完成");
        }
    }
}
