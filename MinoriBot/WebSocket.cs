using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MinoriBot.Utils;

public static class WebSocket
{
    private static HttpListener _listener;

    private static CancellationTokenSource _cancellationTokenSource;

    private static PraseMessage _praseMessage;

    private static List<System.Net.WebSockets.WebSocket> webSockets= new List<System.Net.WebSockets.WebSocket>();

    static WebSocket()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:1200/");
        _cancellationTokenSource = new CancellationTokenSource();
        _praseMessage = new PraseMessage();
    }

    public static async Task Start()
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

    public static void Stop()
    {
        _listener.Stop();
        _cancellationTokenSource.Cancel();
        Console.WriteLine("WebSocket server stopped.");
    }

    private static async void ProcessWebSocketRequest(HttpListenerContext context)
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

    private static async Task HandleWebSocketRequest(System.Net.WebSockets.WebSocket webSocket)
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
                string reply = _praseMessage.ProcessingMessage(message);
                if (reply != "")
                {
                    byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(reply);
                    Console.WriteLine("Sended Message: " + reply);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer, 0, responseBuffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed.", CancellationToken.None);
                Console.WriteLine("WebSocket connection closed by the client.");
                webSockets.Remove(webSocket);
            }
        }
    }
    public static async Task SendMessage(string message)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
        await webSockets[0].SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}
