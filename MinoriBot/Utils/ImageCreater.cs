using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
                                canvas.DrawBitmap(await DrawCardIcon(card, isTrained), new SKRect(x, y, x + 156, y + 180));
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
            Console.WriteLine($"正在生成卡面ID为{card.id}的图片");
            string assetDir = "./asset/normal";
            string logoDir = assetDir + $"/logo_{card.GetGroupNameById()}.png";
            using(SKBitmap bitmap = new SKBitmap(1000, 3000))
            {
                using(SKCanvas canvas = new SKCanvas(bitmap))
                {
                    canvas.Clear(SKColors.White);
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
                    canvas.DrawBitmap(logoImage, new SKRect(150, 60, 410, 260 / logoImage.Width * logoImage.Height + 60));
                    //卡牌名与角色名
                    using (SKTypeface typeface = SKTypeface.FromFile("./asset/Fonts/old.ttf"))
                    {
                        using (SKPaint paint = new SKPaint())
                        {
                            paint.Typeface = typeface;
                            paint.IsAntialias = true;
                            paint.TextSize = 24;
                            paint.Color = SKColors.Black;
                            paint.TextAlign = SKTextAlign.Left;
                            string text = card.prefix;
                            canvas.DrawText(text, 470, 100, paint);
                            paint.TextSize = 26;
                            text = NickName.idToName[card.characterId];
                            canvas.DrawText(text, 470, 155, paint);
                        }
                    }
                }
                return ConvertBitmapToBase64(bitmap);
            }
        }
        /// <summary>
        /// 画卡牌头像
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isTrained"></param>
        /// <returns></returns>
        public async Task<SKBitmap> DrawCardIcon(SkCard card, bool isTrained)
        {
            string trainingStatus = isTrained ? "after_training" : "normal";
            string fileDirectory = "./asset/normal";
            SKBitmap bitmap = new SKBitmap(156, 180);
            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
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
                    canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/cardIconFrame_{starCount}.png"), new SKRect(0, 0, 156, 156));
                    for (int i = 0; i < starCount; i++)
                    {
                        canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/{trainingStatus}_star.png"), new SKRect(10 + i * 25, 121, 35 + i * 25, 146));
                    }
                }
                if (starCount == 0)
                {
                    canvas.DrawBitmap(await card.GetCardIcon(false), new SKRect(0, 0, 156, 156));
                    canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/cardIconFrame_bd.png"), new SKRect(0, 0, 156, 156));
                    canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/birthday_star.png"), new SKRect(10, 116, 40, 146));
                }
                //画属性
                canvas.DrawBitmap(SKBitmap.Decode($"{fileDirectory}/{card.attr}.png"), new SKRect(10, 10, 40, 40));
                //添加文字id
                using (SKPaint paint = new SKPaint())
                {
                    paint.TextSize = 20;
                    paint.Color = SKColors.DarkGray;
                    paint.TextAlign = SKTextAlign.Left;
                    string text = "ID:" + card.id.ToString();
                    canvas.DrawText(text, 5, 175, paint);
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
            SKRectI cropRect = new SKRectI(0, 54, input.Width, input.Height - 54 - 55);
            using(SKBitmap output = new SKBitmap(cropRect.Width, cropRect.Height))
            {
                using(SKCanvas canvas = new SKCanvas(output))
                {
                    canvas.DrawBitmap(input, cropRect, new SKRectI(0, 0, output.Width, output.Height));
                    return output;
                }
            }
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
    }
}
