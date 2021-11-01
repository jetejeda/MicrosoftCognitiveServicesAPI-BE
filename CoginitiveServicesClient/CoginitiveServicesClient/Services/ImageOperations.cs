using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoginitiveServicesClient.Services
{
    public static class ImageOperations
    {

        public static Bitmap Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                var image = (Bitmap)Bitmap.FromStream(ms, true);
                return image;
            }
        }

        public static string ImageToBase64(Image image)
        {            
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, ImageFormat.Png);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }            
        }

        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        public static Image ConvertToPng(Image imageToConvert)
        {
            Image bmpNewImage = new Bitmap(imageToConvert.Width,
                                      imageToConvert.Height);
            Graphics gfxNewImage = Graphics.FromImage(bmpNewImage);
            gfxNewImage.DrawImage(imageToConvert,
                                  new Rectangle(0, 0, bmpNewImage.Width,
                                                bmpNewImage.Height),
                                  0, 0,
                                  imageToConvert.
                                  Width, imageToConvert.Height,
                                  GraphicsUnit.Pixel);
            return bmpNewImage;
        }

        public static Image AssembleAnswerImage(Image imageOne, Image imageTwo, string kinshipResult)
        {
            Bitmap finalImage = new Bitmap(imageOne.Width + imageTwo.Width + 30, Math.Max(imageOne.Height, imageTwo.Height) + 280);
            Graphics answerGraphics = Graphics.FromImage(finalImage);
            answerGraphics.Clear(SystemColors.AppWorkspace);
            //write text
            var rectangle = new Rectangle(0, 0, imageOne.Width + imageTwo.Width + 30, Math.Max(imageOne.Height, imageTwo.Height) + 300);
            var font = new Font("Times New Roman", rectangle.Height / 30);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Near;

            answerGraphics.DrawString(kinshipResult, font, Brushes.Black, rectangle, stringFormat);

            answerGraphics.DrawImage(imageOne, new Point(10, 270));
            answerGraphics.DrawImage(imageTwo, new Point(imageOne.Width + 20, 270));
            return finalImage;
        }
    }
}
