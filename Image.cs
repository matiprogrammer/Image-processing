using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria1
{
    class Image
    {
        public Image() { }
        public Image(string imagePath)
        {
            this.imagePath = imagePath;
        }

        public string imagePath { get; set; }
        public Bitmap bitmap { get; set; }
        public Bitmap orygBitmap { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public RectangleF getBounds()
        {
            GraphicsUnit units = GraphicsUnit.Pixel;
            return bitmap.GetBounds(ref units);
        }

        public void PutToGreyScaleBaseRed()
        {
            System.Drawing.Color color;

            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    bitmap.SetPixel(i, j, Color.FromArgb(255, color.R, color.R, color.R));

                }
        }
        public void PutToGreyScaleBaseGreen()
        {
            System.Drawing.Color color;

            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    bitmap.SetPixel(i, j, Color.FromArgb(255, color.G, color.G, color.G));

                }
        }
        public void PutToGreyScaleBaseBlue()
        {
            System.Drawing.Color color;

            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    bitmap.SetPixel(i, j, Color.FromArgb(255, color.B, color.B, color.B));

                }
        }
        public void PutToGreyScaleBaseAverage()
        {
            System.Drawing.Color color;
            int value;
            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    value = (color.R + color.G + color.B) / 3;
                    bitmap.SetPixel(i, j, Color.FromArgb(255, value, value, value));

                }
        }
    }
}
