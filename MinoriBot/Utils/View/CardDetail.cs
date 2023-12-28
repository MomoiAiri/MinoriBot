using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.View
{
    public static class CardDetail
    {
        public static async Task<MessageObj> DrawCardDetail(SkCard card)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"正在生成卡面ID为{card.id}的信息");
            List<SKBitmap> all = new List<SKBitmap>();
            List<SKBitmap> info = new List<SKBitmap>(); 
            SKBitmap line = ImageCreater.DrawDottedLine(800, 30);
            int starCount = card.GetStarsCount();
            string cardLimitType;
            List<SkGachas> gachas = card.GetGachas(out cardLimitType);

            //图片标题
            SKBitmap imageTitle = ImageCreater.DrawImageTitle("查询", "卡牌");
            all.Add(imageTitle);

            //标题
            SKBitmap title = ImageCreater.DrawCardTitle(card);
            info.Add(title);
            info.Add(new SKBitmap(800, 20));

            //插图
            SKBitmap illustration = await ImageCreater.DrawCardIllustrationImage(card, false);
            info.Add(illustration);
            if (starCount > 2)
            {
                info.Add(new SKBitmap(800, 20));
                SKBitmap illustration_afterTraining = await ImageCreater.DrawCardIllustrationImage(card, true);
                info.Add(illustration_afterTraining);
            }
            info.Add(new SKBitmap(800, 20));

            //编号 类型
            SKBitmap cardId = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "ID", text = card.id.ToString() });
            SKBitmap cardType = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "类型", text = cardLimitType });
            info.Add(ImageCreater.FixTwoTitleInOneLine(cardId, cardType));
            info.Add(line);

            //综合力
            SKBitmap power = ImageCreater.DrawCardPower(card);
            info.Add(power);
            info.Add(line);

            //技能
            SKBitmap skill = ImageCreater.DrawCardSkill(card);
            info.Add(skill);
            info.Add(line);

            //招募语
            SKBitmap gachaPhrase = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "招募语", text = card.gachaPhrase });
            info.Add(gachaPhrase);
            info.Add(line);

            //发布日期
            SKBitmap releaseTime = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "发布日期", text = Utils.TimeStampToDateTime(card.releaseAt).ToString("yyyy年MM月dd日 HH:mm") });
            info.Add(releaseTime);
            info.Add(line);

            //缩略图 + (附属团)
            List<SKBitmap> icons = new List<SKBitmap>();
            icons.Add(await ImageCreater.DrawCardIcon(card, false, false));
            if (starCount > 2)
            {
                icons.Add(await ImageCreater.DrawCardIcon(card, true, false));
            }
            SKBitmap cardIcons = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "缩略图", images = icons });
            if (card.supportUnit != "none")
            {
                SKBitmap tempLogo = SKBitmap.Decode($"./asset/normal/logo_{card.supportUnit}.png");
                SKBitmap supportUnitLogo = ImageCreater.ReSizeImage(tempLogo,300,0);
                SKBitmap supportUnit = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "附属团", images = new List<SKBitmap> { supportUnitLogo } });
                SKBitmap tempimage = ImageCreater.FixTwoTitleInOneLine(cardIcons, supportUnit);
                info.Add(tempimage);
                info.Add(line);
            }
            else
            {
                info.Add(cardIcons);
                info.Add(line);
            }

            //相关活动
            SkEvents skEvent = card.GetEvent();
            if (skEvent != null)
            {
                SKBitmap relatedEvent = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "相关活动", images = new List<SKBitmap> { await ImageCreater.DrawEventLogo(skEvent, true) } });
                info.Add(relatedEvent);
                info.Add(line);
            }

            //相关卡池
            if (gachas.Count > 0)
            {
                List<SKBitmap> gachaBanners = new List<SKBitmap>();
                for (int i = 0; i < gachas.Count; i++)
                {
                    gachaBanners.Add(await ImageCreater.DrawGachaCard(gachas[i]));
                }
                SKBitmap cardGachas = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "相关卡池", images = gachaBanners });
                info.Add(cardGachas);
            }

            //信息块合成
            SKBitmap infoBlock = ImageCreater.DrawInfoBlock(info);

            all.Add(infoBlock);

            //最终图片
            SKBitmap final = ImageCreater.DrawALL(all);

            Console.WriteLine("卡面信息生成完毕");
            stopwatch.Stop();
            Console.WriteLine($"绘制完成，花费时间{stopwatch.ElapsedMilliseconds}ms");

            MessageObj ml = new MessageObj() {type = "image",content = ImageCreater.ConvertBitmapToBase64(final) };

            return ml;
        }
    }
}
