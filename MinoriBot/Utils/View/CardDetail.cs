using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.View
{
    public static class CardDetail
    {
        public static async Task<string> DrawCardDetail(SkCard card)
        {
            Console.WriteLine($"正在生成卡面ID为{card.id}的信息");
            List<SKBitmap> all = new List<SKBitmap>();
            SKBitmap line = ImageCreater.DrawDottedLine(800, 30);
            int starCount = card.GetStarsCount();
            string cardLimitType;
            List<SkGachas> gachas = card.GetGachas(out cardLimitType);

            //标题
            SKBitmap title = ImageCreater.DrawCardTitle(card);
            all.Add(title);
            all.Add(new SKBitmap(800, 20));

            //插图
            SKBitmap illustration = await ImageCreater.DrawCardIllustrationImage(card, false);
            all.Add(illustration);
            if (starCount > 2)
            {
                all.Add(new SKBitmap(800, 20));
                SKBitmap illustration_afterTraining = await ImageCreater.DrawCardIllustrationImage(card, true);
                all.Add(illustration_afterTraining);
            }
            all.Add(new SKBitmap(800, 20));

            //编号 类型
            SKBitmap cardId = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "ID", text = card.id.ToString() });
            SKBitmap cardType = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "类型", text = cardLimitType });
            all.Add(ImageCreater.FixTwoTitleInOneLine(cardId, cardType));
            all.Add(line);

            //综合力
            SKBitmap power = ImageCreater.DrawCardPower(card);
            all.Add(power);
            all.Add(line);

            //技能
            SKBitmap skill = ImageCreater.DrawCardSkill(card);
            all.Add(skill);
            all.Add(line);

            //招募语
            SKBitmap gachaPhrase = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "招募语", text = card.gachaPhrase });
            all.Add(gachaPhrase);
            all.Add(line);

            //发布日期
            SKBitmap releaseTime = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "发布日期", text = Utils.TimeStampToDateTime(card.releaseAt).ToString("yyyy年MM月dd日 HH:mm") });
            all.Add(releaseTime);
            all.Add(line);

            //缩略图
            List<SKBitmap> icons = new List<SKBitmap>();
            icons.Add(await ImageCreater.DrawCardIcon(card, false, false));
            if (starCount > 2)
            {
                icons.Add(await ImageCreater.DrawCardIcon(card, true, false));
            }
            SKBitmap cardIcons = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "缩略图", images = icons });
            all.Add(cardIcons);
            all.Add(line);

            //相关活动
            SKBitmap relatedEvent = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "相关活动", images = new List<SKBitmap> { await ImageCreater.DrawEventLogo(card.GetEvent(), true) } });
            all.Add(relatedEvent);
            all.Add(line);

            //相关卡池
            List<SKBitmap> gachaBanners = new List<SKBitmap>();
            for(int i = 0; i < gachas.Count; i++)
            {
                gachaBanners.Add(await ImageCreater.DrawGachaCard(gachas[i]));
            }
            SKBitmap cardGachas = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "相关卡池", images = gachaBanners });
            all.Add(cardGachas);

            //信息合成
            SKBitmap final = ImageCreater.DrawAllInfo(all);


            Console.WriteLine("卡面信息生成完毕");

            return ImageCreater.ConvertBitmapToBase64(final);
        }
    }
}
