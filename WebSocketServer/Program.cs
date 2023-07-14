using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {

       
        WebSocket.Start().Wait();

        Console.ReadKey();
    }
}