

using MinoriBot.Enums;
using MinoriBot.Utils.Routers;
using MinoriBot.Utils.View;
using Newtonsoft.Json;
using System.Drawing;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;

public class HttpServer
{

    HttpListener listener = new HttpListener();
    
    public HttpServer(int port)
    {
        string[] prefixs = new string[]
        {
            $"http://*:{port}/test/",
            $"http://*:{port}/card/",
            $"http://*:{port}/cardIll/",
            $"http://*:{port}/event/",
            $"http://*:{port}/music/",
            $"http://*:{port}/chart/",
            $"http://*:{port}/gacha/"
        };
        for (int i =0;i<prefixs.Length;i++)
        {
            listener.Prefixes.Add(prefixs[i]);
        }
        Thread thread = new Thread(async () => await Start());
        thread.Start();
    }
    public async Task Start()
    {
        listener.Start();
        Console.WriteLine($"Http服务已启用,监听端口:{Config.Instance().httpListenPort}");
        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            if (request.HttpMethod == "POST")
            {
                string requestPath = request.Url.AbsolutePath;
                if (requestPath.StartsWith("/test"))
                {
                    await SendResponse(request, response, "This is a test message");
                }
                else if (requestPath.StartsWith("/cardIll"))
                {
                    await SendResponse(request, response, async (input) => await SearchCard.GetCardIllustrationImage(input));
                }
                else if (requestPath.StartsWith("/card"))
                {
                    await SendResponse(request, response, async(input) => await SearchCard.SearchCharacter(input));
                }
                else if (requestPath.StartsWith("/event"))
                {
                    await SendResponse(request, response, async (input) => await SearchEvent.SearchSkEvents(input));
                }
                else if (requestPath.StartsWith("/music"))
                {
                    await SendResponse(request, response, async (input) => await SearchMusic.SearchSkMusics(input));
                }
                else if (requestPath.StartsWith("/chart"))
                {
                    await SendResponse(request, response, async (input) => await SearchChart.SearchSkMusicChart(input));
                }
                else if (requestPath.StartsWith("/gacha"))
                {

                }
                else
                {

                }
            }
            else if (request.HttpMethod == "GET")
            {
                byte[] responseBytes = Encoding.UTF8.GetBytes("Hello, GET request!");
                response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            }
        }
    }
    private async Task SendResponse(HttpListenerRequest request,HttpListenerResponse response, Func<string, Task<string>> func)
    {
        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            string requestBody = reader.ReadToEnd();
            Console.WriteLine("Received request: " + requestBody);

            HttpMessage message = JsonConvert.DeserializeObject<HttpMessage>(requestBody);
            if (message != null && message.context!=null)
            {
                string file = await func(message.context[0].content);
                string msgType = "image";
                if (file == "内部错误" || file=="none" || file=="error")
                {
                    msgType = "string";
                }
                List<HttpMessage.Context> context = new List<HttpMessage.Context>() { new HttpMessage.Context {type = msgType ,content = file } };

                HttpMessage backRespone = new HttpMessage() { context = context ,from ="minori"}; 
                string responseString = JsonConvert.SerializeObject(backRespone);
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/plain";

                Stream output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);

                output.Close();
            }
        }
    }
    private async Task SendResponse(HttpListenerRequest request, HttpListenerResponse response, Func<string, Task<List<MessageObj>>> func)
    {
        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            string requestBody = reader.ReadToEnd();
            Console.WriteLine($"Received request: {request.Url.AbsolutePath}  " + requestBody);

            ReceivedMessage message = JsonConvert.DeserializeObject<ReceivedMessage>(requestBody);
            if (message != null)
            {
                List<HttpMessage.Context> contexts = new List<HttpMessage.Context>();
                if (message.context == null)
                {
                    contexts.Add(new HttpMessage.Context { type = "string", content = "" });
                }
                else
                {
                    List<MessageObj> files = await func(message.context);
                    
                    if (files.Count == 0 || files == null)
                    {
                        HttpMessage.Context context = new HttpMessage.Context() { type = "string", content = "内部错误" };
                        contexts.Add(context);
                    }
                    else
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpMessage.Context context = new HttpMessage.Context();
                            context.type = files[i].type;
                            context.content = files[i].content;
                            contexts.Add(context);
                        }
                    }
                }

                HttpMessage backRespone = new HttpMessage() { context = contexts, from = "minori" };
                string responseString = JsonConvert.SerializeObject(backRespone);
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/plain";

                Stream output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);

                output.Close();
            }
        }
    }
    private async Task SendResponse(HttpListenerRequest request, HttpListenerResponse response,string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);

        response.ContentLength64 = buffer.Length;
        response.ContentType = "text/plain";

        Stream output = response.OutputStream;
        await output.WriteAsync(buffer, 0, buffer.Length);

        output.Close();
    }
    private class HttpMessage
    {
        public List<Context> context;
        public string from;
        public class Context
        {
            public string type;
            public string content;
        }
    }
    private class ReceivedMessage
    {
        public string type;
        public string context;
        public string from;
    }
}