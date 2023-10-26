using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.View
{
    public static class MusicList
    {
        public static async Task<string> DrawMusicList(List<SkMusics> musics)
        {
            Console.WriteLine($"正在生成歌曲列表");
            List<SKBitmap> all = new List<SKBitmap>();
            List<SKBitmap> info = new List<SKBitmap>();
            SKBitmap line = ImageCreater.DrawDottedLine(800, 5);

            //图片标题
            SKBitmap imageTitle = ImageCreater.DrawImageTitle("查询", "歌曲");
            all.Add(imageTitle);

            //歌曲列表
            for(int i =0;i<musics.Count;i++)
            {
                info.Add(await ImageCreater.DrawMusicLine(musics[i]));
                info.Add(line);
            }
            info.RemoveAt(info.Count - 1);

            //信息块
            SKBitmap infoBlock = ImageCreater.DrawInfoBlock(info,850);
            all.Add(infoBlock);

            //合成最终图片
            SKBitmap final = ImageCreater.DrawALL(all, 950);
            Console.WriteLine("歌曲列表生成完毕");

            return ImageCreater.ConvertBitmapToBase64(final);
        }
    }
}
