using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.View
{
    public static class EventDetail
    {
        public static async Task<MessageObj> DrawEventDetail(SkEvents skEvent)
        {
            Console.WriteLine($"正在生成活动ID为{skEvent.id}的信息");
            List<SKBitmap> all = new List<SKBitmap>();
            List<SKBitmap> info = new List<SKBitmap>();
            SKBitmap line = ImageCreater.DrawDottedLine(800, 30);

            //图片标题
            SKBitmap imageTitle = ImageCreater.DrawImageTitle("查询", "活动");
            all.Add(imageTitle);

            //活动Banner
            SKBitmap banner = await ImageCreater.DrawEventBanner(skEvent);
            info.Add(banner);
            info.Add(new SKBitmap(800, 30));

            //活动名称
            SKBitmap eventName = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "活动名称", text = skEvent.name });
            info.Add(eventName);
            info.Add(line);

            //类型  ID
            SKBitmap eventType = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "ID", text = skEvent.id.ToString() });
            SKBitmap eventId = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "类型", text = skEvent.GetEventType() });
            info.Add(ImageCreater.FixTwoTitleInOneLine(eventType, eventId));
            info.Add(line);

            //开始时间
            SKBitmap startAt = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "开始时间", text = Utils.TimeStampToDateTime(skEvent.startAt).ToString("yyyy年MM月dd日 HH:mm") });
            info.Add(startAt);
            info.Add(line);

            //结束时间
            SKBitmap endAt = ImageCreater.DrawTitleWithText(new ImageCreater.ListConfig() { title = "结束时间", text = Utils.TimeStampToDateTime(skEvent.aggregateAt).ToString("yyyy年MM月dd日 HH:mm") });
            info.Add(endAt);
            info.Add(line);

            //活动属性加成
            if (skEvent.eventType != "world_bloom")
            {
                SKBitmap eventAttr = ImageCreater.DrawBunusImage(skEvent.GetBunusAttr(), null, skEvent.GetBunusAttRate());
                SKBitmap bunusAttr = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "活动属性加成", images = new List<SKBitmap>() { eventAttr } });
                info.Add(bunusAttr);
                info.Add(line);
            }

            //活动角色加成
            SKBitmap eventCharacters = ImageCreater.DrawBunusImage(null, skEvent.GetBunusCharacters(), skEvent.GetBunusCharacterRate());
            SKBitmap bunusCharacters = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "活动角色加成", images = new List<SKBitmap>() { eventCharacters } });
            info.Add(bunusCharacters);
            info.Add(line);

            //活动奖励
            List<SKBitmap> degreeHonors = new List<SKBitmap>();
            List<int> degreesRank = new List<int>() { 1, 2, 3, 10, 100, 1000 };
            SKBitmap degreeMain = await skEvent.GetEventDegreeMain();
            for (int i =0;i<degreesRank.Count; i++)
            {
                degreeHonors.Add(ImageCreater.FixImageSize(ImageCreater.DrawDegreeHonor(degreeMain, degreesRank[i]), 230, 48));
            }
            SKBitmap eventDegree = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "活动奖励", images = degreeHonors });
            info.Add(eventDegree);
            info.Add(line);

            //奖励卡牌
            List<SKBitmap> rewardCardsImages = new List<SKBitmap>();
            List<SkCard> cards = skEvent.GetCurrentCards();
            for(int i = 0; i < cards.Count; i++)
            {
                if (cards[i].GetStarsCount() < 4)
                {
                    rewardCardsImages.Add(await ImageCreater.DrawCardIcon(cards[i], true, true));
                }
            }
            SKBitmap eventRewardCards = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "奖励卡牌", images = rewardCardsImages });
            info.Add(eventRewardCards);
            info.Add(line);

            //卡池卡牌
            List<SKBitmap> gachaCardsImages = new List<SKBitmap>();
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].GetStarsCount() == 4)
                {
                    gachaCardsImages.Add(await ImageCreater.DrawCardIcon(cards[i], true, true));
                }
            }
            SKBitmap eventGachaCards = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "活动期间卡池卡牌", images = gachaCardsImages });
            info.Add(eventGachaCards);
            info.Add(line);

            //活动相关歌曲
            List<SkMusics> eventMusics = skEvent.GetEventMusics();
            if (eventMusics.Count > 0)
            {
                SKBitmap musicList = await ImageCreater.DrawMusicList(eventMusics);
                SKBitmap eventMusicList = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "活动相关歌曲", images = new List<SKBitmap>() { musicList } });
                info.Add(eventMusicList);
                info.Add(line);
            }

            //活动相关卡池
            List<SkGachas> eventGachas = skEvent.GetGachasInEvent();
            if (eventGachas.Count > 0)
            {
                List<SKBitmap> gachaImages = new List<SKBitmap>();
                for (int i = 0; i < eventGachas.Count; i++)
                {
                    gachaImages.Add(await ImageCreater.DrawGachaCard(eventGachas[i]));
                }
                SKBitmap gachas = ImageCreater.DrawTitleWithImage(new ImageCreater.ListConfig() { title = "活动相关卡池", images = gachaImages });
                info.Add(gachas);
            }

            //信息块
            SKBitmap infoBlock = ImageCreater.DrawInfoBlock(info);
            all.Add(infoBlock);

            //最终图片
            SKBitmap final = ImageCreater.DrawALL(all);
            Console.WriteLine("活动信息生成完毕");

            MessageObj ml = new MessageObj() { type = "image", content = ImageCreater.ConvertBitmapToBase64(final) };

            return ml;
        }
    }
}
