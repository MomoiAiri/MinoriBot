﻿using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.View
{
    public static class EventList
    {
        public static async Task<MessageObj> DrawEventList(List<SkEvents> skEvents)
        {
            Console.WriteLine($"正在生成活动列表");
            List<SKBitmap> all = new List<SKBitmap>();
            List<SKBitmap> info = new List<SKBitmap>();
            SKBitmap line = ImageCreater.DrawDottedLine(800, 20);

            //图片标题
            SKBitmap imageTitle = ImageCreater.DrawImageTitle("查询", "活动");
            all.Add(imageTitle);

            //活动列表
            for(int i =0;i<skEvents.Count;i++)
            {
                info.Add(await ImageCreater.DrawSimpleEventImage(skEvents[i]));
                info.Add(line);
            }
            info.RemoveAt(info.Count - 1);

            //信息块
            SKBitmap infoBlock = ImageCreater.DrawInfoBlock(info);
            all.Add(infoBlock);

            //最终图片
            SKBitmap final = ImageCreater.DrawALL(all);
            Console.WriteLine("活动列表生成完毕");

            MessageObj ml = new MessageObj() { type = "image", content = ImageCreater.ConvertBitmapToBase64(final) };

            return ml;
        }
    }
}
