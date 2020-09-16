using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria1
{
    class Binarization
    {
        public static Bitmap manualBinarization(Bitmap img, int threshold)
        {
            for (int w = 0; w < img.Width; w++)
            {
                for (int h = 0; h < img.Height; h++)
                {
                    Color c = img.GetPixel(w, h);
                    img.SetPixel(w, h, c.R >= threshold ?  Color.White: Color.Black);
                }
            }
            return img;
        }

        public static void otsuBinarization(Bitmap bitmap,int[] histogram)
        {

            int maxVariance = 0, tmpMaxVariance, threshold = 0;
            double[] cumulativeDistribution = CumulativeDistribution(histogram);
            double foregroundPixels, backgroundPixels;
            int averageBackground, averageForeground;
            for (int i = 0; i < 256; i++)
            {
                foregroundPixels = GetCountPixels(cumulativeDistribution, i + 1, histogram.Length);
                backgroundPixels = GetCountPixels(cumulativeDistribution, 0, i);
                averageBackground = getAveragePixels(cumulativeDistribution, 0, i, backgroundPixels);
                averageForeground = getAveragePixels(cumulativeDistribution, i+1, histogram.Length, foregroundPixels);
               tmpMaxVariance = (int)(foregroundPixels * backgroundPixels * Math.Pow(averageForeground - averageBackground, 2));
                if (maxVariance < tmpMaxVariance)
                {
                    maxVariance = tmpMaxVariance;
                    threshold = i;
                }
            }
            Binarization.manualBinarization(bitmap, threshold);


        }

        public static int getAveragePixels(double[] cumulativeDistribution, int start, int end, double allPixels)
        {
            double average = 0;
            for (int i = start; i < end; i++)
            {
                average += (cumulativeDistribution[i] * (double)((double)i / allPixels));
            }
            return (int)average;
        }

        
        public static double[] CumulativeDistribution(int[] histogram)
        {
            long countAllPixels = 0;
            double[] cumulativeDistribution = new double[256];
            for (int i = 0; i < histogram.Length; i++)
                countAllPixels += histogram[i];

            for (int i = 0; i < histogram.Length; i++)
            {
                if (i > 0)
                    cumulativeDistribution[i] = ((double)((double)histogram[i] / (double)countAllPixels)) + cumulativeDistribution[i - 1];
                else
                    cumulativeDistribution[i] = ((double)((double)histogram[i] / (double)countAllPixels));
            }
            return cumulativeDistribution;
        }

        public static double GetCountPixels(double[] cumulativeDistribution, int start, int end)
        {
            double sum = 0;
            for (int i = start; i < end; i++)
            {
                sum += cumulativeDistribution[i];
            }
            return sum;
        }


        public static void BernsenBinarization(Bitmap bitmap, int contrastThreshold, int setThreshold)
        {
            
            int midGray;
            int[] minMaxArray;
            for(int i=3;i<bitmap.Width-3;i++)
            {
                for(int j=3;j<bitmap.Height-3;j++)
                {
                    minMaxArray = getMaxMinGrey(i, j, bitmap);
                    midGray = (minMaxArray[0] + minMaxArray[1]) / 2;
                    if ((minMaxArray[1] - minMaxArray[0]) / 2 < contrastThreshold)
                    {
                        bitmap.SetPixel(i, j, (midGray >= setThreshold) ? Color.White : Color.Black);
                    }
                    else
                        bitmap.SetPixel(i,j,(bitmap.GetPixel(i, j).R >= midGray) ? Color.White : Color.Black);

                }
            }
        }

        public static int[] getMaxMinGrey(int x,int y, Bitmap greyBitmap)
        {
            int max = 0, tmp, min=255;
            for(int i=x-3;i<x+4; i++)
                for(int j=y-3;j<y+4;j++)
                {
                    tmp=greyBitmap.GetPixel(i - (x - 3), j - (y - 3)).G;
                    if (max < tmp)
                        max = tmp;
                    if (min > tmp)
                        min = tmp;
                }
            return new int[] { min,max};
        }

    }
}
