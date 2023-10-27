

using MinoriBot.Utils.Routers;
using System.Net;
using System.Text;

public class HttpServer
{

    HttpListener listener = new HttpListener();
    private string[] prefixs = new string[]
    {
        "http://127.0.0.1:8080/test/",
        "http://127.0.0.1:8080/music/"
    };
    public HttpServer(int port)
    {
        for(int i =0;i<prefixs.Length;i++)
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
                    using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = reader.ReadToEnd();
                        Console.WriteLine("Received request: " + requestBody);

                        string responseString = "Hello, Test!";
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                        response.ContentLength64 = buffer.Length;
                        response.ContentType = "text/plain";

                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);

                        output.Close();
                    }
                    
                }
                else if (requestPath.StartsWith("/music"))
                {
                    using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = reader.ReadToEnd();
                        Console.WriteLine("Received request: " + requestBody);

                        string responseString = await SearchMusic.SearchSkMusics(requestBody);
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                        response.ContentLength64 = buffer.Length;
                        response.ContentType = "text/plain";

                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);

                        output.Close();
                    }
                }
            }
            else if (request.HttpMethod == "GET")
            {
                byte[] responseBytes = Encoding.UTF8.GetBytes("Hello, GET request!");
                response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            }
        }
    }
}