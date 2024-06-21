using MinoriBot.Utils.StaticFilesLoader;
using MinoriBot.Utils.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.Routers
{
    internal class SearchGacha
    {
        public static async Task<List<MessageObj>> SearchSkGacha(string message)
        {
            List<MessageObj> result = new List<MessageObj>();
            if (int.TryParse(message, out int gachaId))
            {
                bool isFound = false;
                for (int i = 0; i < SkDataBase.skGachas.Count; i++)
                {
                    if (SkDataBase.skGachas[i].id == gachaId)
                    {
                        isFound = true;
                        result.Add(await GachaDetail.DrawGachaDetail(SkDataBase.skGachas[i]));
                    }
                }
                if (!isFound)
                {
                    result.Add(new MessageObj { type = "string", content = "未找到该卡池" });
                }
            }
            return result;
        }
    }
}
