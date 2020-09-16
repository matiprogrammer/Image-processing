using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria1
{
     class Filtr
    {
        public static void ConvolutionalFilter(Bitmap bitmap, Bitmap bitmapCopy, int[,] mask)
        {
            int wMax = bitmapCopy.Width, hMax = bitmapCopy.Height;
            for (int i=0;i<bitmapCopy.Width;i++)
                for(int j=0;j<bitmapCopy.Height;j++)
                {
                    bitmap.SetPixel(i, j, calculateNewPixelValue(bitmapCopy, i, j,wMax,hMax, mask));
                }
        }
        public static Color calculateNewPixelValue(Bitmap img, int w, int h,int wMax, int hMax, int[,] mask)
        {
            double sumRed = 0, sumGreen = 0, sumBlue = 0;
            Color c;
            int x, y, sumValueMask=GetSumValueMask(mask);
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (w + i < 0) x = 0;
                    else if (w + i >= wMax) x = (w+i)-(wMax- ((w + i)-1));
                    else x = w + i;
                    if (h + j < 0) y = 0;
                    else if (h + j >= hMax) y = (h + j) - (hMax - ((h + j) - 1));
                    else y = h + j;

                    c = img.GetPixel(x, y);
                    sumRed += c.R*mask[j+1,i+1];
                    sumBlue += c.B * mask[j + 1, i + 1];
                    sumGreen += c.G * mask[j + 1, i + 1];
                }

            }
            sumRed = sumRed / sumValueMask;
            sumBlue = sumBlue / sumValueMask;
            sumGreen = sumGreen / sumValueMask;

            if (sumRed > 255) sumRed = 255;
            else if (sumRed < 0) sumRed = 0;
            if (sumGreen > 255) sumGreen = 255;
            else if (sumGreen < 0) sumGreen = 0;
            if (sumBlue > 255) sumBlue = 255;
            else if (sumBlue < 0) sumBlue = 0;

            c =Color.FromArgb((int)sumRed, (int)sumGreen, (int)sumBlue);
            return c;
        }
        public static int GetSumValueMask(int[,] mask)
        {
            int sum = 0;
            for(int i=0;i<3;i++)
            {
                for(int j=0;j<3;j++)
                {
                    sum += mask[i, j];
                }
            }
            return (sum==0)? 1: sum;
        }

        public static void KuwahraFilter(Bitmap bitmap, Bitmap bitmapCopy)
        {
            int[] avgRed=new int[4], avgGreen = new int[4], avgBlue = new int[4], varianceRed = new int[4], varianceGreen = new int[4], varianceBlue = new int[4];
            List<Color[,]> regions;
            for (int i = 2; i < bitmapCopy.Width - 2; i++)
            {
                for (int j = 2; j < bitmapCopy.Height - 2; j++)
                {
                    avgRed = new int[4]; avgGreen = new int[4]; avgBlue = new int[4]; varianceRed = new int[4]; varianceGreen = new int[4]; varianceBlue = new int[4];
                    regions = GetRegionsFromBitmap(i, j, 5, bitmapCopy);
                    foreach (var color in regions[0])
                    {
                        avgRed[0] += color.R;
                        avgGreen[0] += color.G;
                        avgBlue[0] += color.B;
                    }

                    foreach (var color in regions[1])
                    {
                        avgRed[1] += color.R;
                        avgGreen[1] += color.G;
                        avgBlue[1] += color.B;
                    }
                    foreach (var color in regions[2])
                    {
                        avgRed[2] += color.R;
                        avgGreen[2] += color.G;
                        avgBlue[2] += color.B;
                    }
                    foreach (var color in regions[3])
                    {
                        avgRed[3] += color.R;
                        avgGreen[3] += color.G;
                        avgBlue[3] += color.B;
                    }
                    avgRed = GetAvg(avgRed);
                    avgGreen = GetAvg(avgGreen);
                    avgBlue = GetAvg(avgBlue);
                    for(int x=0;x<4;x++)
                    {
                        varianceRed[x] = VarianceRed(avgRed[x], regions[x]);
                        varianceGreen[x] = VarianceGreen(avgGreen[x], regions[x]);
                        varianceBlue[x] = VarianceBlue(avgBlue[x], regions[x]);

                    }
                    bitmap.SetPixel(i, j, Color.FromArgb(255, avgRed[getMinimum(varianceRed)], avgGreen[getMinimum(varianceGreen)], avgBlue[getMinimum(varianceBlue)]));

                }
            }
        }
        
        private static int getMinimum(int[] variance)
        {
            int min = variance[0];
            int minIndex=0;
            for (int i = 0; i < 4; i++)
                if (variance[i] < min)
                {
                    min = variance[i];
                    minIndex = i;
                }
            return minIndex;

        }

        public static int VarianceRed(int avg, Color[,] region)
        {
            int sum = 0;
            foreach (var value in region)
                sum +=(int)Math.Pow(avg - value.R, 2);
            sum /= 9;
            return sum;
        }
        public static int VarianceGreen(int avg, Color[,] region)
        {
            int sum = 0;
            foreach (var value in region)
                sum += (int)Math.Pow(avg - value.G, 2);
            sum /= 9;
            return sum;
        }
        public static int VarianceBlue(int avg, Color[,] region)
        {
            int sum = 0;
            foreach (var value in region)
                sum += (int)Math.Pow(avg - value.B, 2);
            sum /= 9;
            return sum;
        }

        public static int[] GetAvg(int[] tab)
        {
            for (int i = 0; i < 4; i++)
                tab[i] /= 9;
            return tab;
                
        }

        public static List<Color[,]> GetRegionsFromBitmap(int x, int y,int maskSize, Bitmap bitmap)
        {
            List<Color[,]> regions = new List<Color[,]>();
            //pierwszy region
            Color[,] region1 = new Color[3,3];
            for(int i = x - (x - (maskSize / 2)); i >=0 ; i--)
            {
                for(int j= y - (y - (maskSize / 2)); j>=0 ; j--)
                {
                    region1[j, i] = bitmap.GetPixel(x - i, y - j);
                }
            }
            regions.Add(region1);

            //drugi region
            Color[,] region2 = new Color[3, 3];
            for (int i = 0; i <=  (x + (maskSize / 2))-x; i++)
            {
                for (int j = y - (y - (maskSize / 2)); j >=  0; j--)
                {
                    region2[j, i] = bitmap.GetPixel(x + i, y - j);
                }
            }
            regions.Add(region2);

            //trzeci region
            Color[,]  region3 = new Color[3, 3];
            for (int i = x - (x - (maskSize / 2)); i >= 0; i--)
            {
                for (int j = 0; j <= (y + (maskSize / 2)) - y; j++)
                {
                    region3[j, i] = bitmap.GetPixel(x - i, y + j);
                }
            }
            regions.Add(region3);

            //czwarty region
            Color[,] region4 = new Color[3, 3];
            for (int i = 0; i <= (x + (maskSize / 2)) - x; i++)
            {
                for (int j = 0; j <= (y + (maskSize / 2)) - y; j++)
                {
                    region4[j, i] = bitmap.GetPixel(x + i, y + j);
                }
            }
            regions.Add(region4);
            return regions;
        }

       public static void MedianeFilter(Bitmap bitmap, Bitmap bitmapCopy, int maskSize)
        {
            int[] rColor, gColor, bColor;
            for(int i=0;i<bitmapCopy.Width;i++)
            {
                for(int j=0; j<bitmapCopy.Height;j++)
                {
                   
                }
            }
        }

        public static Color[] Sort(Color[] colors)
        { int[] red=new int[colors.Length], green = new int[colors.Length], blue = new int[colors.Length];
            for(int i=0;i<colors.Length;i++)
            {
                red[i] = colors[i].R;
                green[i] = colors[i].G;
                blue[i] = colors[i].B;
            }
            Array.Sort(red, 0, red.Length);
            Array.Sort(green, 0, green.Length);
            Array.Sort(blue, 0, blue.Length);
            Color[] sortColors = new Color[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                sortColors[i] = Color.FromArgb(255,red[i],green[i], blue[i]);
            }
            return sortColors;

        }
        private static Color[] GetArrayFromBitmap(int x, int y, int maskSize, Bitmap bitmap)
        {
            Color[] colors = new Color[(int)Math.Pow(maskSize, 2)];
            for(int i=0;i<maskSize;i++)
                for(int j=0;j<maskSize;j++)
                {
                    colors[i] = bitmap.GetPixel(x - (maskSize/2) + i, y - (maskSize/2) + j);
                }
            return colors;
        }
    }
}
