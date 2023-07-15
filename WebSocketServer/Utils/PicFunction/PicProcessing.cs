using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MinoriBot.Utils.ChatFunction;
//using static System.Net.Mime.MediaTypeNames;

namespace MinoriBot.Utils.PicFunction
{
    internal static class PicProcessing
    {
        public static async Task Generate(string promts)
        {
            string url = "http://pancake.shinonomeena.icu:10386";

            if (ContainChinese(promts))
            {
                return;
                promts = await PicPromtsProcessing.Instance.GetRespond(promts);
            }
            promts += ",best quality,highly detailed,masterpiece,ultra-detailed,illustration,";


            var payload = new
            {
                CLIP_stop_at_last_layers = 2,
                width = 512,
                height = 728,

                //prompt = "1girl, hat, blue eyes, one eye closed, smile, fur trim, dress, beret, side ponytail, capelet, virtual youtuber, brown hair, black dress, triangle, white headwear, looking at viewer, solo, ribbon, green background, bangs, fur-trimmed capelet, bow, striped, gloves, coat, holding, long sleeves, closed mouth, white capelet, circle",
                prompt = promts,
                negative_prompt = "badhandv4,EasyNegative,verybadimagenegative_v1.3",
                sampler_index = "DPM++ 2M Karras",
                cfg_scale = 7,
                steps = 20
            };
            using (HttpClient client = new HttpClient())
            {
                // Send POST request to txt2img endpoint
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var stringContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{url}/sdapi/v1/txt2img", stringContent);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var r = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                foreach (var imageBase64 in r.images)
                {
                    // Convert Base64 image to byte array
                    byte[] imageBytes = Convert.FromBase64String(imageBase64.ToString().Split(",")[0]);

                    // Create Image object from byte array
                    using (var stream = new MemoryStream(imageBytes))
                    using (var image = Image.FromStream(stream))
                    {

                        var pngPayload = new
                        {
                            image = $"data:image/png;base64,{imageBase64}"
                        };

                        // 发送POST请求到png-info端点
                        jsonPayload = JsonConvert.SerializeObject(pngPayload);
                        stringContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                        var response2 = await client.PostAsync($"{url}/sdapi/v1/png-info", stringContent);
                        var jsonResponse2 = await response2.Content.ReadAsStringAsync();
                        var pngInfo = JsonConvert.DeserializeObject<dynamic>(jsonResponse2);

                        // 保存带有PNG元数据的图像
                        using (var output = File.OpenWrite("output.png"))
                        {
                            var pngMetadata = image.PropertyItems[0];
                            pngMetadata.Value = Encoding.ASCII.GetBytes($"parameters={pngInfo.info}");
                            image.SetPropertyItem(pngMetadata);
                            image.Save(output, image.RawFormat);
                        }
                    }
                }
            }
        }

        private static bool ContainChinese(string promts)
        {
            foreach (char c in promts)
            {
                if(c >= '\u4e00' && c <= '\u9fff')
                {
                    return true;
                }
            }
            return false;
        }
    }
}
