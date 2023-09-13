using Microsoft.Extensions.Logging;
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
    public class ImageCreater
    {
        /// <summary>
        /// 画查卡列表
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="isTrained"></param>
        /// <returns></returns>
        public async Task<string> DrawCardIconList(List<SkCard> cards,bool isTrained)
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
        public async Task<string> DrawCardInfo(SkCard card)
        {
            int width = 1000;
            int height = 0;
            int cardIllustrationImageCount = 0;
            Console.WriteLine($"正在生成卡面ID为{card.id}的图片");
            string assetDir = "./asset/normal";
            string logoDir = assetDir + $"/logo_{card.GetGroupNameById()}.png";
            string cardFrameDir = string.Empty;
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
            string skillDescription = card.GetSkillDescription();
            List<string> skillDescriptionList = SplitString(skillDescription, 25);
            SkEvents skEvent = card.GetEvent();
            int hasEvent = skEvent == null ? 0 : 1;
            //200标题,526一张插图与空隙，160编号，370综合力，140+40*技能描述行数,130招募语，130发布日期，266缩略图，370*hasEvent是否有活动图
            height = 200 + 526 * cardIllustrationImageCount + 160 + 370 + 140 + 40 * skillDescriptionList.Count + 130 + 130 + 266 + 370 * hasEvent;
            using (SKBitmap bitmap = new SKBitmap(1000, height))
            {
                using(SKCanvas canvas = new SKCanvas(bitmap))
                {
                    SKPaint highQuality = new SKPaint() { IsAntialias= true ,FilterQuality =SKFilterQuality.High};
                    canvas.Clear(SKColors.White);
                    SKPaint font = new SKPaint() { Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf"), IsAntialias = true };
                    //x,y为当前所操作区块的左上角坐标
                    int x = 100;
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

                    //画卡面插图 卡面大小900*506 星星大小48*47
                    x = 50;
                    y = 200;
                    List<SKBitmap> cardIllustrationImage = await card.GetCardIllustrationImage();
                    List<SKBitmap> cardIllustrationImage_afterCropping = new List<SKBitmap>();
                    for(int i =0;i<cardIllustrationImage.Count;i++)
                    {
                        cardIllustrationImage_afterCropping.Add(CropCardIllustrationImage(cardIllustrationImage[i]));
                    }
                    canvas.DrawBitmap(cardIllustrationImage_afterCropping[0], new SKRect(x, y, x + 900, y + 506), highQuality);
                    canvas.DrawBitmap(SKBitmap.Decode(cardFrameDir), new SKRect(x, y, x + 900, y + 506), highQuality);
                    canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{card.attr}.png"), new SKRect(x + 830, y, x + 900, y + 70), highQuality);
                    SKBitmap normal_star = SKBitmap.Decode("./asset/normal/normal_star.png");
                    //生日卡单独处理
                    if (starCount == 0)
                    {
                        canvas.DrawBitmap(SKBitmap.Decode("./asset/normal/birthday_star.png"), new SKRect(x + 18, y + 442, x + 18 + 48, y + 442 + 47), highQuality);
                        y = 726;
                    }
                    else
                    {
                        for (int i = 0; i < starCount; i++)
                        {
                            canvas.DrawBitmap(normal_star, new SKRect(x + 18, y + 442 - 47 * i, x + 18 + 48, y + 442 + 47 - 47 * i), highQuality);
                        }
                        y = 726;
                        //如果有特训后
                        if (cardIllustrationImage.Count > 1)
                        {
                            y = 200 + 506 + 20;
                            SKBitmap afterTrainingStar = SKBitmap.Decode("./asset/normal/after_training_star.png");
                            canvas.DrawBitmap(cardIllustrationImage_afterCropping[1], new SKRect(x, y, x + 900, y + 506), highQuality);
                            canvas.DrawBitmap(SKBitmap.Decode(cardFrameDir), new SKRect(x, y, x + 900, y + 506), highQuality);
                            canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{card.attr}.png"), new SKRect(x + 830, y, x + 900, y + 70), highQuality);
                            for (int i = 0; i < starCount; i++)
                            {
                                canvas.DrawBitmap(afterTrainingStar, new SKRect(x + 18, y + 442 - 47 * i, x + 18 + 48, y + 442 + 47 - 47 * i), highQuality);
                            }
                            y = 1252;
                        }
                    }
                    //卡面编号
                    canvas.DrawBitmap(DrawPillShapeTitle("编号"), x, y);
                    font.TextSize = 50;
                    canvas.DrawText(card.id.ToString(), x + 25, y + 50 + 60, font);
                    y += 110 + 50;
                    //综合力
                    canvas.DrawBitmap(DrawDottedLine(900, 5), x, y - 20);
                    canvas.DrawBitmap(DrawPillShapeTitle("综合力"), x, y);
                    font.TextSize = 45;
                    int[] power = card.GetPower();
                    canvas.DrawText($"综合力: {power[0] + power[1] + power[2]}  +  ({power[3] * 3 * 5})", x + 25, y + 50 + 45, font);
                    font.TextSize = 30;
                    using(SKPaint bar = new SKPaint())
                    {
                        bar.IsAntialias = true;
                        bar.Color = new SKColor(144, 238, 144);
                        canvas.DrawText($"表现力: {power[0]} + ({power[3] * 5})", x + 25, y + 95 + 30 + 10, font);
                        canvas.DrawRoundRect(new SKRect(x + 25, y + 135 + 10, x + 25 + power[0] / 15000f * 850f, y + 145 + 30), 10, 10, bar);
                        bar.Color = new SKColor(100, 149, 237);
                        canvas.DrawText($"技术力: {power[1]} + ({power[3] * 5})", x + 25, y + 175 + 30, font);
                        canvas.DrawRoundRect(new SKRect(x + 25, y + 205 + 10, x + 25 + power[1] / 15000f * 850f, y + 215 + 30), 10, 10, bar);
                        bar.Color = new SKColor(147, 112, 219);
                        canvas.DrawText($"活力: {power[2]} + ({power[3] * 5})", x + 25, y + 245 + 30, font);
                        canvas.DrawRoundRect(new SKRect(x + 25, y + 275 + 10, x + 25 + power[2] / 15000f * 850f, y + 285 + 30), 10, 10, bar);
                    }
                    y += 315 + 50;
                    //技能
                    canvas.DrawBitmap(DrawDottedLine(900, 5), x, y - 20);
                    canvas.DrawBitmap(DrawPillShapeTitle("技能"), x, y);
                    font.TextSize = 40;
                    canvas.DrawText(card.cardSkillName, x + 25, y + 50 + 40, font);
                    y = y + 90;
                    for (int i = 0; i < skillDescriptionList.Count; i++)
                    {
                        canvas.DrawText(skillDescriptionList[i], x + 25, y + 40, font);
                        y += 40;
                    }
                    y += 50;
                    //招募语
                    canvas.DrawBitmap(DrawDottedLine(900, 5), x, y - 20);
                    canvas.DrawBitmap(DrawPillShapeTitle("招募语"), x, y);
                    canvas.DrawText(card.gachaPhrase, x + 25, y + 50 + 40, font);
                    y += 130;
                    //发布日期
                    canvas.DrawBitmap(DrawDottedLine(900, 5), x, y - 20);
                    canvas.DrawBitmap(DrawPillShapeTitle("发布日期"), x, y);
                    DateTime releaseTime = DateTimeOffset.FromUnixTimeMilliseconds(card.releaseAt).DateTime;
                    canvas.DrawText(releaseTime.ToString("yyyy年MM月dd日 HH:mm"), x + 25, y + 50 + 40, font);
                    y += 130;
                    //缩略图
                    canvas.DrawBitmap(DrawDottedLine(900, 5), x, y - 20);
                    canvas.DrawBitmap(DrawPillShapeTitle("缩略图"), x, y);
                    canvas.DrawBitmap(await DrawCardIcon(card, false,false), x + 25, y + 50 + 20);
                    if (starCount > 2)
                    {
                        canvas.DrawBitmap(await DrawCardIcon(card, true,false), x + 20 + 156 + 20, y + 50 + 20);
                    }
                    y = y + 70 + 156 + 40;
                    //相关活动
                    if (skEvent != null)
                    {
                        canvas.DrawBitmap(DrawDottedLine(900, 5), x, y - 20);
                        canvas.DrawBitmap(DrawPillShapeTitle("相关活动"), x, y);
                        canvas.DrawBitmap(await DrawEventLogo(skEvent, true), x, y + 50);
                    }
                }
                Console.WriteLine("生成卡面信息图片成功");
                return ConvertBitmapToBase64(bitmap);
            }
        }
        /// <summary>
        /// 活动列表
        /// </summary>
        /// <param name="skEvents"></param>
        /// <returns></returns>
        public async Task<string> DrawEventList(List<SkEvents> skEvents)
        {

            return "";
        }
        /// <summary>
        /// 活动信息
        /// </summary>
        /// <param name="skEvent"></param>
        /// <returns></returns>
        public async Task<string> DrawEventInfo(SkEvents skEvent)
        {

            return "";
        }
        /// <summary>
        /// 画卡牌头像
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isTrained"></param>
        /// <returns></returns>
        public async Task<SKBitmap> DrawCardIcon(SkCard card, bool isTrained, bool needId)
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
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.IsAntialias = true;
                        paint.TextSize = 20;
                        paint.Color = SKColors.DarkGray;
                        paint.TextAlign = SKTextAlign.Left;
                        string text = "ID:" + card.id.ToString();
                        canvas.DrawText(text, 5, 175, paint);
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
        public Dictionary<int, Dictionary<string, List<SkCard>>> SortDictionary(Dictionary<int, Dictionary<string, List<SkCard>>> cards)
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
        public int GetAttrId(string attr)
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
        public SKBitmap CropCardIllustrationImage(SKBitmap input)
        {
            SKRectI cropRect = new SKRectI(0, 54, input.Width, input.Height - 55);
            SKBitmap output = new SKBitmap(cropRect.Width, cropRect.Height);
            using (SKCanvas canvas = new SKCanvas(output))
            {
                canvas.DrawBitmap(input, cropRect, new SKRectI(0, 0, output.Width, output.Height));
                return output;
            }
        }
        /// <summary>
        /// 画参数标题图标
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public SKBitmap DrawPillShapeTitle(string title)
        {
            int width = title.Length * 40 + 50;
            int height = 50;
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
                    paint.TextSize = 40;
                    paint.Color = SKColors.White;
                    paint.TextAlign = SKTextAlign.Left;
                    canvas.DrawText(title, height / 2, height - 12, paint);
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
        public async Task<SKBitmap> DrawEventLogo(SkEvents skEvnet ,bool needBonus)
        {
            SKBitmap eventface = await skEvnet.GetEventLogo();
            int width = 900;
            int height = 25+45+eventface.Height;
            SKBitmap eventlogo = new SKBitmap(width, height);
            using(var canvas = new SKCanvas(eventlogo))
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.FilterQuality = SKFilterQuality.High;
                canvas.DrawBitmap(eventface, 25, 25, paint);
                SKPaint font = new SKPaint() { Typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf"), IsAntialias = true };
                font.TextSize = 40;
                canvas.DrawText($"类型: {skEvnet.eventType}    ID: {skEvnet.id}", 25, eventface.Height + 25 + 40, font);
                if (needBonus)
                {
                    SKBitmap attr = SKBitmap.Decode($"./asset/normal/{skEvnet.GetBunusAttr()}.png");
                    canvas.DrawBitmap(attr, new SKRect(25 + eventface.Width + 25, 25, 25 + eventface.Width + 75, 75), paint);
                    canvas.DrawText($"+{(int)skEvnet.GetBunusAttRate()}%", 100 + eventface.Width + 25, 25 + 40, font);
                    List<int> characterIds = skEvnet.GetBunusCharacters();
                    int charaIconY = 100;
                    int charaIconLeftX = eventface.Width + 50;
                    for (int i = 0; i < characterIds.Count; i++)
                    {
                        if (charaIconLeftX + 50 > width)
                        {
                            charaIconY += 55;
                            charaIconLeftX = eventface.Width + 50;
                        }
                        canvas.DrawBitmap(SKBitmap.Decode($"./asset/normal/{characterIds[i]}.png"), new SKRect(charaIconLeftX, charaIconY, 50 + charaIconLeftX, charaIconY + 50), paint);
                        charaIconLeftX += 55;
                    }
                    if (charaIconLeftX + 100 > width)
                    {
                        charaIconY += 60;
                        charaIconLeftX = eventface.Width + 50;
                    }
                    else
                    {
                        charaIconLeftX += 25;
                    }
                    canvas.DrawText($"+{(int)skEvnet.GetBunusCharacterRate()}%", charaIconLeftX, charaIconY + 40, font);
                }
            }
            return eventlogo;
        }
        /// <summary>
        /// 画虚线
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public SKBitmap DrawDottedLine(int width,int height)
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
        /// 将SkBitmap转换成Base64
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public string ConvertBitmapToBase64(SKBitmap bitmap)
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
        public List<string> SplitString(string input, int interval)
        {
            List<string> result = new List<string>();
            string temp = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                temp += input[i];

                if (temp.Length == interval || i == input.Length - 1)
                {
                    result.Add(temp);
                    temp = string.Empty;
                }
            }

            return result;
        }
    }
}
