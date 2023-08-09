using MinoriBot.Utils.StaticFilesLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot
{
    internal static class Init
    {
        public static async Task Start()
        {
            await SekaiImageDownload.InitNormalImage();
            await SkDataBase.Start();
        }
    }
}
