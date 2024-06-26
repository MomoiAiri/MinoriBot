﻿using MinoriBot;
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
    static HttpServer httpServer;
    static WebSocketReverse ws;

    static async Task Main(string[] args)
    {
        Console.WriteLine("初始化资源中。。。");
        await Init.Start();
        Console.WriteLine("初始化资源完成");
        try
        {
            //Http服务(Koishi)
            if (Config.Instance().useKoishi)
            {
                httpServer = new HttpServer(Config.Instance().httpListenPort);
            }
            //ws服务(gocq)
            if (Config.Instance().useGocq)
            {
                //正向ws连接
                if (Config.Instance().socketMode == 1 && Config.Instance().wsAddr != "")
                {
                    WebSocketPositive ws = new WebSocketPositive(Config.Instance().wsAddr);
                    ws.Start().Wait();
                }
                //反向ws监听
                if (Config.Instance().socketMode == 2 && Config.Instance().listenPort != 0)
                {
                    WebSocketReverse ws = new WebSocketReverse(Config.Instance().listenPort);
                    ws.Start().Wait();
                    throw (new Exception("配置文件有误，无法启动Socket"));
                }
            }
            
        }
        catch (Exception ex) { Console.WriteLine("Error: " + ex); }
        bool run = true;
        while (run)
        {
            string cmd = Console.ReadLine();
            if(cmd == "exit")
            {
                run = false;
            }
        }
        
    }
}