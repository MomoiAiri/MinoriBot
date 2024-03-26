using MinoriBot.Enums;
using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MinoriBot.Utils.View
{
    public static class Chart
    {
        static Dictionary<string, string> convertDiff = new Dictionary<string, string>() { { "expert", "exp" }, { "master", "mst" }, { "append", "apd" } };
        static string chartUri = "https://sdvx.in";

        public static async Task<MessageObj> DrawChart(SkMusics musics, string diffType)
        {
            Console.WriteLine($"开始绘制{musics.title}-{diffType}的谱面,资源地址{chartUri}");
            if (!convertDiff.ContainsKey(diffType))
            {
                return new MessageObj {type="string",content = "没有该难度的谱面" };
            }
            string html = string.Empty;
            HttpClient httpClient;
            if(Config.Instance().proxy && Config.Instance().proxyaddr != "")
            {
                HttpClientHandler hch = new HttpClientHandler();
                hch.Proxy = new WebProxy(Config.Instance().proxyaddr);
                httpClient = new HttpClient(hch);
            }
            else
            {
                httpClient= new HttpClient();
            }
            html = await httpClient.GetStringAsync("https://sdvx.in/prsk.html");
            string pattern = @$"<script\s+src=""/prsk/js/(\d+)sort\.js"">.*</script><!--{musics.title}-->";
            Match match = Regex.Match(html, pattern);
            string chartId = string.Empty;
            if (match.Success)
            {
                chartId = match.Groups[1].Value;
                Console.WriteLine(chartId);
            }
            else
            {
                return new MessageObj { type = "string", content = "没有查找到该歌曲的谱面" };
            }
            SKBitmap bg = new SKBitmap();
            SKBitmap chart = new SKBitmap();
            SKBitmap bar = new SKBitmap();
            try
            {
                byte[] bbg = await httpClient.GetByteArrayAsync($"{chartUri}/prsk/bg/{chartId}bg.png");
                bg = SKBitmap.Decode(new SKMemoryStream(bbg));

                byte[] bchart = await httpClient.GetByteArrayAsync($"{chartUri}/prsk/obj/data{chartId}{convertDiff[diffType]}.png");
                chart = SKBitmap.Decode(new SKMemoryStream(bchart));

                byte[] bbar = await httpClient.GetByteArrayAsync($"{chartUri}/prsk/bg/{chartId}bar.png");
                bar = SKBitmap.Decode(new SKMemoryStream(bbar));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new MessageObj { type = "string", content = "内部错误" };
            }
            httpClient.Dispose();
            if (bg.Width != 0 & chart.Width != 0 && bar.Width != 0)
            {
                int width = chart.Width;
                int height = chart.Height;
                SKBitmap final = new SKBitmap(width, height);
                using (SKCanvas canvas = new SKCanvas(final))
                {
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.Color = SKColors.Black;
                        paint.Style = SKPaintStyle.Fill;
                        canvas.DrawRect(0, 0, width, height, paint);
                    }
                    canvas.DrawBitmap(bg, 0, 0);
                    canvas.DrawBitmap(bar, 0, 0);
                    canvas.DrawBitmap(chart, 0, 0);
                }
                Console.WriteLine("谱面绘制完毕");

                MessageObj ml = new MessageObj() { type = "image", content = ImageCreater.ConvertBitmapToBase64(final) };

                return ml;
            }

            return new MessageObj { type = "string", content = "内部错误" }; ;
        }
    }
}
