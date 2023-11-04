using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using MinoriBot.Utils.View;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MinoriBot.Utils.Routers
{
    public static class SearchChart
    {
        public static async Task<List<MessageObj>> SearchSkMusicChart(string message)
        {
            List<MessageObj> result = new List<MessageObj>();
            string[] key = message.Split(' ');
            if (key.Length == 2)
            {
                string diffType = key[1];
                if (int.TryParse(key[0], out int musicId) && NickName.diffType.ContainsKey(diffType))
                {
                    bool isFound = false;
                    for (int i = 0; i < SkDataBase.skMusics.Count; i++)
                    {
                        if (SkDataBase.skMusics[i].id == musicId)
                        {
                            isFound = true;
                            result.Add(await Chart.DrawChart(SkDataBase.skMusics[i], NickName.diffType[diffType]));
                            return result;
                        }
                    }
                    if (!isFound)
                    {
                        result.Add(new MessageObj() { type = "string", content = "没有查找到该ID的谱面" });
                        return result;
                    }
                }
            }
            else
            {
                result.Add(new MessageObj() { type = "string", content = "缺少关键词" });
                return result;
            }
            result.Add(new MessageObj() { type = "string", content = "内部错误" });
            return result;  
        }
    }
}
