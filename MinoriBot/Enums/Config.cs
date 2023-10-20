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
        public int listenPort;
        public string wsAddr;
        public int socketMode;
        public int httpListenPort;
    }
}
