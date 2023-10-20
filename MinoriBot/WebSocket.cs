using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MinoriBot.Utils;

public  class WebSocketReverse
{
    private  HttpListener _listener;

    private  CancellationTokenSource _cancellationTokenSource;

    private  PraseMessage _praseMessage;

    private  List<System.Net.WebSockets.WebSocket> webSockets= new List<System.Net.WebSockets.WebSocket>();

    public WebSocketReverse(int listenPort)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{listenPort}/");
        _cancellationTokenSource = new CancellationTokenSource();
        _praseMessage = new PraseMessage();
    }

    public async Task Start()
    {
        _listener.Start();
        Console.WriteLine("WebSocket server started.");

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            HttpListenerContext context = await _listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                ProcessWebSocketRequest(context);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    public  void Stop()
    {
        _listener.Stop();
        _cancellationTokenSource.Cancel();
        Console.WriteLine("WebSocket server stopped.");
    }

    private  async void ProcessWebSocketRequest(HttpListenerContext context)
    {
        HttpListenerWebSocketContext webSocketContext = null;

        try
        {
            webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
            Console.WriteLine("WebSocket connection established.");
            //把确认的WebSocket连接 放在List里面
            webSockets.Add(webSocketContext.WebSocket);
            await HandleWebSocketRequest(webSocketContext.WebSocket);
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
        }
        finally
        {
            if (webSocketContext != null)
            {
                webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed.", CancellationToken.None).Wait();
                webSocketContext.WebSocket.Dispose();
                Console.WriteLine("WebSocket connection closed.");
            }
        }
    }

    private  async Task HandleWebSocketRequest(System.Net.WebSockets.WebSocket webSocket)
    {
        byte[] buffer = new byte[1024];

        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                //Console.WriteLine("Receive Message: "+message);

                // 在这里处理从客户端接收到的消息

                // 将消息发送回客户端
                _praseMessage.ProcessingMessage(message,webSocket);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed.", CancellationToken.None);
                Console.WriteLine("WebSocket connection closed by the client.");
                webSockets.Remove(webSocket);
            }
        }
    }
}


public class WebSocketPositive
{
    private ClientWebSocket _webSocket = new ClientWebSocket();
    private WebSocket _ws;
    private string _wsAddr = string.Empty;
    private PraseMessage _praseMessage;
    public WebSocketPositive(string wsAddr)
    {
        _wsAddr= wsAddr;
        _praseMessage = new PraseMessage();
    }

    public async Task Start()
    {
        while (true)
        {
            try
            {
                //Console.WriteLine("ws-state: " + _webSocket.State);
                if (_webSocket.State != WebSocketState.Open)
                {
                    await _webSocket.ConnectAsync(new Uri(_wsAddr), CancellationToken.None);
                    Console.WriteLine($"已连接到gocq服务{_wsAddr}");
                }
                await ReceiveMessages();
                Console.WriteLine("连接出现异常已断开，尝试重新连接");
            }
            catch (Exception ex)
            {
                _webSocket.Dispose();
                _webSocket = new ClientWebSocket();
                Console.WriteLine($"出现异常断开连接\nError: {ex.Message}");
            }
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
    private async Task ReceiveMessages()
    {
        try
        {
            var buffer = new byte[1024];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    //Console.WriteLine(message);
                    _praseMessage.ProcessingMessage(message, _webSocket);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in receiving messages: {ex.Message}");
        }
    } 
}

public static class MessageSender
{
    public static async Task SendMessage(string message,WebSocket ws)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
    //public static async Task SendMessage(string message,ClientWebSocket ws)
    //{
    //    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
    //    await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    //}
}