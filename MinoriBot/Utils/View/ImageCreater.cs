using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace MinoriBot.Utils.View
{
    public static class ImageCreater
    {
        static SKTypeface _typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf");
        static SKPaint _highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
        /// <summary>
        /// 画查卡列表
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="isTrained"></param>
        /// <returns></returns>
        public static async Task<SKBitmap> DrawCardIconList(Dictionary<int, Dictionary<string, List<SkCard>>> classifiedCards, bool isTrained)
        {
            string fileDirectory = "./asset/normal";
            //string result = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+".png";
            int line = 0;//输出图片的行数
            int maxLineCount = 0;
            foreach (var kvp in classifiedCards)
            {
                Console.WriteLine($"ID: {kvp.Key}");
                foreach (var attrKvp in kvp.Value)
                {
                    Console.WriteLine($"  Attr: {attrKvp.Key}");
                    //对分类好的卡组进行星级排序，高星在前
                    attrKvp.Value.Sort((card1, card2) => card2.GetStarsCount().CompareTo(card1.GetStarsCount()));
                    line++;
                    int countTemp = 0;
                    foreach (var card in attrKvp.Value)
                    {
                        Console.WriteLine($"    Name: {card.prefix}_{card.GetStarsCount()}星");
                        countTemp++;
                    }
                    if (countTemp > maxLineCount)
                    {
                        maxLineCount = countTemp;
                    }
                }
            }
            Console.WriteLine($"需要输出{line}行图片，最长的一行有{maxLineCount}张卡");
            int width = 148 + (156 + 15) * maxLineCount;
            int height = (180 + 15) * line - 15;
            Console.WriteLine("开始生成图片");
            SKBitmap cardList = new SKBitmap(width, height);
            using (SKCanvas canvas = new SKCanvas(cardList))
            {
                int x = 0;
                int y = 0;
                foreach (var kvp in classifiedCards)
                {
                    //画角色头图
                    canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/{kvp.Key}.png"), new SKRect(x, y, x + 128, y + 128));
                    x = 148;
                    foreach (var attrKvp in kvp.Value)
                    {
                        foreach (var card in attrKvp.Value)
                        {
                            canvas.DrawBitmap(await DrawCardIcon(card, isTrained, true), new SKRect(x, y, x + 156, y + 180));
                            x = x + 156 + 15;
                        }
                        x = 148;
                        y = y + 180 + 15;
                    }
                    x = 0;
                }

            }
            return cardList;
        }
        //public static async Task<string> DrawCardInfo(SkCard card)
        //{
        //    int width = 1000;
        //    int height = 0;
        //    int cardIllustrationImageCount = 0;
        //    Console.WriteLine($"正在生成卡面ID为{card.id}的图片");
        //    string assetDir = "./asset/normal";
        //    string logoDir = assetDir + $"/logo_{card.GetGroupNameById()}.png";
        //    string cardFrameDir = string.Empty;
        //    string cardType = string.Empty;
        //    int starCount = card.GetStarsCount();
        //    if (starCount > 0)
        //    {
        //        cardFrameDir = assetDir + $"/cardFrame_{starCount}.png";
        //    }
        //    else
        //    {
        //        cardFrameDir = assetDir + "/cardFrame_bd.png";
        //    }
        //    cardIllustrationImageCount = starCount > 2 ? 2 : 1;
        //    //计算卡面信息所需要的图片高度
        //    string skillDescription = card.GetSkillDescription().Replace("\n", "");
        //    List<string> skillDescriptionList = SplitString(skillDescription, 30, 750);
        //    SkEvents skEvent = card.GetEvent();
        //    int hasEvent = skEvent == null ? 0 : 1;
        //    List<SkGachas> gachas = card.GetGachas(out cardType);
        //    int gachaHight = (gachas.Count / 2 + gachas.Count % 2) * 200;
        //    int hasGacha = gachas.Count > 0 ? 1 : 0;
        //    //200标题,450一张插图与空隙，150编号，360综合力，140+30*技能描述行数,130招募语，130发布日期，256缩略图，360*hasEvent是否有活动图
        //    height = 200 + 450 * cardIllustrationImageCount + 150 + 360 + 140 + 30 * skillDescriptionList.Count + 130 + 130 + 266 + 360 * hasEvent + 60 * hasGacha + gachaHight + 70 + 250;
        //    SKBitmap cardInfo = new SKBitmap(width, height);
        //    using (SKCanvas canvas = new SKCanvas(cardInfo))
        //    using (SKPaint font = new SKPaint() { Typeface = _typeface, IsAntialias = true })
        //    using (SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High })
        //    {
        //        DrawBackGroud(cardInfo, "卡牌");
        //        //x,y为当前所操作区块的左上角坐标
        //        int x = 100;
        //        int y = 250;
        //        //标题背景颜色填充
        //        using (SKPaint paint = new SKPaint())
        //        {
        //            paint.Color = new SKColor(235, 235, 235);
        //            paint.IsAntialias = true;
        //            paint.Style = SKPaintStyle.Fill;
        //            canvas.DrawRect(new SKRect(x, y, x + 800, y + 130), paint);
        //        }

        //        //团体logo
        //        SKBitmap dottedLine = DrawDottedLine(800, 5);
        //        SKBitmap logoImage = SKBitmap.Decode(logoDir);
        //        float logoWidth = 100f / logoImage.Height * logoImage.Width;
        //        canvas.DrawBitmap(logoImage, new SKRect(x + 70, y + 15, x + 70 + logoWidth, y + 115), highQuality);

        //        //卡牌名与角色名
        //        font.TextSize = 30;
        //        font.Color = SKColors.Black;
        //        font.TextAlign = SKTextAlign.Left;
        //        string text = card.prefix;
        //        canvas.DrawText(text, x + 350, y + 50, font);
        //        text = NickName.idToName[card.characterId];
        //        font.TextSize = 40;
        //        canvas.DrawText(text, x + 350, y + 100, font);

        //        //画卡面插图 卡面大小800*450 星星大小40*39
        //        y = 400;
        //        List<SKBitmap> cardIllustrationImage = await card.GetCardIllustrationImage();
        //        List<SKBitmap> cardIllustrationImage_afterCropping = new List<SKBitmap>();
        //        for (int i = 0; i < cardIllustrationImage.Count; i++)
        //        {
        //            cardIllustrationImage_afterCropping.Add(CropCardIllustrationImage(cardIllustrationImage[i]));
        //        }
        //        canvas.DrawBitmap(cardIllustrationImage_afterCropping[0], new SKRect(x, y, x + 800, y + 450), highQuality);
        //        canvas.DrawBitmap(SKBitmap.Decode(cardFrameDir), new SKRect(x, y, x + 800, y + 450), highQuality);
        //        canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{card.attr}.png"), new SKRect(x + 740, y, x + 800, y + 60), highQuality);
        //        SKBitmap normal_star = SKBitmap.Decode("./asset/normal/normal_star.png");
        //        //生日卡单独处理
        //        if (starCount == 0)
        //        {
        //            canvas.DrawBitmap(SKBitmap.Decode("./asset/normal/birthday_star.png"), new SKRect(x + 18, y + 393, x + 18 + 40, y + 393 + 39), highQuality);
        //            y = y + 450 + 20;
        //        }
        //        else
        //        {
        //            for (int i = 0; i < starCount; i++)
        //            {
        //                canvas.DrawBitmap(normal_star, new SKRect(x + 18, y + 393 - 39 * i, x + 18 + 40, y + 393 + 39 - 39 * i), highQuality);
        //            }
        //            y = y + 450 + 20;
        //            //如果有特训后
        //            if (cardIllustrationImage.Count > 1)
        //            {
        //                y = 400 + 450 + 20;
        //                SKBitmap afterTrainingStar = SKBitmap.Decode("./asset/normal/after_training_star.png");
        //                canvas.DrawBitmap(cardIllustrationImage_afterCropping[1], new SKRect(x, y, x + 800, y + 450), highQuality);
        //                canvas.DrawBitmap(SKBitmap.Decode(cardFrameDir), new SKRect(x, y, x + 800, y + 450), highQuality);
        //                canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{card.attr}.png"), new SKRect(x + 740, y, x + 800, y + 60), highQuality);
        //                for (int i = 0; i < starCount; i++)
        //                {
        //                    canvas.DrawBitmap(afterTrainingStar, new SKRect(x + 18, y + 393 - 39 * i, x + 18 + 40, y + 393 + 39 - 39 * i), highQuality);
        //                }
        //                y = 400 + 450 * 2 + 20 * 2;
        //            }
        //        }
        //        //卡面编号
        //        canvas.DrawBitmap(DrawPillShapeTitle("编号"), x, y);
        //        font.TextSize = 40;
        //        canvas.DrawText(card.id.ToString(), x + 25, y + 40 + 50, font);
        //        //类型
        //        canvas.DrawBitmap(DrawPillShapeTitle("类型"), x + 400, y);
        //        canvas.DrawText(cardType, x + 25 + 400, y + 40 + 50, font);
        //        //综合力
        //        y += 100 + 40;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("综合力"), x, y);
        //        font.TextSize = 45;
        //        int[] power = card.GetPower();
        //        canvas.DrawText($"综合力: {power[0] + power[1] + power[2]}  +  ({power[3] * 3 * 5})", x + 25, y + 50 + 45, font);
        //        font.TextSize = 30;
        //        using (SKPaint bar = new SKPaint())
        //        {
        //            bar.IsAntialias = true;
        //            bar.Color = new SKColor(144, 238, 144);
        //            canvas.DrawText($"表现力: {power[0]} + ({power[3] * 5})", x + 25, y + 95 + 30 + 10, font);
        //            canvas.DrawRoundRect(new SKRect(x + 25, y + 135 + 10, x + 25 + power[0] / 15000f * 750f, y + 145 + 30), 10, 10, bar);
        //            bar.Color = new SKColor(100, 149, 237);
        //            canvas.DrawText($"技术力: {power[1]} + ({power[3] * 5})", x + 25, y + 175 + 30, font);
        //            canvas.DrawRoundRect(new SKRect(x + 25, y + 205 + 10, x + 25 + power[1] / 15000f * 750f, y + 215 + 30), 10, 10, bar);
        //            bar.Color = new SKColor(147, 112, 219);
        //            canvas.DrawText($"活力: {power[2]} + ({power[3] * 5})", x + 25, y + 245 + 30, font);
        //            canvas.DrawRoundRect(new SKRect(x + 25, y + 275 + 10, x + 25 + power[2] / 15000f * 750f, y + 285 + 30), 10, 10, bar);
        //        }
        //        y += 315 + 40;
        //        //技能
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("技能"), x, y);
        //        font.TextSize = 40;
        //        canvas.DrawText(card.cardSkillname, x + 25, y + 50 + 40, font);
        //        y = y + 100;
        //        using (SKPaint paint = new SKPaint())
        //        {
        //            paint.Color = new SKColor(221, 221, 221);
        //            paint.IsAntialias = true;
        //            paint.Style = SKPaintStyle.Fill;
        //            canvas.DrawRect(new SKRect(x + 15, y + 5, x + 750 + 30, y + skillDescriptionList.Count * 30 + 10), paint);
        //        }
        //        font.TextSize = 30;
        //        for (int i = 0; i < skillDescriptionList.Count; i++)
        //        {
        //            canvas.DrawText(skillDescriptionList[i], x + 25, y + 30, font);
        //            y += 30;
        //        }
        //        font.TextSize = 40;
        //        y += 40;
        //        //招募语
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("招募语"), x, y);
        //        canvas.DrawText(card.gachaPhrase, x + 25, y + 50 + 40, font);
        //        y += 130;
        //        //发布日期
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("发布日期"), x, y);
        //        DateTime releaseTime = DateTimeOffset.FromUnixTimeMilliseconds(card.releaseAt).LocalDateTime;
        //        canvas.DrawText(releaseTime.ToString("yyyy年MM月dd日 HH:mm"), x + 25, y + 50 + 40, font);
        //        y += 130;
        //        //缩略图
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        //canvas.DrawBitmap(DrawPillShapeTitle("缩略图"), x, y);
        //        //canvas.DrawBitmap(await DrawCardIcon(card, false, false), x + 25, y + 40 + 20);
        //        //if (starCount > 2)
        //        //{
        //        //    canvas.DrawBitmap(await DrawCardIcon(card, true, false), x + 20 + 156 + 20, y + 40 + 20);
        //        //}
        //        List<SKBitmap> cardIcons = new List<SKBitmap>();
        //        cardIcons.Add(await DrawCardIcon(card, false, false));
        //        if (starCount > 2)
        //        {
        //            cardIcons.Add(await DrawCardIcon(card, true, false));
        //        }
        //        SKBitmap suoluetu = DrawTitleWithImage(new ListConfig() { title="缩略图", images = cardIcons });
        //        canvas.DrawBitmap(suoluetu, x, y);
        //        //附属团
        //        if (card.supportUnit != "none")
        //        {
        //            canvas.DrawBitmap(DrawPillShapeTitle("附属团"), x + 400, y);
        //            SKBitmap supportUnitLogo = SKBitmap.Decode($"./asset/normal/logo_{card.supportUnit}.png");
        //            float supportUnitLogoWith = 100f / supportUnitLogo.Height * supportUnitLogo.Width;
        //            canvas.DrawBitmap(supportUnitLogo, new SKRect(x + 400 + 25, y + 60, x + 400 + 25 + logoWidth, y + 160), highQuality);
        //        }
        //        y += 60 + 156 + 40;
        //        //相关活动
        //        if (skEvent != null)
        //        {
        //            canvas.DrawBitmap(dottedLine, x, y - 20);
        //            canvas.DrawBitmap(DrawPillShapeTitle("相关活动"), x, y);
        //            canvas.DrawBitmap(await DrawEventLogo(skEvent, true), x, y + 60);
        //            y += 360;
        //        }
        //        //相关卡池
        //        if (gachaHight > 0)
        //        {
        //            canvas.DrawBitmap(dottedLine, x, y - 20);
        //            canvas.DrawBitmap(DrawPillShapeTitle("相关卡池"), x, y);
        //            for (int i = 0; i < gachas.Count; i++)
        //            {
        //                if (i % 2 == 0)
        //                {
        //                    canvas.DrawBitmap(await DrawGachaCard(gachas[i]), x + 25, y + 60);
        //                }
        //                else
        //                {
        //                    canvas.DrawBitmap(await DrawGachaCard(gachas[i]), x + 25 + 25 + 350, y + 60);
        //                    y += 200;
        //                }
        //            }
        //        }
        //        using (SKPaint mark = new SKPaint())
        //        {
        //            mark.TextAlign = SKTextAlign.Center;
        //            mark.TextSize = 25;
        //            mark.Color = SKColors.Gray;
        //            mark.IsAntialias = true;
        //            mark.Typeface = _typeface;
        //            canvas.DrawText("Created By MinoriBot @GitHub", width / 2, height - 15, mark);
        //        }
        //        dottedLine.Dispose();
        //    }
        //    Console.WriteLine("生成卡面信息图片成功");
        //    return ConvertBitmapToBase64(cardInfo);
        //}
        /// <summary>
        /// 活动列表
        /// </summary>
        /// <param name="skEvents"></param>
        /// <returns></returns>
        //public static async Task<string> DrawEventList(List<SkEvents> skEvents)
        //{
        //    int width = 1000;
        //    int height = 280;
        //    List<SKBitmap> eventImages = new List<SKBitmap>();
        //    for (int i = 0; i < skEvents.Count; i++)
        //    {
        //        eventImages.Add(await DrawSimpleEventImage(skEvents[i]));
        //        height += 30 + eventImages[i].Height;
        //    }
        //    SKBitmap eventList = new SKBitmap(width, height);
        //    using (var canvas = new SKCanvas(eventList))
        //    {
        //        DrawBackGroud(eventList, "活动");
        //        int x = 100;
        //        int y = 250;
        //        SKBitmap dottedLine = DrawDottedLine(800, 5);
        //        SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
        //        for (int i = 0; i < eventImages.Count; i++)
        //        {
        //            canvas.DrawBitmap(eventImages[i], x, y, highQuality);
        //            canvas.DrawBitmap(dottedLine, x, y + eventImages[i].Height + 10);
        //            y += eventImages[i].Height + 30;
        //        }
        //        using (SKPaint mark = new SKPaint())
        //        {
        //            mark.TextAlign = SKTextAlign.Center;
        //            mark.TextSize = 25;
        //            mark.Color = SKColors.Gray;
        //            mark.IsAntialias = true;
        //            mark.Typeface = _typeface;
        //            canvas.DrawText("Created By MinoriBot @GitHub", width / 2, height - 15, mark);
        //        }
        //    }
        //    Console.WriteLine("活动列表绘制完毕");
        //    return ConvertBitmapToBase64(eventList);
        //}
        /// <summary>
        /// 活动信息
        /// </summary>
        /// <param name="skEvent"></param>
        /// <returns></returns>
        //public static async Task<string> DrawEventInfo(SkEvents skEvent)
        //{
        //    int width = 1000;
        //    int height = 0;
        //    List<SkMusics> currentMusics = skEvent.GetEventMusics();
        //    List<SkGachas> gachas = skEvent.GetGachasInEvent();
        //    List<SKBitmap> gachaImages = new List<SKBitmap>();
        //    int gachaHeight = (int)Math.Ceiling(gachas.Count / 2f) * 200;
        //    for (int i = 0; i < gachas.Count; i++)
        //    {
        //        gachaImages.Add(await DrawGachaCard(gachas[i]));
        //    }

        //    //430大图,130单行文字+标题，280卡牌，100+90n相关歌曲,50+gachaHeight相关卡池，50底部留白,250背景留空
        //    height = 430 + 130 + 130 + 130 + 130 + 130 + 130 + 220 + 280 + 280 + 100 + currentMusics.Count * 90 + 50 + gachaHeight + 50 + 250;
        //    SKBitmap eventInfo = new SKBitmap(width, height);
        //    using (var canvas = new SKCanvas(eventInfo))
        //    {
        //        DrawBackGroud(eventInfo, "活动");
        //        SKBitmap dottedLine = DrawDottedLine(800, 5);
        //        SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
        //        SKPaint font = new SKPaint() { Typeface = _typeface, IsAntialias = true, TextSize = 40 };
        //        int x = 100;
        //        int y = 250;
        //        canvas.DrawBitmap(await skEvent.GetEventBanner(), new SKRect(x, y, x + 800, y + 341), highQuality);
        //        y = y + 340 + 40;
        //        canvas.DrawBitmap(DrawPillShapeTitle("活动名称"), x, y);
        //        canvas.DrawText(skEvent.name, x + 25, y + 50 + 40, font);
        //        y += 90 + 40;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("类型"), x, y);
        //        canvas.DrawText(skEvent.GetEventType(), x + 25, y + 50 + 40, font);
        //        canvas.DrawBitmap(DrawPillShapeTitle("ID"), x + 400, y);
        //        canvas.DrawText(skEvent.id.ToString(), x + 400 + 25, y + 50 + 40, font);
        //        y += 90 + 40;
        //        DateTime startTime = Utils.TimeStampToDateTime(skEvent.startAt);
        //        DateTime endTime = Utils.TimeStampToDateTime(skEvent.aggregateAt);
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("开始时间"), x, y);
        //        canvas.DrawText(startTime.ToString("yyyy年MM月dd日 HH:mm"), x + 25, y + 50 + 40, font);
        //        y += 90 + 40;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("结束时间"), x, y);
        //        canvas.DrawText(endTime.ToString("yyyy年MM月dd日 HH:mm"), x + 25, y + 50 + 40, font);
        //        y += 90 + 40;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("活动属性加成"), x, y);
        //        canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{skEvent.GetBunusAttr()}.png"), new SKRect(x + 25, y + 50 + 10, x + 25 + 40, y + 50 + 10 + 40), highQuality);
        //        canvas.DrawText($"+{skEvent.GetBunusAttRate().ToString("F0")}%", x + 80, y + 50 + 40, font);
        //        y += 90 + 40;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("活动角色加成"), x, y);
        //        List<int> bonusCharacters = skEvent.GetBunusCharacters();
        //        for (int i = 0; i < bonusCharacters.Count; i++)
        //        {
        //            canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{bonusCharacters[i]}.png"), new SKRect(x + 25 + i * 45, y + 50 + 10, x + 25 + 40 + i * 45, y + 50 + 10 + 40), highQuality);
        //        }
        //        canvas.DrawText($"+{skEvent.GetBunusCharacterRate().ToString("F0")}%", x + 25 + bonusCharacters.Count * 45 + 20, y + 50 + 40, font);
        //        y += 90 + 40;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("活动奖励"), x, y);
        //        List<int> degrees = new List<int>() { 1, 2, 3, 10, 100, 1000 };
        //        int tempX = x + 25;
        //        int tempY = y + 40 + 20;
        //        for (int i = 0; i < 6; i++)
        //        {
        //            if (i % 3 == 0 && i != 0)
        //            {
        //                tempY += 70;
        //                tempX = x + 25;
        //            }
        //            canvas.DrawBitmap(DrawDegreeHonor(await skEvent.GetEventDegreeMain(), degrees[i]), new SKRect(tempX, tempY, tempX + 230, tempY + 48), highQuality);
        //            tempX += 250;
        //        }
        //        y += 40 + 20 + 70 + 50 + 40;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("奖励卡牌"), x, y);
        //        List<SkCard> cards = skEvent.GetCurrentCards();
        //        int tempI = 0;
        //        for (int i = 0; i < cards.Count; i++)
        //        {
        //            if (cards[i].GetStarsCount() < 4)
        //            {
        //                canvas.DrawBitmap(await DrawCardIcon(cards[i], true, true), x + 25 + 165 * tempI, y + 60);
        //                tempI++;
        //            }
        //        }
        //        y += 60 + 180 + 40;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("活动期间卡池卡牌"), x, y);
        //        tempI = 0;
        //        for (int i = 0; i < cards.Count; i++)
        //        {
        //            if (cards[i].GetStarsCount() == 4)
        //            {
        //                canvas.DrawBitmap(await DrawCardIcon(cards[i], true, true), x + 25 + 165 * tempI, y + 60);
        //                tempI++;
        //            }
        //        }
        //        y += 60 + 180 + 40;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("活动相关歌曲"), x, y);
        //        int musicY = y + 60;
        //        for (int i = 0; i < currentMusics.Count; i++)
        //        {
        //            canvas.DrawBitmap(await DrawMusicCard(currentMusics[i]), x + 25, musicY);
        //            if (i != currentMusics.Count - 1)
        //            {
        //                canvas.DrawBitmap(DrawDottedLine(750, 5), x + 25, musicY + 90 - 7.5f);
        //            }
        //            musicY += 90;
        //        }
        //        y += 60 + 90 * currentMusics.Count + 40;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("活动相关卡池"), x, y);
        //        int gachaY = y + 60;
        //        for (int i = 0; i < gachaImages.Count; i++)
        //        {
        //            if (i % 2 == 0)
        //            {
        //                canvas.DrawBitmap(gachaImages[i], x + 25, gachaY);
        //            }
        //            if (i % 2 == 1)
        //            {
        //                canvas.DrawBitmap(gachaImages[i], x + 25 + 350 + 25, gachaY);
        //                gachaY += gachaImages[i].Height + 10;
        //            }
        //        }

        //        //水印
        //        using (SKPaint mark = new SKPaint())
        //        {
        //            mark.TextAlign = SKTextAlign.Center;
        //            mark.TextSize = 30;
        //            mark.Color = SKColors.Gray;
        //            mark.IsAntialias = true;
        //            mark.Typeface = font.Typeface;
        //            canvas.DrawText("Created By MinoriBot @GitHub", width / 2, height - 15, mark);
        //        }
        //    }
        //    Console.WriteLine("活动信息图绘制完成");
        //    return ConvertBitmapToBase64(eventInfo);
        //}
        public static async Task<SKBitmap> DrawMusicList(List<SkMusics> skMusics)
        {
            int width = 750;
            int height = 0;
            List<SKBitmap> musicLines = new List<SKBitmap>();
            for(int i = 0; i < skMusics.Count; i++)
            {
                musicLines.Add(await DrawMusicLine(skMusics[i]));
                height += musicLines[i].Height + 10;
            }
            height -= 10;
            SKBitmap dottline = DrawDottedLine(800, 10);
            SKBitmap musicList = new SKBitmap(width,height);
            using(SKCanvas canvas = new SKCanvas(musicList))
            {
                int x = 0;
                int y = 0;
                for(int i = 0; i < musicLines.Count; i++)
                {
                    canvas.DrawBitmap(musicLines[i], x, y);
                    canvas.DrawBitmap(dottline, x, y + musicLines[i].Height);
                    y += musicLines[i].Height + 10;
                }
            }
            return musicList;
        }
        public static async Task<SKBitmap> DrawMusicFace(SkMusics music)
        {
            int width = 800;
            int height = 400;
            SKBitmap musicFace = new SKBitmap(width, height);
            using(SKCanvas canvas = new SKCanvas(musicFace))
            using (SKPaint font = new SKPaint() { Typeface = _typeface, TextSize = 40, IsAntialias = true })
            {
                int x = 0;
                int y = 0;
                canvas.DrawBitmap(await music.GetMusicJacket(), new SKRect(x, y, x + 400, y + 400), _highQuality);
                List<string> musicTitle = SplitString(music.title, 40, 360);
                for (int i = 0; i < musicTitle.Count; i++)
                {
                    canvas.DrawText(musicTitle[i], x + 440, y + 50 + 40 * i, font);
                }
                y += 30 + 40 * musicTitle.Count;
                canvas.DrawBitmap(DrawDottedLine(360, 5), x + 440, y);
                font.TextSize = 34;
                y += 15;
                float textLong = font.MeasureText("Vocals:");
                canvas.DrawText($"Vocals:", x + 440, y + 34, font);
                List<List<SkMusicVocals.Characters>> vocals = music.GetVocals();
                List<SKBitmap> vocalImages = new List<SKBitmap>();
                for (int i = 0; i < vocals.Count; i++)
                {
                    vocalImages.Add(DrawVocals(vocals[i]));
                }
                int vocalLong = (int)textLong + 20;
                int vocalX = 440 + vocalLong;
                int vocalY = y;
                for (int i = 0; i < vocalImages.Count; i++)
                {
                    vocalLong = vocalX - 440 + vocalImages[i].Width;
                    if (vocalLong > 360)
                    {
                        vocalLong = vocalImages[i].Width;
                        vocalX = 440;
                        vocalY += 60;
                    }
                    canvas.DrawBitmap(vocalImages[i], vocalX, vocalY);
                    vocalX += vocalImages[i].Width + 10;
                }
                y = 340;
                x = 740;
                List<int> diffs = music.GetDifficulties().Values.ToList();
                List<SKColor> colors = new List<SKColor>() { new SKColor(102, 221, 17), new SKColor(51, 187, 237), new SKColor(255, 170, 1), new SKColor(238, 69, 102), new SKColor(187, 51, 239) };
                for(int i = diffs.Count-1;i>= 0;i--)
                {
                    if(i == 5)
                    {
                        canvas.DrawBitmap(DrawGradientCircleWithText(diffs[i].ToString(), 34, 30, new SKColor(172, 143, 235), new SKColor(255, 121, 225)), x, y);
                        x -= 65;
                    }
                    else
                    {
                        canvas.DrawBitmap(DrawRoundWithText(diffs[i].ToString(), 34, 30, colors[i]), x , y);
                        x -= 65;
                    }
                }
            }
            return musicFace;
        }
        //public static async Task<string> DrawMusicInfo(SkMusics skMusic)
        //{
        //    int width = 1000;
        //    int height = 0;
        //    height = 250 + 500 + 130 * 6;
        //    SKBitmap musicInfo = new SKBitmap(width, height);
        //    using (SKCanvas canvas = new SKCanvas(musicInfo))
        //    using (SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High })
        //    using (SKPaint font = new SKPaint() { Typeface = _typeface, TextSize = 40, IsAntialias = true })
        //    {
        //        Console.WriteLine("正在生成歌曲信息图");
        //        int x = 100;
        //        int y = 250;
        //        DrawBackGroud(musicInfo, "歌曲");
        //        canvas.DrawBitmap(await skMusic.GetMusicJacket(), new SKRect(x, y, x + 400, y + 400), highQuality);
        //        List<string> musicTitle = SplitString(skMusic.title, 40, 360);
        //        for (int i = 0; i < musicTitle.Count; i++)
        //        {
        //            canvas.DrawText(musicTitle[i], x + 440, y + 50 + 40 * i, font);
        //        }
        //        y += 30 + 40 * musicTitle.Count;
        //        canvas.DrawBitmap(DrawDottedLine(360, 5), x + 440, y);
        //        font.TextSize = 34;
        //        y += 15;
        //        float textLong = font.MeasureText("Vocals:");
        //        canvas.DrawText($"Vocals:", x + 440, y + 34, font);
        //        List<List<SkMusicVocals.Characters>> vocals = skMusic.GetVocals();
        //        List<SKBitmap> vocalImages = new List<SKBitmap>();
        //        for (int i = 0; i < vocals.Count; i++)
        //        {
        //            vocalImages.Add(DrawVocals(vocals[i]));
        //        }
        //        int vocalLong = (int)textLong + 20;
        //        int vocalX = 540 + vocalLong;
        //        int vocalY = y;
        //        for (int i = 0; i < vocalImages.Count; i++)
        //        {
        //            vocalLong = vocalX - 540 + vocalImages[i].Width;
        //            if (vocalLong > 360)
        //            {
        //                vocalLong = vocalImages[i].Width;
        //                vocalX = 540;
        //                vocalY += 60;
        //            }
        //            canvas.DrawBitmap(vocalImages[i], vocalX, vocalY);
        //            vocalX += vocalImages[i].Width + 10;
        //        }


        //        y = 590;
        //        List<int> diffs = skMusic.GetDifficulties().Values.ToList();
        //        List<SKColor> colors = new List<SKColor>() { new SKColor(102, 221, 17), new SKColor(51, 187, 237), new SKColor(255, 170, 1), new SKColor(238, 69, 102), new SKColor(187, 51, 239) };
        //        for (int i = 0; i < diffs.Count; i++)
        //        {
        //            canvas.DrawBitmap(DrawRoundWithText(diffs[i].ToString(), 34, 30, colors[i]), x + 440 + 70 * i, y);
        //        }
        //        y = 700;
        //        font.TextSize = 40;
        //        SKBitmap dottedLine = DrawDottedLine(800, 5);
        //        canvas.DrawBitmap(DrawPillShapeTitle("歌曲名称"), x, y);
        //        canvas.DrawText(skMusic.title, x + 25, y + 50 + 40, font);
        //        y += 130;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("ID"), x, y);
        //        canvas.DrawText(skMusic.id.ToString(), x + 25, y + 50 + 40, font);
        //        y += 130;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("作词"), x, y);
        //        canvas.DrawText(skMusic.lyricist, x + 25, y + 50 + 40, font);
        //        y += 130;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("作曲"), x, y);
        //        canvas.DrawText(skMusic.composer, x + 25, y + 50 + 40, font);
        //        y += 130;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("编曲"), x, y);
        //        canvas.DrawText(skMusic.arranger, x + 25, y + 50 + 40, font);
        //        y += 130;
        //        canvas.DrawBitmap(dottedLine, x, y - 20);
        //        canvas.DrawBitmap(DrawPillShapeTitle("开放时间"), x, y);
        //        canvas.DrawText(Utils.TimeStampToDateTime(skMusic.publishedAt).ToString("yyyy年MM月dd日 HH:mm:ss"), x + 25, y + 50 + 40, font);
        //    }
        //    Console.WriteLine("成歌曲信息图制作完毕");
        //    return ConvertBitmapToBase64(musicInfo);
        //}
        public static async Task<string> DrawGachaInfo(SkGachas gacha)
        {
            int width = 1000;
            int height = 2000;
            SKBitmap banner = await gacha.GetGachaBanner();
            int bannerHeight = (int)(800f / banner.Width * banner.Height);
            SKBitmap gachaInfo = new SKBitmap(width, height);
            using (SKCanvas canvas = new SKCanvas(gachaInfo))
            using (SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High })
            using (SKPaint font = new SKPaint() { Typeface = _typeface, TextSize = 40, IsAntialias = true })
            {
                Console.WriteLine("正在生成卡池信息图");
                int x = 100;
                int y = 250;
                SKBitmap dottedLine = DrawDottedLine(800, 5);
                DrawBackGroud(gachaInfo, "扭蛋");
                canvas.DrawBitmap(banner, new SKRect(x, y, x + 800, y + bannerHeight), highQuality);
                y += bannerHeight + 40;
                canvas.DrawBitmap(DrawPillShapeTitle("扭蛋名称"), x, y);
                canvas.DrawText(gacha.name, x + 25, y + 50 + 40, font);
                y += 130;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("类型"), x, y);
                canvas.DrawText(gacha.GetGachaType(), x + 25, y + 50 + 40, font);
                canvas.DrawBitmap(DrawPillShapeTitle("ID"), x + 400, y);
                canvas.DrawText(gacha.id.ToString(), x + 400 + 25, y + 50 + 40, font);
                y += 130;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("开始时间"), x, y);
                canvas.DrawText(Utils.TimeStampToDateTime(gacha.startAt).ToString("yyyy年MM月dd日  HH:mm"), x + 25, y + 50 + 40, font);
                y += 130;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("结束时间"), x, y);
                canvas.DrawText(Utils.TimeStampToDateTime(gacha.endAt).ToString("yyyy年MM月dd日  HH:mm"), x + 25, y + 50 + 40, font);
                y += 130;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawTitleWithText("描述", gacha.gachaInformation.description), x, y);
            }
            return ConvertBitmapToBase64(gachaInfo);
        }
        /// <summary>
        /// 调整banner的大小
        /// </summary>
        /// <param name="skEvent"></param>
        /// <returns></returns>
        public static async Task<SKBitmap> DrawEventBanner(SkEvents skEvent)
        {
            SKBitmap banner = new SKBitmap(800,341);
            using(SKCanvas canvas = new SKCanvas(banner))
            {
                canvas.DrawBitmap(await skEvent.GetEventBanner(), new SKRect(0, 0, 800, 341), _highQuality);
            }
            return banner;
        }
        public static async Task<SKBitmap> DrawSimpleEventImage(SkEvents skEvent)
        {
            int width = 800;
            List<int> bonusChara = skEvent.GetBunusCharacters();
            List<SkCard> currentCards = skEvent.GetCurrentCards();
            int height = 150 + 10 + 150 * (int)Math.Ceiling(currentCards.Count / 5.0);
            SKBitmap eventImage = new SKBitmap(width, height);
            SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
            SKPaint font = new SKPaint() { Typeface = _typeface, IsAntialias = true, TextSize = 23 };
            using (var canvas = new SKCanvas(eventImage))
            {
                canvas.Clear(SKColors.White);
                canvas.DrawBitmap(await skEvent.GetEventBanner(), new SKRect(0, 0, 350, 150), highQuality);
                canvas.DrawText($"ID:{skEvent.id}    类型:{skEvent.GetEventType()}", 350 + 20, 25, font);
                DateTime startTime = Utils.TimeStampToDateTime(skEvent.startAt);
                DateTime endTime = Utils.TimeStampToDateTime(skEvent.aggregateAt);
                canvas.DrawText("开始时间: " + startTime.ToString("yyyy年MM月dd日 HH:mm"), 350 + 20, 50, font);
                canvas.DrawText("结束时间: " + endTime.ToString("yyyy年MM月dd日 HH:mm"), 350 + 20, 75, font);
                canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{skEvent.GetBunusAttr()}.png"), new SKRect(350 + 20, 80, 350 + 20 + 30, 110), highQuality);
                canvas.DrawText("+" + skEvent.GetBunusAttRate().ToString("F0") + "%", 420, 103, font);
                for (int i = 0; i < bonusChara.Count; i++)
                {
                    canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{bonusChara[i]}.png"), new SKRect(370 + i * 35, 115, 400 + i * 35, 145), highQuality);
                }
                canvas.DrawText("+" + skEvent.GetBunusCharacterRate().ToString("F0") + "%", 370 + 35 * bonusChara.Count + 20, 138, font);
                int x = 0;
                int y = 160;
                for (int i = 0; i < currentCards.Count; i++)
                {
                    if (i % 5 == 0)
                    {
                        x = 0;
                        y = 160 + i / 5 * 150;
                    }
                    canvas.DrawBitmap(await DrawCardIcon(currentCards[i], true, true), new SKRect(x, y, x + 135, y + 150), highQuality);
                    x += 140;
                }
            }
            return eventImage;
        }
        public static async Task<string> TestImage(SkEvents skEvent)
        {
            return ConvertBitmapToBase64(await DrawSimpleEventImage(skEvent));
        }



        //上边内容将在未来分离到别处



        public class ListConfig
        {
            public string title;
            public string text;
            public List<SKBitmap> images;
            public SKPaint font = new SKPaint() { Typeface = _typeface, IsAntialias = true, TextSize = 40 };
        }
        /// <summary>
        /// 将所有图片拼到一起输出最终图片
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public static SKBitmap DrawALL(List<SKBitmap> images ,int width = 1000)
        {
            int height = 100;
            for(int i = 0; i < images.Count; i++)
            {
                height += images[i].Height + 50;
            }
            height -= 50;
            SKBitmap result = new SKBitmap(width, height);
            using (SKCanvas canvas = new SKCanvas(result))
            {
                SKBitmap backGroud = SKBitmap.Decode("./asset/normal/searchBg.png");
                int bgHeight = (int)((float)width / backGroud.Width * backGroud.Height);
                for (int i = 0; i < height; i += bgHeight)
                {
                    canvas.DrawBitmap(backGroud, new SKRect(0, i, width, i + bgHeight), _highQuality);
                }
                int x = 50;
                int y = 50;
                if (images[0].Height == 110 && height > 220 && width > 700)//表情包
                {
                    canvas.DrawBitmap(SKBitmap.Decode("./asset/normal/Minori01.png"), new SKRect(556, 20, 775, 220), _highQuality);
                }
                for(int i = 0; i < images.Count; i++)
                {
                    canvas.DrawBitmap(images[i], x, y);
                    y += images[i].Height + 50;
                }
                using (SKPaint mark = new SKPaint())
                {
                    mark.TextAlign = SKTextAlign.Center;
                    mark.TextSize = 30;
                    mark.Color = SKColors.Gray;
                    mark.IsAntialias = true;
                    mark.Typeface = _typeface;
                    canvas.DrawText("Created By MinoriBot @GitHub", width / 2, height - 15, mark);
                }
            }
            return result;
        }
        /// <summary>
        /// 将所有信息拼在一起
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public static SKBitmap DrawInfoBlock(List<SKBitmap> images, int width = 900)
        {
            int height = 100;
            for(int i = 0; i < images.Count; i++)
            {
                height += images[i].Height;
            }
            SKBitmap allInfo = new SKBitmap(width,height);
            using(SKCanvas canvas = new SKCanvas(allInfo))
            {
                int x = 50;
                int y = 50;
                using (SKPaint paint = new SKPaint())
                {
                    paint.Color = SKColors.White;
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawRoundRect(new SKRect(0,0,width,height), 25, 25, paint);
                }
                for (int i = 0; i < images.Count; i++)
                {
                    canvas.DrawBitmap(images[i], x, y);
                    y += images[i].Height;
                }
            }
            return allInfo;
        }
        /// <summary>
        /// 标题+文字内容
        /// </summary>
        /// <returns></returns>
        public static SKBitmap DrawTitleWithText(ListConfig config)
        {
            int width = 800;
            int height = 40 + 10;
            int textSize = (int)config.font.TextSize;
            int lineHeight = (int)(textSize * 4f / 3f);
            List<string> texts = SplitString(config.text, 40, width - 40);
            height += texts.Count * lineHeight;
            SKBitmap output = new SKBitmap(width, height);
            using (SKCanvas canvas = new SKCanvas(output))
            using (SKPaint font = config.font)
            {
                int x = 0;
                int y = 0;
                canvas.DrawBitmap(DrawPillShapeTitle(config.title), x, y);
                x += 20;
                y += 40 +10;
                for (int i = 0; i < texts.Count; i++)
                {
                    canvas.DrawText(texts[i], x, y + lineHeight / 2 + textSize / 3, font);
                    y += lineHeight;
                }
            }
            return output;
        }
        /// <summary>
        /// 标题+纯图片内容
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static SKBitmap DrawTitleWithImage(ListConfig config)
        {
            int width = 800;
            int height = 40 + 20;
            int tempLength = 0;
            int lineCount = 1;
            if (config.images.Count > 0)
            {
                for (int i = 0; i < config.images.Count; i++)
                {
                    if(tempLength + config.images[i].Width > 800 - 40)
                    {
                        lineCount++;
                        tempLength = config.images[i].Width;
                        continue;
                    }
                    tempLength += config.images[i].Width + 20;
                }
            }
            height += config.images[0].Height * lineCount + (lineCount - 1) * 20;
            tempLength = 0;
            lineCount = 1;
            SKBitmap output = new SKBitmap(width,height);
            using(SKCanvas canvas = new SKCanvas(output))
            {
                int x = 0;
                int y = 0;
                canvas.DrawBitmap(DrawPillShapeTitle(config.title), x, y);
                x = 20;
                y = 60;
                for(int i = 0; i < config.images.Count; i++)
                {
                    if (tempLength + config.images[i].Width > 800 - 40)
                    {
                        tempLength = config.images[i].Width +20;
                        x = 20;
                        y += config.images[0].Height + 20;
                        canvas.DrawBitmap(config.images[i], x, y);
                        x = tempLength + 20;
                    }
                    else
                    {
                        tempLength += config.images[i].Width + 20;
                        canvas.DrawBitmap(config.images[i], x, y);
                        x = tempLength +20;
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// 画加成图 属性/角色图标+文字
        /// </summary>
        /// <returns></returns>
        public static SKBitmap DrawBunusImage(string? attr,List<int>? characters,double rate)
        {
            SKBitmap bunus = new SKBitmap(750, 40);
            using(SKCanvas canvas = new SKCanvas(bunus))
            using(SKPaint font = new SKPaint() { Typeface = _typeface, IsAntialias = true, TextSize = 36 })
            {
                if (attr != null && characters == null)
                {
                    canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{attr}.png"), new SKRect(0, 0, 40, 40), _highQuality);
                    canvas.DrawText($"+{rate.ToString("F0")}%", 60, 32, font);
                }
                if (characters != null && attr == null)
                {
                    for (int i = 0; i < characters.Count; i++)
                    {
                        canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{characters[i]}.png"), new SKRect(i * 45, 0, 40 + i * 45, 40), _highQuality);
                    }
                    canvas.DrawText($"+{rate.ToString("F0")}%", characters.Count * 45 + 20, 32, font);
                }
            }
            return bunus;
        }
        public static SKBitmap FixTwoTitleInOneLine(SKBitmap image1,SKBitmap image2)
        {
            int maxHeight = image1.Height>image2.Height? image1.Height : image2.Height;
            SKBitmap output = new SKBitmap(800, maxHeight);
            using(SKCanvas canvas = new SKCanvas(output))
            {
                canvas.DrawBitmap(image1, 0, 0);
                canvas.DrawBitmap(image2, 400, 0);
            }
            return output;
        }
        public static SKBitmap DrawCardTitle(SkCard card)
        {
            SKBitmap cardTitle = new SKBitmap(800, 130);
            using(SKCanvas canvas = new SKCanvas(cardTitle))
            {
                //背景颜色
                using (SKPaint paint = new SKPaint())
                {
                    paint.Color = new SKColor(235, 235, 235);
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawRect(new SKRect(0, 0, 800, 130), paint);
                }

                //团体logo
                SKBitmap dottedLine = DrawDottedLine(800, 5);
                string logoDir = $"asset/normal/logo_{card.GetGroupNameById()}.png";
                SKBitmap logoImage = SKBitmap.Decode(logoDir);
                float logoWidth = 100f / logoImage.Height * logoImage.Width;
                canvas.DrawBitmap(logoImage, new SKRect(70, 15, 70 + logoWidth,115), _highQuality);

                //卡牌名称与角色名称
                using(SKPaint font =new SKPaint())
                {
                    font.Typeface = _typeface;
                    font.IsAntialias = true;
                    font.TextSize = 30;
                    font.Color = SKColors.Black;
                    canvas.DrawText(card.prefix, 350, 50, font);
                    font.TextSize = 40;
                    canvas.DrawText(NickName.idToName[card.characterId], 350, 100, font);
                }
            }
            return cardTitle;
        }
        /// <summary>
        /// 画卡面插画
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isTrained"></param>
        /// <returns></returns>
        public static async Task<SKBitmap> DrawCardIllustrationImage(SkCard card, bool isTrained)
        {
            SKBitmap cardIllustration = new SKBitmap(800, 450);
            int starCount = card.GetStarsCount();
            string cardFrameDir = string.Empty;
            if (starCount > 0)
            {
                cardFrameDir =  $"./asset/normal/cardFrame_{starCount}.png";
            }
            else
            {
                cardFrameDir = "./asset/normal/cardFrame_bd.png";
            }
            using (SKCanvas canvas = new SKCanvas(cardIllustration))
            {
                //画卡面插图 卡面大小800*450 星星大小40*39
                int x = 0;
                int y = 0;
                SKBitmap cardIllustrationImage = await card.GetCardIllustrationImage(isTrained);
                SKBitmap cardIllustrationImage_afterCropping = CropCardIllustrationImage(cardIllustrationImage);
                canvas.DrawBitmap(cardIllustrationImage_afterCropping, new SKRect(x, y, x + 800, y + 450), _highQuality);
                canvas.DrawBitmap(SKBitmap.Decode(cardFrameDir), new SKRect(x, y, x + 800, y + 450), _highQuality);
                canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{card.attr}.png"), new SKRect(x + 740, y, x + 800, y + 60), _highQuality);
                SKBitmap star = SKBitmap.Decode("./asset/normal/normal_star.png");
                if (starCount > 2 && isTrained == true)
                {
                    star = SKBitmap.Decode("./asset/normal/after_training_star.png");
                }
                //星星
                if (starCount == 0)//生日卡
                {
                    canvas.DrawBitmap(SKBitmap.Decode("./asset/normal/birthday_star.png"), new SKRect(x + 18, y + 393, x + 18 + 40, y + 393 + 39), _highQuality);
                }
                else
                {
                    for (int i = 0; i < starCount; i++)
                    {
                        canvas.DrawBitmap(star, new SKRect(x + 18, y + 393 - 39 * i, x + 18 + 40, y + 393 + 39 - 39 * i), _highQuality);
                    }
                }
            }
            return cardIllustration;
        }
        /// <summary>
        /// 画综合力
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static SKBitmap DrawCardPower(SkCard card)
        {
            SKBitmap cardPower = new SKBitmap(800, 320);
            using (SKCanvas canvas = new SKCanvas(cardPower))
            using(SKPaint font =new SKPaint() { IsAntialias=true,Typeface=_typeface})
            { 
                int x = 0;
                int y = 0;
                canvas.DrawBitmap(DrawPillShapeTitle("综合力"), x, y);
                font.TextSize = 40;
                int[] power = card.GetPower();
                canvas.DrawText($"综合力: {power[0] + power[1] + power[2]}  +  ({power[3] * 3 * 5})", x + 25, y + 50 + 40, font);
                font.TextSize = 30;
                using (SKPaint bar = new SKPaint())
                {
                    bar.IsAntialias = true;
                    bar.Color = new SKColor(144, 238, 144);
                    canvas.DrawText($"表现力: {power[0]} + ({power[3] * 5})", x + 25, y + 95 + 30 + 10, font);
                    canvas.DrawRoundRect(new SKRect(x + 25, y + 135 + 10, x + 25 + power[0] / 15000f * 750f, y + 145 + 30), 10, 10, bar);
                    bar.Color = new SKColor(100, 149, 237);
                    canvas.DrawText($"技术力: {power[1]} + ({power[3] * 5})", x + 25, y + 175 + 30, font);
                    canvas.DrawRoundRect(new SKRect(x + 25, y + 205 + 10, x + 25 + power[1] / 15000f * 750f, y + 215 + 30), 10, 10, bar);
                    bar.Color = new SKColor(147, 112, 219);
                    canvas.DrawText($"活力: {power[2]} + ({power[3] * 5})", x + 25, y + 245 + 30, font);
                    canvas.DrawRoundRect(new SKRect(x + 25, y + 275 + 10, x + 25 + power[2] / 15000f * 750f, y + 285 + 30), 10, 10, bar);
                }
            }
            return cardPower;
        }
        /// <summary>
        /// 画技能
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static SKBitmap DrawCardSkill(SkCard card)
        {
            string skillDescription = card.GetSkillDescription().Replace("\n", "");
            List<string> skillDescriptionList = SplitString(skillDescription, 30, 760);
            SKBitmap titleAndName = DrawTitleWithText(new ListConfig() { title = "技能", text = card.cardSkillname });
            int height = titleAndName.Height + 10 + skillDescriptionList.Count * 30;
            SKBitmap skill = new SKBitmap(800,height);
            using(SKCanvas canvas = new SKCanvas(skill))
            {
                int x = 0;
                int y = 0;
                canvas.DrawBitmap(titleAndName, x, y);
                y += titleAndName.Height;
                using (SKPaint paint = new SKPaint())
                {
                    paint.Color = new SKColor(221, 221, 221);
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawRect(new SKRect(x + 15, y + 5, x + 750 + 30, y + skillDescriptionList.Count * 30 + 10), paint);
                    paint.TextSize = 30;
                    paint.Color = SKColors.Black;
                    paint.Typeface = _typeface;
                    for (int i = 0; i < skillDescriptionList.Count; i++)
                    {
                        canvas.DrawText(skillDescriptionList[i], x + 25, y + 30, paint);
                        y += 30;
                    }
                }
            }
            return skill;
        }
        /// <summary>
        /// 画卡牌头像
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isTrained"></param>
        /// <returns></returns>
        public static async Task<SKBitmap> DrawCardIcon(SkCard card, bool isTrained, bool needId)
        {
            string trainingStatus = isTrained ? "after_training" : "normal";
            string fileDirectory = "./asset/normal";
            int height = needId ? 180 : 156;
            SKBitmap bitmap = new SKBitmap(156, height);
            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
                SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
                canvas.Clear(SKColors.White); ;
                int starCount = card.GetStarsCount();
                if (starCount < 3)
                {
                    trainingStatus = "normal";
                    isTrained = false;
                }
                //画头像边框和星星
                if (starCount > 0)
                {
                    canvas.DrawBitmap(await card.GetCardIcon(isTrained), new SKRect(0, 0, 156, 156));
                    canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/cardIconFrame_{starCount}.png"), new SKRect(0, 0, 156, 156), highQuality);
                    for (int i = 0; i < starCount; i++)
                    {
                        canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/{trainingStatus}_star.png"), new SKRect(10 + i * 25, 121, 35 + i * 25, 146), highQuality);
                    }
                }
                if (starCount == 0)
                {
                    canvas.DrawBitmap(await card.GetCardIcon(false), new SKRect(0, 0, 156, 156));
                    canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/cardIconFrame_bd.png"), new SKRect(0, 0, 156, 156), highQuality);
                    canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/birthday_star.png"), new SKRect(10, 116, 40, 146), highQuality);
                }
                //画属性
                canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/{card.attr}.png"), new SKRect(10, 10, 40, 40), highQuality);
                //是否限定
                if (card.GetCardLimitType() == "限定")
                {
                    canvas.DrawBitmap(SKBitmap.Decode("./asset/normal/badge_limited.png"), 45, 0, highQuality);
                }
                //添加文字id
                if (needId)
                {
                    using (SKPaint font = new SKPaint())
                    {
                        font.IsAntialias = true;
                        font.TextSize = 20;
                        font.Typeface = _typeface;
                        font.Color = SKColors.DarkGray;
                        font.TextAlign = SKTextAlign.Left;
                        string text = "ID:" + card.id.ToString();
                        canvas.DrawText(text, 5, 175, font);
                    }
                }
                //using (var image = SKImage.FromBitmap(bitmap))
                //using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                //using (var stream = File.OpenWrite($"./asset/temp/{card.id}.png"))
                //{
                //    data.SaveTo(stream);
                //}
                return bitmap;
            }
        }

        /// <summary>
        /// 对卡面插图进行裁剪
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static SKBitmap CropCardIllustrationImage(SKBitmap input)
        {
            try
            {
                SKRectI cropRect = new SKRectI(0, 54, input.Width, input.Height - 55);
                SKBitmap output = new SKBitmap(cropRect.Width, cropRect.Height);
                using (SKCanvas canvas = new SKCanvas(output))
                {
                    canvas.DrawBitmap(input, cropRect, new SKRectI(0, 0, output.Width, output.Height));
                    input.Dispose();
                    return output;
                }
            }
            catch
            {
                return SKBitmap.Decode("./asset/normal/err.png");
            }
        }
        public static SKBitmap DrawTitleWithText(string title, string text)
        {
            int width = 800;
            int height = 20 + 40 + 20;
            List<string> texts = SplitString(text, 40, width);
            height += texts.Count * 40;
            SKBitmap output = new SKBitmap(width, height);
            using (SKCanvas canvas = new SKCanvas(output))
            using (SKPaint font = new SKPaint() { TextSize = 40, Typeface = _typeface, IsAntialias = true })
            {
                int x = 0;
                int y = 10;
                canvas.DrawBitmap(DrawPillShapeTitle(title), x, y);
                x = 20;
                y = 70 + 40;
                for (int i = 0; i < texts.Count; i++)
                {
                    canvas.DrawText(texts[i], x, y, font);
                    y += 40;
                }
            }
            return output;
        }
        /// <summary>
        /// 画参数标题图标
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static SKBitmap DrawPillShapeTitle(string title)
        {
            SKPaint p = new SKPaint() { TextSize = 34, Typeface = _typeface };
            int width = (int)p.MeasureText(title) + 40;
            p.Dispose();
            int height = 40;
            SKBitmap sKBitmap = new SKBitmap(width, height);
            using (var canvas = new SKCanvas(sKBitmap))
            {
                canvas.Clear(SKColors.Transparent);
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Color = SKColors.LightBlue;
                    var leftCircleRect = SKRect.Create(0, 0, height, height);
                    canvas.DrawOval(leftCircleRect, paint);
                    var rectRect = SKRect.Create(height / 2, 0, width - height, height);
                    canvas.DrawRect(rectRect, paint);
                    var rightCircleRect = SKRect.Create(width - height, 0, height, height);
                    canvas.DrawOval(rightCircleRect, paint);
                    paint.Typeface = _typeface;
                    paint.TextSize = 34;
                    paint.Color = SKColors.White;
                    paint.TextAlign = SKTextAlign.Left;
                    canvas.DrawText(title, height / 2, height - 8, paint);
                }
            }
            return sKBitmap;
        }
        public static SKBitmap DrawPillShapeTitle(int height, int width, SKColor bgColor)
        {
            SKBitmap sKBitmap = new SKBitmap(width, height);
            using (var canvas = new SKCanvas(sKBitmap))
            {
                canvas.Clear(SKColors.Transparent);
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Color = bgColor;
                    var leftCircleRect = SKRect.Create(0, 0, height, height);
                    canvas.DrawOval(leftCircleRect, paint);
                    var rectRect = SKRect.Create(height / 2, 0, width - height, height);
                    canvas.DrawRect(rectRect, paint);
                    var rightCircleRect = SKRect.Create(width - height, 0, height, height);
                    canvas.DrawOval(rightCircleRect, paint);
                }
            }
            return sKBitmap;
        }
        /// <summary>
        /// 活动简图
        /// </summary>
        /// <param name="skEvnet"></param>
        /// <param name="needBonus"></param>
        /// <returns></returns>
        public static async Task<SKBitmap> DrawEventLogo(SkEvents skEvnet, bool needBonus)
        {
            SKBitmap eventBanner = await skEvnet.GetEventBanner();
            int width = 760;
            int height = 45 + 208;
            SKBitmap eventlogo = new SKBitmap(width, height);
            using (var canvas = new SKCanvas(eventlogo))
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.FilterQuality = SKFilterQuality.High;
                canvas.DrawBitmap(eventBanner, new SKRect(0, 0, 488, 208), paint);
                SKPaint font = new SKPaint() { Typeface = _typeface, IsAntialias = true };
                font.TextSize = 40;
                canvas.DrawText($"类型: {skEvnet.GetEventType()}    ID: {skEvnet.id}", 0, 208 + 40, font);
                if (needBonus)
                {
                    SKBitmap attr = SKBitmap.Decode($"./asset/normal/{skEvnet.GetBunusAttr()}.png");
                    canvas.DrawBitmap(attr, new SKRect(488 + 25, 0, 488 + 75, 50), paint);
                    canvas.DrawText($"+{(int)skEvnet.GetBunusAttRate()}%", 75 + 488 + 25, 36, font);
                    List<int> characterIds = skEvnet.GetBunusCharacters();
                    int charaIconY = 50;
                    int charaIconLeftX = 488 + 25;
                    for (int i = 0; i < characterIds.Count; i++)
                    {
                        if (charaIconLeftX + 50 > width)
                        {
                            charaIconY += 55;
                            charaIconLeftX = 488 + 25;
                        }
                        canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{characterIds[i]}.png"), new SKRect(charaIconLeftX, charaIconY, 50 + charaIconLeftX, charaIconY + 50), paint);
                        charaIconLeftX += 50;
                    }
                    if (charaIconLeftX + 100 > width)
                    {
                        charaIconY += 60;
                        charaIconLeftX = 488 + 25;
                    }
                    else
                    {
                        charaIconLeftX += 15;
                    }
                    canvas.DrawText($"+{(int)skEvnet.GetBunusCharacterRate()}%", charaIconLeftX, charaIconY + 36, font);
                }
            }
            eventBanner.Dispose();
            return eventlogo;
        }
        
        public static async Task<SKBitmap> DrawMusicCard(SkMusics music)
        {
            SKBitmap musicCard = new SKBitmap(750, 80);
            using (var canvas = new SKCanvas(musicCard))
            using (SKPaint font = new SKPaint() { Typeface = _typeface, IsAntialias = true, TextSize = 24, TextAlign = SKTextAlign.Left })
            using (SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High })
            {
                canvas.DrawText(music.id.ToString(), 0, 32, font);
                canvas.DrawBitmap(await music.GetMusicJacket(), new SKRect(55, 8, 55 + 64, 8 + 64), highQuality);
                canvas.DrawText(music.title, 125, 32, font);
                Dictionary<string, int> diffs = music.GetDifficulties();
                List<SKColor> colors = new List<SKColor>() { new SKColor(102, 221, 17), new SKColor(51, 187, 237), new SKColor(255, 170, 1), new SKColor(238, 69, 102), new SKColor(187, 51, 239),new SKColor(0,0,0) };
                font.TextAlign = SKTextAlign.Center;
                int i = 0;
                foreach (int diff in diffs.Values)
                {
                    canvas.DrawBitmap(DrawRoundWithText(diff.ToString(), 24, 23, colors[i]), 500 + 51 * i, 17);
                    i++;
                }
            }
            return musicCard;
        }
        /// <summary>
        /// 音乐简条
        /// </summary>
        /// <param name="music"></param>
        /// <returns></returns>
        public static async Task<SKBitmap> DrawMusicLine(SkMusics music)
        {
            SKBitmap musicImage = new SKBitmap(750,80);
            using (var canvas = new SKCanvas(musicImage))
            using (SKPaint font = new SKPaint() { Typeface = _typeface, IsAntialias = true, TextSize = 24, TextAlign = SKTextAlign.Left })
            {
                canvas.DrawText(music.id.ToString(), 0, 32, font);
                canvas.DrawBitmap(await music.GetMusicJacket(), new SKRect(55, 8, 55 + 64, 8 + 64), _highQuality);
                canvas.DrawText(music.title, 125, 32, font);
                List<int> diffs = music.GetDifficulties().Values.ToList();
                List<SKColor> colors = new List<SKColor>() { new SKColor(102, 221, 17), new SKColor(51, 187, 237), new SKColor(255, 170, 1), new SKColor(238, 69, 102), new SKColor(187, 51, 239) };
                font.TextAlign = SKTextAlign.Center;
                int x = 700;
                for(int i = diffs.Count - 1; i >= 0; i--)
                {
                    if (i == 5)
                    {
                        canvas.DrawBitmap(DrawGradientCircleWithText(diffs[5].ToString(), 24, 23, new SKColor(172, 143, 235), new SKColor(255, 121, 225)), x, 17);
                        x -= 50;
                    }
                    else
                    {
                        canvas.DrawBitmap(DrawRoundWithText(diffs[i].ToString(), 24, 23, colors[i]), x, 17);
                        x -= 50;
                    }
                }
            }
            return musicImage;
        }

        /// <summary>
        /// 绘制相关卡池，单张图为800*160
        /// </summary>
        /// <param name="gacha"></param>
        /// <returns></returns>
        public static async Task<SKBitmap> DrawGachaCard(SkGachas gacha)
        {
            SKBitmap gachaBanner = await gacha.GetGachaBanner();
            //int height = gacha.GetGachaType() == "生日" ? 160 : 149;
            SKBitmap gachaCard = new SKBitmap(350, 190);
            using (var canvas = new SKCanvas(gachaCard))
            using (var font = new SKPaint() { Typeface = _typeface, IsAntialias = true, TextSize = 25 })
            using (SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High })
            {
                canvas.DrawBitmap(gachaBanner, new SKRect(0, 0, 350, 160), highQuality);
                canvas.DrawText($"ID:  {gacha.id}", 0, 188, font);
            }
            gachaBanner.Dispose();
            return gachaCard;
        }
        static SKBitmap DrawRoundWithText(string text, int textSize, int radius, SKColor color)
        {
            SKBitmap bitmap = new SKBitmap(radius * 2, radius * 2);
            using (var canvas = new SKCanvas(bitmap))
            using (SKPaint font = new SKPaint() { Typeface = _typeface, IsAntialias = true, TextSize = textSize, TextAlign = SKTextAlign.Center })
            using (SKPaint paint = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High, Color = color })
            {
                canvas.DrawCircle(radius, radius, radius, paint);
                canvas.DrawText(text, radius, radius + textSize / 3, font);
            }
            return bitmap;
        }
        static SKBitmap DrawGradientCircleWithText(string text, int textSize, int radius, SKColor startColor, SKColor endColor)
        {
            SKBitmap result = new SKBitmap(radius * 2, radius * 2);
            SKBitmap origianl = new SKBitmap(radius * 2, radius * 2);

            using (SKCanvas rcanvas = new SKCanvas(result))
            using (SKCanvas ocanvas = new SKCanvas(origianl))
            {
                // 创建一个线性渐变
                SKShader shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),  // 渐变起点坐标
                    new SKPoint(radius * 2, radius * 2),  // 渐变终点坐标
                    new SKColor[] { new SKColor(172, 143, 235), new SKColor(255, 121, 225) },  // 渐变的颜色
                    null,  // 渐变颜色位置（null表示均匀分布）
                    SKShaderTileMode.Clamp);  // 渐变重复模式

                // 创建一个画刷，并设置为线性渐变
                SKPaint shaderPaint = new SKPaint
                {
                    Shader = shader
                };

                // 使用画刷填充整个画布
                ocanvas.DrawRect(new SKRect(0, 0, radius * 2, radius * 2), shaderPaint);

                SKPaint cropPaint = new SKPaint
                {
                    IsAntialias = true,
                    FilterQuality = SKFilterQuality.High,
                };

                // 创建一个圆形路径，以用作裁剪蒙版
                SKPath circlePath = new SKPath();
                circlePath.AddCircle(radius, radius, radius);

                rcanvas.ClipPath(circlePath, antialias: true);
                rcanvas.DrawBitmap(origianl, 0, 0, cropPaint);

                using (SKPaint font = new SKPaint() { Typeface = _typeface, IsAntialias = true, TextSize = textSize, TextAlign = SKTextAlign.Center })
                {
                    rcanvas.DrawText(text, radius, radius + textSize / 3, font);
                }
            }
            return result;
        }
        /// <summary>
        /// 画虚线
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static SKBitmap DrawDottedLine(int width, int height, float lineWidth = 5)
        {
            SKBitmap dottedline = new SKBitmap(width, height);
            using (var canvas = new SKCanvas(dottedline))
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Color = SKColors.LightGray;
                paint.StrokeWidth = lineWidth;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeCap = SKStrokeCap.Round;
                float[] intervals = new float[] { 1f, 20f };
                SKPath path = new SKPath();
                path.MoveTo(height / 2f, height / 2f);
                path.LineTo(width, height / 2f);
                paint.PathEffect = SKPathEffect.CreateDash(intervals, 0);
                canvas.DrawPath(path, paint);
            }
            return dottedline;
        }
        public static SKBitmap FixImageSize(SKBitmap image,int? fixWidth,int? fixHeight)
        {
            int width = 0;
            int height = 0;
            if(fixWidth!=null && fixHeight == null)
            {
                width = fixWidth.Value;
                height = (int)(width / (float)image.Width * image.Height);
            }
            if (fixWidth == null && fixHeight != null)
            {
                height = fixHeight.Value;
                width= (int)(height / (float)image.Height * image.Width);
            }
            if (fixWidth != null && fixHeight != null)
            {
                width = fixWidth.Value;
                height =fixHeight.Value;
            }
            SKBitmap fix = new SKBitmap(width,height);
            using(SKCanvas canvas = new SKCanvas(fix))
            {
                canvas.DrawBitmap(image,new SKRect(0, 0, width, height), _highQuality);
            }
            return fix;
        }
        static SKBitmap DrawVocals(List<SkMusicVocals.Characters> vocals)
        {
            int width = 20;
            int height = 50;
            SKPaint font = new SKPaint() { TextSize = 34, Typeface = _typeface, IsAntialias = true };
            for (int i = 0; i < vocals.Count; i++)
            {
                if (vocals[i].characterType == "game_character")
                {
                    width += 55;
                    if (i == vocals.Count - 1 && i > 0)
                    {
                        width -= 5;
                    }
                }
                if (vocals[i].characterType == "outside_character")
                {
                    width += (int)font.MeasureText(vocals[i].GetOutsideCharacterName()) + 5;
                    if (i == vocals.Count - 1 && i > 0)
                    {
                        width -= 5;
                    }
                }
            }
            SKBitmap vocalImage = new SKBitmap(width, height);
            using (SKCanvas canvas = new SKCanvas(vocalImage))
            using (SKPaint highQuality = new SKPaint() { FilterQuality = SKFilterQuality.High, IsAntialias = true })
            {
                canvas.DrawBitmap(DrawRoundWithText("", 10, 5, SKColors.Gray), 0, 20);
                for (int i = 0; i < vocals.Count; i++)
                {
                    if (vocals[i].characterType == "game_character")
                    {
                        canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{vocals[i].characterId}.png"), new SKRect(20 + 55 * i, 0, 70 + 55 * i, 50), highQuality);
                    }
                    if (vocals[i].characterType == "outside_character")
                    {
                        canvas.DrawText(vocals[i].GetOutsideCharacterName(), 20, 34, font);
                    }
                }
            }
            font.Dispose();
            return vocalImage;
        }
        /// <summary>
        /// 画牌子
        /// </summary>
        /// <param name="main"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static SKBitmap DrawDegreeHonor(SKBitmap main, int degree)
        {
            SKBitmap hornor = new SKBitmap(380, 80);
            using (var canvas = new SKCanvas(hornor))
            using (SKPaint paint = new SKPaint() { FilterQuality = SKFilterQuality.High })
            {
                canvas.DrawBitmap(main, new SKRect(0, 0, 380, 80), paint);
                if (degree <= 1000)
                {
                    canvas.DrawBitmap(SKBitmap.Decode("./asset/normal/degree_frame3.png"), 0, 0);
                }
                else
                {
                    canvas.DrawBitmap(SKBitmap.Decode("./asset/normal/degree_frame2.png"), 0, 0);
                }
                canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/degree_top{degree}.png"), 190, 0);
            }
            return hornor;
        }
        /// <summary>
        /// 添加背景以及指令标题
        /// </summary>
        /// <param name="image"></param>
        public static void DrawBackGroud(SKBitmap image, string commad)
        {
            using (SKCanvas canvas = new SKCanvas(image))
            using (SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High })
            {
                SKBitmap backGroud = SKBitmap.Decode("./asset/normal/searchBg.png");
                int bgHeight = (int)((float)image.Width / backGroud.Width * backGroud.Height);
                for (int y = 0; y < image.Height; y += bgHeight)
                {
                    canvas.DrawBitmap(backGroud, new SKRect(0, y, image.Width, y + bgHeight), highQuality);
                }
                canvas.DrawBitmap(DrawPillShapeTitle(70, 510, SKColors.White), 50, 90);
                canvas.DrawBitmap(DrawPillShapeTitle(50, 450, SKColors.LightBlue), 50, 50);
                canvas.DrawBitmap(SKBitmap.Decode("./asset/normal/Minori01.png"), new SKRect(556, 20, 775, 220), highQuality);
                using (SKPaint font = new SKPaint())
                {
                    font.Color = SKColors.White;
                    font.TextSize = 34;
                    font.Typeface = _typeface;
                    font.IsAntialias = true;
                    canvas.DrawText("查询", 80, 88, font);
                    font.TextSize = 40;
                    font.Color = SKColors.Black;
                    canvas.DrawText(commad, 100, 145, font);
                }
                using (SKPaint paint = new SKPaint())
                {
                    paint.Color = SKColors.White;
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawRoundRect(new SKRect(50, 200, image.Width - 50, image.Height - 50), 25, 25, paint);
                }
            }
        }
        /// <summary>
        /// 图片标题
        /// </summary>
        /// <param name="command"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static SKBitmap DrawImageTitle(string command,string content)
        {
            SKBitmap title = new SKBitmap(510, 110);
            using(SKCanvas canvas = new SKCanvas(title))
            {
                canvas.DrawBitmap(DrawPillShapeTitle(70, 510, SKColors.White), 0, 40);
                canvas.DrawBitmap(DrawPillShapeTitle(50, 450, SKColors.LightBlue), 0, 0);
                using(SKPaint font = new SKPaint())
                {
                    font.Color = SKColors.White;
                    font.TextSize = 34;
                    font.Typeface = _typeface;
                    font.IsAntialias = true;
                    canvas.DrawText(command, 30, 38, font);
                    font.TextSize = 40;
                    font.Color = SKColors.Black;
                    canvas.DrawText(content, 50, 93, font);
                }
            }
            return title;
        }
        /// <summary>
        /// 将SkBitmap转换成Base64
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static string ConvertBitmapToBase64(SKBitmap bitmap)
        {
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode())
            {
                using (var stream = new MemoryStream())
                {
                    data.SaveTo(stream);
                    return Convert.ToBase64String(stream.ToArray());
                }
            }
        }
        /// <summary>
        /// 字符串换行
        /// </summary>
        /// <param name="input"></param>
        /// <param name="interval">换行位置</param>
        /// <returns></returns>
        public static List<string> SplitString(string input, int textSize, int maxWidth)
        {
            List<string> result = new List<string>();
            using (SKPaint font = new SKPaint())
            {
                font.TextSize = textSize;
                font.Typeface = _typeface;
                font.IsAntialias = true;
                font.Color = SKColors.Black;
                float textWidth = font.MeasureText(input);
                int linesCount = (int)Math.Ceiling(textWidth / maxWidth);
                long charCount = 0;
                string text = input;
                for (int i = 0; i < linesCount; i++)
                {
                    charCount = font.BreakText(text, maxWidth);
                    result.Add(text.Substring(0, (int)charCount));
                    text = text.Remove(0, (int)charCount);
                }
            }
            return result;
        }
    }
}
