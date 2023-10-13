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
        public static async Task<string> SearchSkGacha(string message)
        {
            if (int.TryParse(message, out int gachaId))
            {
                bool isFound = false;
                for (int i = 0; i < SkDataBase.skGachas.Count; i++)
                {
                    if (SkDataBase.skGachas[i].id == gachaId)
                    {
                        isFound = true;

                        return await ImageCreater.DrawGachaInfo(SkDataBase.skGachas[i]);
                    }
                }
                if (!isFound)
                {
                    return "error";
                }
            }
            return "error";
        }
    }
}
