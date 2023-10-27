

using MinoriBot.Enums;
using MinoriBot.Utils.Routers;
using Newtonsoft.Json;
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
        Console.WriteLine("Http服务已启用");
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
    private async Task SendResponse(HttpListenerRequest request,HttpListenerResponse response,Func<string,Task<string>> func)
    {
        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            string requestBody = reader.ReadToEnd();
            Console.WriteLine("Received request: " + requestBody);

            HttpMessage message = JsonConvert.DeserializeObject<HttpMessage>(requestBody);
            if (message != null)
            {
                string file = await func(message.context);
                string msgType = "image";
                if (file == "内部错误" || file=="none" || file=="error")
                {
                    msgType = "text";
                }
                HttpMessage backRespone = new HttpMessage() { type = msgType,context = file ,from ="minori"}; 
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
        public string type;
        public string context;
        public string from;
    }
}