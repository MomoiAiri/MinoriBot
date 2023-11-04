using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.View
{
    public static class MusicDetail
    {
        public static async Task<MessageObj> DrawMusicDetails(SkMusics music)
        {
            Console.WriteLine($"正在生成歌曲ID为{music.id}的信息");
            List<SKBitmap> all = new List<SKBitmap>();
            List<SKBitmap> info = new List<SKBitmap>();
            SKBitmap line = ImageCreater.DrawDottedLine(800, 30);

            //图片标题
            SKBitmap imageTitle = ImageCreater.DrawImageTitle("查询", "歌曲");
            all.Add(imageTitle);

            //歌曲马甲
            SKBitmap musicFace = await ImageCreater.DrawMusicFace(music);
            info.Add(musicFace);
            info.Add(new SKBitmap(800, 30));

            //歌曲名
            SKBitmap musicName = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "歌曲名称", text = music.title });
            info.Add(musicName);
            info.Add(line);

            //ID
            SKBitmap musicID = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "ID", text = music.id.ToString() });
            info.Add(musicID);
            info.Add(line);

            //作词
            SKBitmap lyricist = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "作词", text = music.lyricist });
            info.Add(lyricist);
            info.Add(line);

            //作曲
            SKBitmap composer = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "作曲", text = music.composer });
            info.Add(composer);
            info.Add(line);

            //编曲
            SKBitmap arranger = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "编曲", text = music.arranger });
            info.Add(arranger);
            info.Add(line);

            //开放时间
            SKBitmap publishedAt = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "开放时间", text = Utils.TimeStampToDateTime(music.publishedAt).ToString("yyyy年MM月dd日 HH:mm:ss") });
            info.Add(publishedAt);
            info.Add(line);

            //信息块
            SKBitmap infoBlock = ImageCreater.DrawInfoBlock(info);
            all.Add(infoBlock);

            //合成最终图片
            SKBitmap final = ImageCreater.DrawALL(all);
            Console.WriteLine("歌曲信息生成完毕");
                
            MessageObj ml = new MessageObj() { type = "image", content = ImageCreater.ConvertBitmapToBase64(final) };

            return ml;
        }
    }
}
