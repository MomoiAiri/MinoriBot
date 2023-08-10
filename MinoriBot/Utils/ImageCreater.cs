using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils
{
    public class ImageCreater
    {
        public async Task DrawCardIconImage(List<SkCard> cards,bool isTrained)
        {
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
            foreach (var kvp in classifiedCards)
            {
                Console.WriteLine($"ID: {kvp.Key}");
                foreach (var attrKvp in kvp.Value)
                {
                    Console.WriteLine($"  Attr: {attrKvp.Key}");
                    foreach (var card in attrKvp.Value)
                    {
                        Console.WriteLine($"    Name: {card.prefix}");
                    }
                }
            }
            List<SKBitmap> iconLines = new List<SKBitmap>();

        }
        public async Task<SKBitmap> DrawCardIconLine (List<SkCard> cards ,bool isTrained)
        {

            using(SKBitmap bitmap = new SKBitmap(100, 100))
            {

                return bitmap;
            }
        }
        public async Task<SKBitmap> DrawCardIcon(SkCard card, bool isTrained)
        {
            string trainingStatus = isTrained ? "after_training" : "normal";
            string fileDirectory = "./asset/normal";
            using (SKBitmap bitmap = new SKBitmap(156, 180))
            {
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
                    return bitmap;

                    //using (var image = SKImage.FromBitmap(bitmap))
                    //using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    //using (var stream = File.OpenWrite("./asset/temp/output.png"))
                    //{
                    //    data.SaveTo(stream);
                    //}
                }
            }
        }
    }
}
