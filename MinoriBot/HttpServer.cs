

using System.Net;
using System.Text;

public class HttpServer
{

    HttpListener listener = new HttpListener();

    public HttpServer(int port)
    {
        listener.Prefixes.Add($"http://127.0.0.1:{port}/");
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
                using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = reader.ReadToEnd();
                    Console.WriteLine("Received POST request with body: " + requestBody);
                    byte[] responseBytes = Encoding.UTF8.GetBytes("Received POST request");
                    response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
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