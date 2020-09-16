using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Biometria1
{
    class Histogram
    {
        public static List<int[]> calculateHistograms(Bitmap image)
        {
            int[] histogramRed = new int[256];
            int[] histogramGreen = new int[256];
            int[] histogramBlue = new int[256];
            int[] histogramAverage = new int[256];
            for (int w = 0; w < image.Width; w++)
            {
                for (int h = 0; h < image.Height; h++)
                {
                    Color color = image.GetPixel(w,h);
                    histogramRed[color.R]++;
                    histogramGreen[color.G]++;
                    histogramBlue[color.B]++;
                    histogramAverage[(color.R + color.G + color.B) / 3]++;
                }
            }

            List<int[]> histograms = new List<int[]>();
            histograms.Add(histogramRed);
            histograms.Add(histogramGreen);
            histograms.Add(histogramBlue);
            histograms.Add(histogramAverage);

            return histograms;
        }
    }
}
