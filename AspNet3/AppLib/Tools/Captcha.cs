using AspNet3.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Tools
{
    public static class Captcha
    {
        // INFO Capctha letters:
        private const string Letters = "0123456789";

        private static string GenerateCaptchaCode(int characterCount = 4)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < characterCount; i++)
            {
                int index = RandomGenerator.Next(0, Letters.Length - 1);
                sb.Append(Letters[index]);
            }

            return sb.ToString();
        }

        public static bool ValidateCaptchaCode(string userInputCaptcha, HttpContext context)
        {
            var isValid = (userInputCaptcha == context.Session.GetString("CaptchaCode"));
            context.Session.Remove("CaptchaCode");
            return isValid;
        }

        public static CaptchaResult GenerateCaptchaImage(HttpContext context)
        {
            CaptchaResult cr = GenerateCaptchaImage(captchaCode: GenerateCaptchaCode());
            context.Session.SetString("CaptchaCode", cr.CaptchaCode);
            return cr;
        }

        public static CaptchaResult GenerateCaptchaImage2(HttpContext context)
        {
            CaptchaResult cr = GenerateCaptchaImage2(captchaCode: GenerateCaptchaCode());
            context.Session.SetString("CaptchaCode", cr.CaptchaCode);
            return cr;
        }

        private static CaptchaResult GenerateCaptchaImage(string captchaCode, int width=100, int height=36)
        {
            using (Bitmap baseMap = new Bitmap(width, height))
            using (Graphics graph = Graphics.FromImage(baseMap))
            {
                Random rand = new Random();

                graph.Clear(GetRandomLightColor());

                DrawCaptchaCode();
                DrawDisorderLine();

                //graph.FillRectangle(new HatchBrush(HatchStyle.BackwardDiagonal, Color.Silver, Color.Transparent), graph.ClipBounds);
                //graph.FillRectangle(new HatchBrush(HatchStyle.ForwardDiagonal, Color.Silver, Color.Transparent), graph.ClipBounds);

                graph.FillRectangle(new HatchBrush(HatchStyle.ZigZag, Color.Silver, Color.Transparent), graph.ClipBounds);

                MemoryStream ms = new MemoryStream();

                baseMap.Save(ms, ImageFormat.Png);

                return new CaptchaResult { CaptchaCode = captchaCode, CaptchaByteData = ms.ToArray(), Timestamp = DateTime.Now };

                int GetFontSize(int imageWidth, int captchCodeCount)

                {

                    var averageSize = imageWidth / captchCodeCount;



                    return Convert.ToInt32(averageSize);

                }

                Color GetRandomDeepColor()

                {

                    // text color

                    int redlow = 160, greenLow = 100, blueLow = 160;

                    return Color.FromArgb(rand.Next(redlow), rand.Next(greenLow), rand.Next(blueLow));

                }

                Color GetRandomLightColor()
                {
                    // bg color
                    return Color.White;

                    //int low = 180, high = 255;
                    //int nRend = rand.Next(high) % (high - low) + low;
                    //int nGreen = rand.Next(high) % (high - low) + low;
                    //int nBlue = rand.Next(high) % (high - low) + low;
                    //return Color.FromArgb(nRend, nGreen, nBlue);

                }

                void DrawCaptchaCode()

                {

                    SolidBrush fontBrush = new SolidBrush(Color.Black);



                    int fontSize = GetFontSize(width, captchaCode.Length);



                    Font font = new Font(FontFamily.GenericSerif, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);



                    for (int i = 0; i < captchaCode.Length; i++)

                    {

                        fontBrush.Color = GetRandomDeepColor();



                        int shiftPx = fontSize / 6;



                        float x = i * fontSize + rand.Next(-shiftPx, shiftPx) + rand.Next(-shiftPx, shiftPx);



                        int maxY = height - fontSize;



                        if (maxY < 0) maxY = 0;



                        float y = rand.Next(0, maxY);



                        graph.DrawString(captchaCode[i].ToString(), font, fontBrush, x, y);

                    }

                }

                void DrawDisorderLine()

                {

                    Pen linePen = new Pen(new SolidBrush(Color.Black), 1);



                    for (int i = 0; i < rand.Next(3, 5); i++)

                    {

                        linePen.Color = GetRandomDeepColor();



                        Point startPoint = new Point(rand.Next(0, width), rand.Next(0, height));

                        Point endPoint = new Point(rand.Next(0, width), rand.Next(0, height));

                        graph.DrawLine(linePen, startPoint, endPoint);



                        //Point bezierPoint1 = new Point(rand.Next(0, width), rand.Next(0, height));

                        //Point bezierPoint2 = new Point(rand.Next(0, width), rand.Next(0, height));

                        //graph.DrawBezier(linePen, startPoint, bezierPoint1, bezierPoint2, endPoint);

                    }

                }
                
            }
        }

        private static CaptchaResult GenerateCaptchaImage2(string captchaCode, int width=100, int height=36)
        {
            using (Bitmap bmp = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Navy);
                g.DrawString(captchaCode, new Font("Courier", 16), new SolidBrush(Color.WhiteSmoke), 18, 8);
                g.FillRectangle(new HatchBrush(HatchStyle.BackwardDiagonal, Color.LightGray, Color.Transparent), g.ClipBounds);
                g.FillRectangle(new HatchBrush(HatchStyle.ForwardDiagonal, Color.LightGray, Color.Transparent), g.ClipBounds);

                MemoryStream ms = new MemoryStream();

                bmp.Save(ms, ImageFormat.Png);

                return new CaptchaResult { CaptchaCode = captchaCode, CaptchaByteData = ms.ToArray(), Timestamp = DateTime.Now };
            }
        }
    }
}
