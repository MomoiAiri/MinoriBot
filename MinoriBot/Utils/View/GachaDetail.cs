using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.View
{
    public static class GachaDetail
    {
        public static async Task<MessageObj> DrawGachaDetail(SkGachas gacha)
        {
            Console.WriteLine($"正在生成卡池ID为{gacha.id}的信息");
            List<SKBitmap> all = new List<SKBitmap>();
            List<SKBitmap> info = new List<SKBitmap>();
            SKBitmap line = ImageCreater.DrawDottedLine(800, 30);

            //图片标题
            SKBitmap imageTitle = ImageCreater.DrawImageTitle("查询", "卡池");
            all.Add(imageTitle);

            //卡池Banner
            SKBitmap banner = ImageCreater.ResizeImage(await gacha.GetGachaBanner(), 800);
            info.Add(banner);
            info.Add(new SKBitmap(800, 30));

            //卡池名称
            SKBitmap gachaName = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "卡池名称", text = gacha.name });
            info.Add(gachaName);
            info.Add(line);

            //类型  ID
            SKBitmap gachaType = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "ID", text = gacha.id.ToString() });
            SKBitmap gachaId = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "类型", text = gacha.gachaType });
            info.Add(ImageCreater.FixTwoTitleInOneLine(gachaType, gachaId));
            info.Add(line);

            //开始时间
            SKBitmap startAt = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "开始时间", text = Utils.TimeStampToDateTime(gacha.startAt).ToString("yyyy年MM月dd日 HH:mm") });
            info.Add(startAt);
            info.Add(line);

            //结束时间
            SKBitmap endAt = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "结束时间", text = Utils.TimeStampToDateTime(gacha.endAt).ToString("yyyy年MM月dd日 HH:mm") });
            info.Add(endAt);
            info.Add(line);

            //出现概率
            List<SKBitmap> rates = new List<SKBitmap>();
            for(int i = 0; i < gacha.gachaCardRarityRates.Count; i++)
            {
                rates.Add(ImageCreater.DrawGachaRarityRate(gacha.gachaCardRarityRates[i].cardRarityType, GetRateString(gacha.gachaCardRarityRates[i].rate)));
            }
            SKBitmap rate = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "出现概率", images = rates });
            info.Add(rate);
            info.Add(line);

            //PickUp卡牌
            if(gacha.gachaPickups.Count > 0)
            {
                List<SkCard> pickupCards = new List<SkCard>();
                List<int> pickupIds = gacha.gachaPickups.Select(card=>card.cardId).ToList();
                pickupCards = SkDataBase.skCards.Where(card => pickupIds.Contains(card.id)).ToList();
                List<SKBitmap> cardIcons = new List<SKBitmap>();
                for(int i =0;i< pickupCards.Count;i++)
                {
                    cardIcons.Add(await ImageCreater.DrawCardIcon(pickupCards[i], false, true));
                }
                SKBitmap pickup = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "卡池pickup", images = cardIcons });
                info.Add(pickup);
            }

            SKBitmap infoBlock= ImageCreater.DrawInfoBlock(info);
            all.Add(infoBlock);

            SKBitmap final = ImageCreater.DrawALL(all);
            Console.WriteLine("卡池信息生成完毕");

            MessageObj ml = new MessageObj() {type="image",content =ImageCreater.ConvertBitmapToBase64(final) };

            return ml;
        }

        static string GetRateString(double rate)
        {
            if (rate % 1 == 0)
            {
                return $"+{(int)rate}%";
            }
            else
            {
                return $"+{rate:0.0}%";
            }
        }
    }
    
}
