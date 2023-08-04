using MinoriBot.Enums;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet;
using YamlDotNet.Serialization;

class Program
{
    
    static void Main(string[] args)
    {
        try
        {
            string yaml = File.ReadAllText("D:\\Code\\MinoriBot\\MinoriBot\\config.yaml");
            IDeserializer deserializer = new DeserializerBuilder().Build();
            Config config = deserializer.Deserialize<Config>(yaml);
            if (config.socketMode == 1 && config.wsAddr != "")
            {
                WebSocketPositive ws = new WebSocketPositive(config.wsAddr);
                ws.Start().Wait();
            }
            if(config.socketMode == 2 && config.listenPort!= 0)
            {
                WebSocketReverse ws = new WebSocketReverse(config.listenPort);
                ws.Start().Wait();
            }
            throw (new Exception("配置文件有误，无法启动Socket"));
        }
        catch (Exception ex) { Console.WriteLine("Error: " + ex); }
       

        Console.ReadKey();
    }
}