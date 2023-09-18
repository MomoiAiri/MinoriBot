﻿using Microsoft.Extensions.Logging;
using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils
{
    public static class ImageCreater
    {
        /// <summary>
        /// 画查卡列表
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="isTrained"></param>
        /// <returns></returns>
        public static async Task<string> DrawCardIconList(List<SkCard> cards,bool isTrained)
        {
            string fileDirectory = "./asset/normal";
            //string result = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+".png";
            string base64String = string.Empty;
            //根据角色ID与属性进行分类
            Dictionary<int,Dictionary<string,List<SkCard>>> classifiedCards = new Dictionary<int, Dictionary<string, List<SkCard>>>();
            foreach (SkCard card in cards)
            {
                if (!classifiedCards.ContainsKey(card.characterId))
                {
                    classifiedCards[card.characterId] = new Dictionary<string, List<SkCard>>();
                }
                if (!classifiedCards[card.characterId].ContainsKey(card.attr))
                {
                    classifiedCards[card.characterId][card.attr] = new List<SkCard>();
                }
                classifiedCards[card.characterId][card.attr].Add(card);
            }
            int line = 0;//输出图片的行数
            int maxLineCount = 0;
            classifiedCards = SortDictionary(classifiedCards);
            foreach (var kvp in classifiedCards)
            {
                Console.WriteLine($"ID: {kvp.Key}");
                foreach (var attrKvp in kvp.Value)
                {
                    Console.WriteLine($"  Attr: {attrKvp.Key}");
                    //对分类好的卡组进行星级排序，高星在前
                    attrKvp.Value.Sort((card1, card2) => card2.GetStarsCount().CompareTo(card1.GetStarsCount()));
                    line ++;
                    int countTemp = 0;
                    foreach (var card in attrKvp.Value)
                    {
                        Console.WriteLine($"    Name: {card.prefix}_{card.GetStarsCount()}星");
                        countTemp ++;
                    }
                    if (countTemp > maxLineCount)
                    {
                        maxLineCount = countTemp;
                    }
                }
            }
            Console.WriteLine($"需要输出{line}行图片，最长的一行有{maxLineCount}张卡");
            int width = 168 + (156 + 15) * maxLineCount;
            int height = 40 + (180 + 15) * line - 15;
            Console.WriteLine("开始生成图片");
            using(SKBitmap bitmap = new SKBitmap(width, height))
            {
                using(SKCanvas canvas = new SKCanvas(bitmap))
                {
                    int x = 20;
                    int y = 20;
                    canvas.Clear(SKColors.White); 
                    foreach(var kvp in classifiedCards)
                    {
                        //画角色头图
                        canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/{kvp.Key}.png"), new SKRect(x, y, x+128, y+128));
                        x = 163;
                        foreach(var attrKvp in kvp.Value)
                        {
                            foreach(var card in attrKvp.Value)
                            {
                                canvas.DrawBitmap(await DrawCardIcon(card, isTrained,true), new SKRect(x, y, x + 156, y + 180));
                                x = x + 156 + 15;
                            }
                            x = 163;
                            y = y + 180 + 15;
                        }
                        x = 20;
                    }
                    base64String = ConvertBitmapToBase64(bitmap);
                }

            }
            return base64String;
        }
        public static async Task<string> DrawCardInfo(SkCard card)
        {
            int width = 900;
            int height = 0;
            int cardIllustrationImageCount = 0;
            Console.WriteLine($"正在生成卡面ID为{card.id}的图片");
            string assetDir = "./asset/normal";
            string logoDir = assetDir + $"/logo_{card.GetGroupNameById()}.png";
            string cardFrameDir = string.Empty;
            string cardType = string.Empty;
            int starCount = card.GetStarsCount();
            if (starCount > 0)
            {
                cardFrameDir = assetDir + $"/cardFrame_{starCount}.png";
            }
            else
            {
                cardFrameDir = assetDir + "/cardFrame_bd.png";
            }
            cardIllustrationImageCount = starCount > 2 ? 2 : 1;
            //计算卡面信息所需要的图片高度
            string skillDescription = card.GetSkillDescription().Replace("\n","");
            List<string> skillDescriptionList = SplitString(skillDescription, 40,750);
            SkEvents skEvent = card.GetEvent();
            int hasEvent = skEvent == null ? 0 : 1;
            List<SkGachas> gachas = card.GetGachas(ref cardType);
            int hasGachas = gachas.Count > 0 ? 1 : 0;
            //200标题,450一张插图与空隙，150编号，360综合力，130+40*技能描述行数,130招募语，130发布日期，256缩略图，360*hasEvent是否有活动图
            height = 200 + 450 * cardIllustrationImageCount + 150 + 360 + 130 + 40 * skillDescriptionList.Count + 130 + 130 + 266 + 360 * hasEvent + 260 * hasGachas + 70;
            SKBitmap cardInfo = new SKBitmap(width, height);
            using (SKCanvas canvas = new SKCanvas(cardInfo))
            using(SKPaint font =new SKPaint() { Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf"), IsAntialias = true })
            using(SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High })
            {
                canvas.Clear(SKColors.White);
                //x,y为当前所操作区块的左上角坐标
                int x = 50;
                int y = 50;
                //标题背景颜色填充
                using (SKPaint paint = new SKPaint())
                {
                    paint.Color = new SKColor(235, 235, 235);
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawRect(new SKRect(x, y, x + 800, y + 130), paint);
                }

                //团体logo
                SKBitmap dottedLine = DrawDottedLine(800, 5);
                SKBitmap logoImage = SKBitmap.Decode(logoDir);
                float logo_y = 260f / logoImage.Width * logoImage.Height + 60f;
                canvas.DrawBitmap(logoImage, new SKRect(150, 60, 410, logo_y), highQuality);

                //卡牌名与角色名
                font.TextSize = 30;
                font.Color = SKColors.Black;
                font.TextAlign = SKTextAlign.Left;
                string text = card.prefix;
                canvas.DrawText(text, 470, 100, font);
                font.TextSize = 32;
                text = NickName.idToName[card.characterId];
                canvas.DrawText(text, 470, 145, font);

                //画卡面插图 卡面大小800*450 星星大小40*39
                x = 50;
                y = 200;
                List<SKBitmap> cardIllustrationImage = await card.GetCardIllustrationImage();
                List<SKBitmap> cardIllustrationImage_afterCropping = new List<SKBitmap>();
                for (int i = 0; i < cardIllustrationImage.Count; i++)
                {
                    cardIllustrationImage_afterCropping.Add(CropCardIllustrationImage(cardIllustrationImage[i]));
                }
                canvas.DrawBitmap(cardIllustrationImage_afterCropping[0], new SKRect(x, y, x + 800, y + 450), highQuality);
                canvas.DrawBitmap(SKBitmap.Decode(cardFrameDir), new SKRect(x, y, x + 800, y + 450), highQuality);
                canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{card.attr}.png"), new SKRect(x + 740, y, x + 800, y + 60), highQuality);
                SKBitmap normal_star = SKBitmap.Decode("./asset/normal/normal_star.png");
                //生日卡单独处理
                if (starCount == 0)
                {
                    canvas.DrawBitmap(SKBitmap.Decode("./asset/normal/birthday_star.png"), new SKRect(x + 18, y + 393, x + 18 + 40, y + 393 + 39), highQuality);
                    y = 200 + 450 + 20;
                }
                else
                {
                    for (int i = 0; i < starCount; i++)
                    {
                        canvas.DrawBitmap(normal_star, new SKRect(x + 18, y + 393 - 39 * i, x + 18 + 40, y + 393 + 39 - 39 * i), highQuality);
                    }
                    y = 200 + 450 + 20;
                    //如果有特训后
                    if (cardIllustrationImage.Count > 1)
                    {
                        y = 200 + 450 + 20;
                        SKBitmap afterTrainingStar = SKBitmap.Decode("./asset/normal/after_training_star.png");
                        canvas.DrawBitmap(cardIllustrationImage_afterCropping[1], new SKRect(x, y, x + 800, y + 450), highQuality);
                        canvas.DrawBitmap(SKBitmap.Decode(cardFrameDir), new SKRect(x, y, x + 800, y + 450), highQuality);
                        canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{card.attr}.png"), new SKRect(x + 740, y, x + 800, y + 60), highQuality);
                        for (int i = 0; i < starCount; i++)
                        {
                            canvas.DrawBitmap(afterTrainingStar, new SKRect(x + 18, y + 393 - 39 * i, x + 18 + 40, y + 393 + 39 - 39 * i), highQuality);
                        }
                        y = 200 + 450 * 2 + 20 * 2;
                    }
                }
                //卡面编号
                canvas.DrawBitmap(DrawPillShapeTitle("编号"), x, y);
                font.TextSize = 40;
                canvas.DrawText(card.id.ToString(), x + 25, y + 40 + 50, font);
                //类型
                canvas.DrawBitmap(DrawPillShapeTitle("类型"), x + 400, y);
                canvas.DrawText(cardType, x + 25 + 400, y + 40 + 50, font);
                //综合力
                y += 100 + 40;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("综合力"), x, y);
                font.TextSize = 45;
                int[] power = card.GetPower();
                canvas.DrawText($"综合力: {power[0] + power[1] + power[2]}  +  ({power[3] * 3 * 5})", x + 25, y + 50 + 45, font);
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
                y += 315 + 40;
                //技能
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("技能"), x, y);
                font.TextSize = 40;
                canvas.DrawText(card.cardSkillName, x + 25, y + 50 + 40, font);
                y = y + 90;
                for (int i = 0; i < skillDescriptionList.Count; i++)
                {
                    canvas.DrawText(skillDescriptionList[i], x + 25, y + 40, font);
                    y += 40;
                }
                y += 40;
                //招募语
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("招募语"), x, y);
                canvas.DrawText(card.gachaPhrase, x + 25, y + 50 + 40, font);
                y += 130;
                //发布日期
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("发布日期"), x, y);
                DateTime releaseTime = DateTimeOffset.FromUnixTimeMilliseconds(card.releaseAt).LocalDateTime;
                canvas.DrawText(releaseTime.ToString("yyyy年MM月dd日 HH:mm"), x + 25, y + 50 + 40, font);
                y += 130;
                //缩略图
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("缩略图"), x, y);
                canvas.DrawBitmap(await DrawCardIcon(card, false, false), x + 25, y + 40 + 20);
                if (starCount > 2)
                {
                    canvas.DrawBitmap(await DrawCardIcon(card, true, false), x + 20 + 156 + 20, y + 40 + 20);
                }
                y = y + 60 + 156 + 40;
                //相关活动
                if (skEvent != null)
                {
                    canvas.DrawBitmap(dottedLine, x, y - 20);
                    canvas.DrawBitmap(DrawPillShapeTitle("相关活动"), x, y);
                    canvas.DrawBitmap(await DrawEventLogo(skEvent, true), x, y + 60);
                }
                if (hasGachas != 0)
                {
                    y += 360;
                    for(int i =0;i<gachas.Count;i++)
                    {
                        canvas.DrawBitmap(dottedLine, x, y - 20);
                        canvas.DrawBitmap(DrawPillShapeTitle("相关卡池"), x, y);
                        if (i % 2 == 0)
                        {
                            canvas.DrawBitmap(await DrawGachaCard(gachas[i]), x + 25, y + 60);
                        }
                        else
                        {
                            canvas.DrawBitmap(await DrawGachaCard(gachas[i]), x + 25 + 25 + 350, y + 60);
                            y += 60;
                        }
                    }
                }
                using (SKPaint mark = new SKPaint())
                {
                    mark.TextAlign = SKTextAlign.Center;
                    mark.TextSize = 25;
                    mark.Color = SKColors.Gray;
                    mark.IsAntialias = true;
                    mark.Typeface = font.Typeface;
                    canvas.DrawText("Created By MinoriBot @GitHub", width / 2, height - 15, mark);
                }
                dottedLine.Dispose();
            }
            Console.WriteLine("生成卡面信息图片成功");
            return ConvertBitmapToBase64(cardInfo);
        }
        /// <summary>
        /// 活动列表
        /// </summary>
        /// <param name="skEvents"></param>
        /// <returns></returns>
        public static async Task<string> DrawEventList(List<SkEvents> skEvents)
        {
            int width = 900;
            int height = 80;
            List<SKBitmap> eventImages = new List<SKBitmap>();
            for(int i = 0; i < skEvents.Count; i++)
            {
                eventImages.Add(await DrawSimpleEventImage(skEvents[i]));
                height += 30 + eventImages[i].Height;
            }
            SKBitmap eventList = new SKBitmap(width,height);
            using(var canvas = new SKCanvas(eventList))
            {
                canvas.Clear(SKColors.White);
                int x = 50;
                int y = 50;
                SKBitmap dottedLine = DrawDottedLine(800, 5);
                SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
                for (int i=0;i<eventImages.Count; i++)
                {
                    canvas.DrawBitmap(eventImages[i], x, y, highQuality);
                    canvas.DrawBitmap(dottedLine, x, y + eventImages[i].Height +10);
                    y += eventImages[i].Height +30;
                }
            }
            Console.WriteLine("活动列表绘制完毕");
            return ConvertBitmapToBase64(eventList);
        }
        /// <summary>
        /// 活动信息
        /// </summary>
        /// <param name="skEvent"></param>
        /// <returns></returns>
        public static async Task<string> DrawEventInfo(SkEvents skEvent)
        {
            int width = 900;
            int height = 0;
            List<SkMusics> currentMusics = skEvent.GetEventMusics();
            List<SkGachas> gachas = skEvent.GetGachasInEvent();
            List<SKBitmap> gachaImages = new List<SKBitmap>();
            int gachaHeight = (int)Math.Ceiling(gachas.Count/2f) * 200;
            for (int i = 0; i < gachas.Count; i++)
            {
                gachaImages.Add(await DrawGachaCard(gachas[i]));
            }
            
            //430大图,130单行文字+标题，280卡牌，100+90n相关歌曲,50+gachaHeight相关卡池，50底部留白
            height = 430 + 130 + 130 + 130 + 130 + 130 + 130 + 220 + 280 + 280 + 100 + currentMusics.Count * 90 + 50 + gachaHeight + 50;
            SKBitmap eventInfo = new SKBitmap(width,height);
            using(var canvas = new SKCanvas(eventInfo))
            {
                canvas.Clear(SKColors.White);
                SKBitmap dottedLine = DrawDottedLine(800, 5);
                SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
                SKPaint font = new SKPaint() { Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf"), IsAntialias = true, TextSize = 40 };
                int x = 50;
                int y = 50;
                canvas.DrawBitmap(await skEvent.GetEventBanner(), new SKRect(x, y, x + 800, x + 341), highQuality);
                y = 50 + 340 + 40;
                canvas.DrawBitmap(DrawPillShapeTitle("活动名称"), x, y);
                canvas.DrawText(skEvent.name, x + 25, y + 50 + 40, font);
                y += 90 + 40;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("类型"), x, y);
                canvas.DrawText(skEvent.GetEventType(), x + 25, y + 50 + 40, font);
                canvas.DrawBitmap(DrawPillShapeTitle("ID"), x + 400, y);
                canvas.DrawText(skEvent.id.ToString(), x + 400 + 25, y + 50 + 40, font);
                y += 90 + 40;
                DateTime startTime = Utils.TimeStampToDateTime(skEvent.startAt);
                DateTime endTime = Utils.TimeStampToDateTime(skEvent.aggregateAt);
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("开始时间"), x, y);
                canvas.DrawText(startTime.ToString("yyyy年MM月dd日 HH:mm"), x + 25, y + 50 + 40, font);
                y += 90 + 40;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("结束时间"), x, y);
                canvas.DrawText(endTime.ToString("yyyy年MM月dd日 HH:mm"), x + 25, y + 50 + 40, font);
                y += 90 + 40;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("活动属性加成"), x, y);
                canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{skEvent.GetBunusAttr()}.png"), new SKRect(x + 25, y + 50 + 10, x + 25 + 40, y + 50 + 10 + 40), highQuality);
                canvas.DrawText($"+{skEvent.GetBunusAttRate().ToString("F0")}%", x + 80, y + 50 + 40, font);
                y += 90 + 40;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("活动角色加成"), x, y);
                List<int> bonusCharacters = skEvent.GetBunusCharacters();
                for (int i = 0; i < bonusCharacters.Count; i++)
                {
                    canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{bonusCharacters[i]}.png"), new SKRect(x + 25 + i * 45, y + 50 + 10, x + 25 + 40 + i * 45, y + 50 + 10 + 40), highQuality);
                }
                canvas.DrawText($"+{skEvent.GetBunusCharacterRate().ToString("F0")}%", x + 25 + bonusCharacters.Count * 45 + 20, y + 50 + 40, font);
                y += 90 + 40;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("活动奖励"), x, y);
                List<int> degrees = new List<int>() { 1, 2, 3, 10, 100, 1000 };
                int tempX = x + 25;
                int tempY = y + 40 + 20;
                for(int i = 0; i < 6; i++)
                {
                    if (i % 3 == 0 && i != 0)
                    {
                        tempY += 70;
                        tempX = x + 25;
                    }
                    canvas.DrawBitmap(DrawDegreeHonor(await skEvent.GetEventDegreeMain(), degrees[i]), new SKRect(tempX, tempY, tempX + 230, tempY + 48), highQuality);
                    tempX += 250;
                }
                y += 40 + 20 + 70 + 50 + 40;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("奖励卡牌"), x, y);
                List<SkCard> cards = skEvent.GetCurrentCards();
                int tempI = 0;
                for(int i = 0; i < cards.Count; i++)
                {
                    if (cards[i].GetStarsCount() < 4)
                    {
                        canvas.DrawBitmap(await DrawCardIcon(cards[i], true, true), x + 25 + 165 * tempI, y + 60);
                        tempI++;
                    }
                }
                y += 60 + 180 + 40;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("活动期间卡池卡牌"), x, y);
                tempI = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i].GetStarsCount() == 4)
                    {
                        canvas.DrawBitmap(await DrawCardIcon(cards[i], true, true), x + 25 +165 * tempI, y + 60);
                        tempI++;
                    }
                }
                y += 60 + 180 + 40;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("活动相关歌曲"), x, y);
                int musicY = y + 60;
                for(int i =0;i<currentMusics.Count;i++)
                {
                    canvas.DrawBitmap(await DrawMusicCard(currentMusics[i]), x, musicY);
                    if (i != currentMusics.Count - 1)
                    {
                        canvas.DrawBitmap(dottedLine, x, musicY + 90 - 7.5f);
                    }
                    musicY += 90;
                }
                y += 60 + 90 * currentMusics.Count + 40;
                canvas.DrawBitmap(dottedLine, x, y - 20);
                canvas.DrawBitmap(DrawPillShapeTitle("活动相关卡池"), x, y);
                int gachaY = y + 60;
                for(int i =0;i<gachaImages.Count;i++)
                {
                    if (i % 2 == 0)
                    {
                        canvas.DrawBitmap(gachaImages[i], x + 25, gachaY);
                    }
                    if (i % 2 == 1)
                    {
                        canvas.DrawBitmap(gachaImages[i], x + 25 + 350 + 25, gachaY);
                        gachaY += gachaImages[i].Height + 10;
                    }
                }
                
                //水印
                using (SKPaint mark =new SKPaint())
                {
                    mark.TextAlign= SKTextAlign.Center;
                    mark.TextSize = 30;
                    mark.Color = SKColors.Gray;
                    mark.IsAntialias = true;
                    mark.Typeface = font.Typeface;
                    canvas.DrawText("Created By MinoriBot @GitHub", width / 2, height - 15, mark);
                }
            }
            Console.WriteLine("活动信息图绘制完成");
            return ConvertBitmapToBase64(eventInfo);
        }
        public static async Task<SKBitmap> DrawSimpleEventImage(SkEvents skEvent)
        {
            int width = 800;
            List<int> bonusChara = skEvent.GetBunusCharacters();
            List<SkCard> currentCards = skEvent.GetCurrentCards();
            int height = 150 + 10 + 150 * (int)Math.Ceiling(currentCards.Count/5.0);
            SKBitmap eventImage = new SKBitmap(width,height);
            SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
            SKPaint font = new SKPaint() { Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf"), IsAntialias = true, TextSize = 23 };
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
                for(int i = 0; i < bonusChara.Count; i++)
                {
                    canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{bonusChara[i]}.png"), new SKRect(370 + i * 35, 115, 400 + i * 35, 145), highQuality);
                }
                canvas.DrawText("+" + skEvent.GetBunusCharacterRate().ToString("F0") + "%", 370 + 35 * bonusChara.Count + 20, 138, font);
                int x = 0;
                int y = 160;
                for(int i = 0; i < currentCards.Count; i++)
                {
                    if (i % 5 ==0)
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
            SKBitmap bitmap = new SKBitmap(156, 180);
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
                //添加文字id
                if (needId)
                {
                    using (SKPaint font = new SKPaint())
                    {
                        font.IsAntialias = true;
                        font.TextSize = 20;
                        font.Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf");
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
        /// 对查询到的所有卡牌进行排序
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static Dictionary<int, Dictionary<string, List<SkCard>>> SortDictionary(Dictionary<int, Dictionary<string, List<SkCard>>> cards)
        {
            Dictionary<int, Dictionary<string, List<SkCard>>> result = new Dictionary<int, Dictionary<string, List<SkCard>>>();
            List<int> sortKeys = cards.Keys.ToList();
            sortKeys.Sort();
            foreach(int key in sortKeys)
            {
                result.Add(key, new Dictionary<string, List<SkCard>>());
                List<string> attrList = cards[key].Keys.ToList();
                attrList.Sort((attr1, attr2) => GetAttrId(attr1).CompareTo(GetAttrId(attr2)));
                foreach (string strkey in attrList)
                {
                    result[key].Add(strkey, cards[key][strkey]);
                }

            } 
            return result;
        }
        public static int GetAttrId(string attr)
        {
            if (attr == "cool") return 1;
            if (attr == "cute") return 2;
            if (attr == "happy") return 3;
            if (attr == "mysterious") return 4;
            if (attr == "pure") return 5;
            return 0;
        }
        /// <summary>
        /// 对卡面插图进行裁剪
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static SKBitmap CropCardIllustrationImage(SKBitmap input)
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
        /// <summary>
        /// 画参数标题图标
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static SKBitmap DrawPillShapeTitle(string title)
        {
            int width = title.Length * 34 + 40;
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
                    paint.Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf");
                    paint.TextSize = 34;
                    paint.Color = SKColors.White;
                    paint.TextAlign = SKTextAlign.Left;
                    canvas.DrawText(title, height / 2, height - 8, paint);
                }
                return sKBitmap;
            }
        }
        /// <summary>
        /// 活动简图
        /// </summary>
        /// <param name="skEvnet"></param>
        /// <param name="needBonus"></param>
        /// <returns></returns>
        public static async Task<SKBitmap> DrawEventLogo(SkEvents skEvnet ,bool needBonus)
        {
            SKBitmap eventBanner = await skEvnet.GetEventBanner();
            int width = 800;
            int height = 45+ eventBanner.Height;
            SKBitmap eventlogo = new SKBitmap(width, height);
            using(var canvas = new SKCanvas(eventlogo))
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.FilterQuality = SKFilterQuality.High;
                canvas.DrawBitmap(eventBanner, 25, 0, paint);
                SKPaint font = new SKPaint() { Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf"), IsAntialias = true };
                font.TextSize = 40;
                canvas.DrawText($"类型: {skEvnet.GetEventType()}    ID: {skEvnet.id}", 25, eventBanner.Height + 40, font);
                if (needBonus)
                {
                    SKBitmap attr = SKBitmap.Decode($"./asset/normal/{skEvnet.GetBunusAttr()}.png");
                    canvas.DrawBitmap(attr, new SKRect(25 + eventBanner.Width + 25, 0, 25 + eventBanner.Width + 75, 50), paint);
                    canvas.DrawText($"+{(int)skEvnet.GetBunusAttRate()}%", 100 + eventBanner.Width + 25, 40, font);
                    List<int> characterIds = skEvnet.GetBunusCharacters();
                    int charaIconY = 50;
                    int charaIconLeftX = eventBanner.Width + 50;
                    for (int i = 0; i < characterIds.Count; i++)
                    {
                        if (charaIconLeftX + 50 > width)
                        {
                            charaIconY += 55;
                            charaIconLeftX = eventBanner.Width + 50;
                        }
                        canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{characterIds[i]}.png"), new SKRect(charaIconLeftX, charaIconY, 50 + charaIconLeftX, charaIconY + 50), paint);
                        charaIconLeftX += 50;
                    }
                    if (charaIconLeftX + 100 > width)
                    {
                        charaIconY += 60;
                        charaIconLeftX = eventBanner.Width + 50;
                    }
                    else
                    {
                        charaIconLeftX += 15;
                    }
                    canvas.DrawText($"+{(int)skEvnet.GetBunusCharacterRate()}%", charaIconLeftX, charaIconY + 32, font);
                }
            }
            eventBanner.Dispose();
            return eventlogo;
        }
        public static async Task<SKBitmap> DrawMusicCard(SkMusics music)
        {
            SKBitmap musicCard = new SKBitmap(800, 80);
            using(var canvas = new SKCanvas(musicCard))
            using (SKPaint font = new SKPaint() { Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf"), IsAntialias = true,TextSize = 24 ,TextAlign = SKTextAlign.Left})
            using (SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High })
            {
                canvas.DrawText(music.id.ToString(), 25, 32, font);
                canvas.DrawBitmap(await music.GetMusicJacket(), new SKRect(80, 8, 80 + 64, 8 + 64), highQuality);
                canvas.DrawText(music.title, 150, 32, font);
                List<int> diff = music.GetDifficulties();
                List<SKColor> colors = new List<SKColor>() { new SKColor(102, 221, 17), new SKColor(51, 187, 237), new SKColor(255, 170, 1), new SKColor(238, 69, 102), new SKColor(187, 51, 239) };
                font.TextAlign = SKTextAlign.Center;
                for(int i = 0; i < diff.Count; i++)
                {
                    canvas.DrawBitmap(DrawRoundWithText(diff[i].ToString(), 24, 23, colors[i]), 500+51*i, 17);
                }
            }
            return musicCard;
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
            using(var canvas =new SKCanvas(gachaCard))
            using(var font =new SKPaint() { Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf"), IsAntialias = true, TextSize = 25 })
            using (SKPaint highQuality = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High })
            {
                canvas.DrawBitmap(gachaBanner, new SKRect(0, 0, 350, 160), highQuality);
                canvas.DrawText($"ID:  {gacha.id}", 0, 188, font);
            }
            gachaBanner.Dispose();
            return gachaCard;
        }
        static SKBitmap DrawRoundWithText(string text,int textSize,int radius,SKColor color)
        {
            SKBitmap bitmap = new SKBitmap(radius * 2, radius * 2);
            using (var canvas = new SKCanvas(bitmap))
            using (SKPaint font = new SKPaint() { Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf"), IsAntialias = true, TextSize = textSize ,TextAlign = SKTextAlign.Center})
            using (SKPaint paint = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High ,Color = color})
            {
                canvas.DrawCircle(radius, radius, radius, paint);
                canvas.DrawText(text, radius , radius + textSize / 3, font);
            }
            return bitmap;
        }
        /// <summary>
        /// 画虚线
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static SKBitmap DrawDottedLine(int width,int height)
        {
            SKBitmap dottedline = new SKBitmap(width, height);
            using(var canvas = new SKCanvas(dottedline))
            using(var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Color = SKColors.LightGray;
                paint.StrokeWidth = height;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeCap = SKStrokeCap.Round;
                float[] intervals = new float[] { 1, 20 };
                SKPath path =new SKPath();
                path.MoveTo(height/2f, height / 2f);
                path.LineTo(width, height / 2f);
                paint.PathEffect = SKPathEffect.CreateDash(intervals, 0);
                canvas.DrawPath(path, paint);
            }
            return dottedline;
        }
        /// <summary>
        /// 画牌子
        /// </summary>
        /// <param name="main"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static SKBitmap DrawDegreeHonor(SKBitmap main,int degree)
        {
            using(var canvas = new SKCanvas(main))
            {
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
            return main;
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
        public static List<string> SplitString(string input, int textSize ,int maxWidth)
        {
            List<string> result = new List<string>();
            using(SKPaint font =new SKPaint())
            {
                font.TextSize = textSize;
                font.Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf");
                font.IsAntialias = true;
                font.Color = SKColors.Black;
                float textWidth = font.MeasureText(input);
                int linesCount = (int)Math.Ceiling(textWidth / maxWidth);
                long charCount = 0;
                string text = input;
                for(int i =0;i< linesCount;i++)
                {
                    charCount = font.BreakText(text, maxWidth);
                    result.Add(text.Substring(0, (int)charCount));
                    text =text.Remove(0, (int)charCount);
                }
            }
            return result;
        }
    }
}
