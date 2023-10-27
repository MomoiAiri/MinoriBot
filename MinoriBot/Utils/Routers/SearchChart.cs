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
        public static async Task<string> SearchSkMusicChart(string message)
        {
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
                            return await Chart.DrawChart(SkDataBase.skMusics[i], NickName.diffType[diffType]);
                        }
                    }
                    if (!isFound)
                    {
                        return "none";
                    }
                }
            }
            else
            {
                return "lack";
            }
            return "error";
        }
    }
}
