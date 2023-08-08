using MinoriBot.Enums.Sekai;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils
{
    internal class ImageCreater
    {
        public async void DrawCardIcon(SkCard card, bool isTrained)
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
                    using (var image = SKImage.FromBitmap(bitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var stream = File.OpenWrite("./asset/temp/output.png"))
                    {
                        data.SaveTo(stream);
                    }
                }
            }
        }
    }
}
