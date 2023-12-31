﻿using Newtonsoft.Json;
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
            StringBuilder Add = new StringBuilder();
            promts = ChangeAzurLaneLora(promts,Add);


            if (ContainChinese(promts))
            {
                //return;
                promts = await PicPromtsProcessing.Instance.GetRespond(promts);
            }
            promts += ",best quality,highly detailed,masterpiece,ultra-detailed,illustration,";
            promts += Add.ToString();

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

        private static string ChangeAzurLaneLora(string promts,StringBuilder add)
        {
            add.Clear();
            foreach (string origin in Lora.Keys)
            {
                if (promts.Contains(origin))
                {
                    add.Append(Lora[origin]);
                    promts.Replace(origin, "");
                }
            }
            return promts;
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
        static Dictionary<string,string> Lora = new Dictionary<string, string>() 
        {
            {"塔什干","<lora:塔什干常服:0.6>,1girl,"},
            {"线稿","<lora:animeoutlineV4_16:0.6>,"},
            {"绫波","<lora:Ayanami_AzurLan:0.6>,kimono, silver hair, hair_ornament, off_shoulder, navel,"},
            {"恶毒","<lora:LeMalin_V7_8dim_e6:0.6>,LeMalinDefault,"},
            {"能代","<lora:Azur Lane-noshiro fragrance of the eastern snow:0.6>,noshirodress,"},
            {"企业"," <lora:Enterprisev10:0.6>,OriginalOutfit, military uniform,"},
            {"埃塞克斯","<lora:EssexChameleonAI_v1.0:0.6>,essex,"},
            {"光辉","<lora:illustrious (maiden lily's radiance):0.6>,illustrious (maiden lily's radiance) (azur lane),"},
            {"新泽西","<lora:new_jersey-origin-bunny-wedding:0.6>,new_jersey,wedding,"},
            {"普利茅斯","<lora:plmtazln_v25e1:0.6>,plmtazln,"},

        };
    }
}
