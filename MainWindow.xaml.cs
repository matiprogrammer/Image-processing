using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Windows.Interop;
using LiveCharts;
using LiveCharts.Wpf;

namespace Biometria1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image currentImage;
        private bool isEdit;
        private System.Windows.Point lastMouseClicekPoint;
        private System.Windows.Point origin;
        private System.Windows.Point start;
        public MainWindow()
        {
            InitializeComponent();
            currentImage = new Image();
            isEdit = false;
            TransformGroup group = new TransformGroup();

            ScaleTransform xform = new ScaleTransform();
            group.Children.Add(xform);

            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);

            imgPhoto.RenderTransform = group;

            this.MouseWheel += image_MouseWheel;
            imgPhoto.MouseLeftButtonDown += image_MouseLeftButtonDown;
            imgPhoto.MouseLeftButtonUp += image_MouseLeftButtonUp;
            imgPhoto.MouseMove += image_MouseMove;
        }
    

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            string imagepath;
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png" +
                "|BMP Files (*.bmp)|*.bmp;" +
                "|SVG Files (*.svg)|*.svg;";
            if (op.ShowDialog() == true)
            {
                imagepath = op.FileName;
                //imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
                string name = System.IO.Path.GetFileName(imagepath);
                string destinationPath = GetDestinationPath(name, "Images");
                File.Copy(imagepath, destinationPath, true);
                currentImage.imagePath = destinationPath;

                BitmapSource imgSource = (new BitmapImage(new Uri(op.FileName)));
                var width = imgSource.PixelWidth;
                var height = imgSource.PixelHeight;
                var stride1 = width * ((imgSource.Format.BitsPerPixel + 7) / 8);
                var memoryBlockPointer = Marshal.AllocHGlobal(height * stride1);
                imgSource.CopyPixels(new Int32Rect(0, 0, width, height), memoryBlockPointer, height * stride1, stride1);
                
                currentImage.bitmap = new Bitmap(width, height, stride1, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, memoryBlockPointer);
               currentImage.orygBitmap = new Bitmap(currentImage.bitmap);
                imgPhoto.Source=ImageSourceForBitmap(currentImage.bitmap);
                
            }
            

        }

        public ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
           
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png|"+
                "Bitmap Image File (*.bmp)|*.bmp|"+
                "SVG (*.svg)|*.svg";
            if (saveFileDialog.ShowDialog() == true)
            {
                ImageConverter converter = new ImageConverter();
                File.WriteAllBytes(saveFileDialog.FileName, (byte[])converter.ConvertTo(currentImage.bitmap, typeof(byte[])));
              
            }

          

        }

        private static String GetDestinationPath(string filename, string foldername)
        {
            String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            appStartPath = String.Format(appStartPath + "\\{0}\\" + filename, foldername);
            return appStartPath;
        }

        private void MouseOverImage(object sender, MouseEventArgs e)
        {
            //System.Windows.Point mousePoint = new System.Windows.Point(e.GetPosition(imgPhoto).X,e.GetPosition(imgPhoto).Y);
            if (!isEdit)
            {
                int xCoordinate = (int)e.GetPosition(imgPhoto).X;
                int yCoordinate =(int) e.GetPosition(imgPhoto).Y;
                if (currentImage.getBounds().Contains((float)xCoordinate, (float)yCoordinate))
                {
                    System.Drawing.Color pixel = currentImage.bitmap.GetPixel(xCoordinate,yCoordinate );

                    RvalueBox.Text = pixel.R.ToString();
                    GvalueBox.Text = pixel.G.ToString();
                    BvalueBox.Text = pixel.B.ToString();
                    //line.Visibility = Visibility.Visible;
                    //line.X1 = e.GetPosition(imgPhoto).X;
                    //line.Y1 = e.GetPosition(imgPhoto).Y;
                    //line.X2 = 0;
                    //line.Y2 = 0;
                }
            }
        }
        
        private void mouseClickImage(object sender, MouseButtonEventArgs e)
        {
         
            isEdit = true;
            


            ApplyButton.IsEnabled = true;
            if (currentImage.getBounds().Contains((float)e.GetPosition(imgPhoto).X, (float)e.GetPosition(imgPhoto).Y))
            {
                lastMouseClicekPoint = new System.Windows.Point(e.GetPosition(imgPhoto).X, e.GetPosition(imgPhoto).Y);
                System.Drawing.Color pixel = currentImage.bitmap.GetPixel((int)Math.Round(e.GetPosition(imgPhoto).X), (int)Math.Round(e.GetPosition(imgPhoto).Y));
                RvalueBox.Text = pixel.R.ToString();
                GvalueBox.Text = pixel.G.ToString();
                BvalueBox.Text = pixel.B.ToString();
                Console.WriteLine((int)Math.Round(e.GetPosition(imgPhoto).X) + " " + (int)Math.Round(e.GetPosition(imgPhoto).Y));

            }
           

        }

        private void ChangePixelValue_Click(object sender, RoutedEventArgs e)
        {
            ApplyButton.IsEnabled = false;
            isEdit = false;

            int rValue, gValue, bValue;
            if(Int32.TryParse(RvalueBox.Text,out rValue) && Int32.TryParse(GvalueBox.Text, out gValue) && Int32.TryParse(BvalueBox.Text, out bValue))
            currentImage.bitmap.SetPixel((int)lastMouseClicekPoint.X, (int)lastMouseClicekPoint.Y, System.Drawing.Color.FromArgb(255,rValue,gValue,bValue));
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);

            System.Drawing.Color pixel = currentImage.bitmap.GetPixel((int)Math.Round(lastMouseClicekPoint.X), (int)Math.Round(lastMouseClicekPoint.Y)) ;

            Console.WriteLine(pixel.R.ToString() + " " + pixel.G.ToString() + " " + pixel.B.ToString());

        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgPhoto.ReleaseMouseCapture();
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!imgPhoto.IsMouseCaptured) return;

            var tt = (TranslateTransform)((TransformGroup)imgPhoto.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Vector v = start - e.GetPosition(border);
            tt.X = origin.X - v.X;
            tt.Y = origin.Y - v.Y;
           
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            imgPhoto.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)imgPhoto.RenderTransform).Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(border);
            origin = new System.Windows.Point(tt.X, tt.Y);
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)imgPhoto.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            double zoom = e.Delta > 0 ? .2 : -.2;
            transform.ScaleX += zoom;
            transform.ScaleY += zoom;
            Console.WriteLine(e.GetPosition(imgPhoto).X / currentImage.getBounds().Width + " " + e.GetPosition(imgPhoto).Y / currentImage.getBounds().Height);
            imgPhoto.RenderTransformOrigin = new System.Windows.Point(e.GetPosition(imgPhoto).X/ currentImage.getBounds().Width, e.GetPosition(imgPhoto).Y/ currentImage.getBounds().Height);
        }

        private void Histogram_Click(object sender, RoutedEventArgs e)
        {
            List<int[]> histograms=Histogram.calculateHistograms(currentImage.bitmap);
            Histograms window = new Histograms(histograms);
            window.Show();
        }

        private void DimImage(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)DimSlider.Value - 20;
            int r, g, b;
            System.Drawing.Color pixel;
            if (currentImage == null)
                return;
            if (currentImage.bitmap == null)
                return;
            int valueX=0;
            for (int i = 0; i < currentImage.bitmap.Width; i++)
                for (int j = 0; j < currentImage.bitmap.Height; j++) 
                {
                    if (sliderValue < 0)
                        valueX = (int)Math.Pow(sliderValue, 2) * (-1);
                    else
                        valueX = (int)Math.Pow(sliderValue, 2);

                        pixel = currentImage.orygBitmap.GetPixel(i, j);
                    r = (int)(pixel.R+ valueX);
                    g = (int)(pixel.G + valueX);
                    b = (int)(pixel.B + valueX);
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(255, ColorValueValid(r), ColorValueValid(g), ColorValueValid(b));
                    currentImage.bitmap.SetPixel(i, j, color);

                }
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

        public int ColorValueValid(int value)
        {
            if (value < 0)
                return 0;
            else if (value > 255)
                return 255;
            else return value;
        }

        private void btnStretch_Click(object sender, RoutedEventArgs e)
        {
            int[] LutR = StretchLUT(currentImage.bitmap)["R"];
            int[] LutG = StretchLUT(currentImage.bitmap)["G"];
            int[] LutB = StretchLUT(currentImage.bitmap)["B"];
            System.Drawing.Color pixel;
            System.Drawing.Color color;
            for (int i = 0; i < currentImage.bitmap.Width; i++)
                for (int j = 0; j < currentImage.bitmap.Height; j++)
                {
                    pixel = currentImage.bitmap.GetPixel(i, j);
                     color = System.Drawing.Color.FromArgb(255, LutR[pixel.R], LutG[pixel.G], LutB[pixel.B]);
                    currentImage.bitmap.SetPixel(i, j, color);
                }
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

        private Dictionary<String,int[]> StretchLUT(Bitmap bitmap)
        {
            int krMin, kgMin, kbMin, krMax, kgMax, kbMax;
            krMin = kgMin = kbMin = 255;
            krMax = kgMax = kbMax = 0;
            System.Drawing.Color pixel;
            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    pixel = currentImage.bitmap.GetPixel(i, j);
                    if (pixel.R > krMax)
                        krMax = pixel.R;

                    if (pixel.G > kgMax)
                        kgMax = pixel.G;

                    if (pixel.B > kbMax)
                        kbMax = pixel.B;

                    if (pixel.R < krMin)
                        krMin = pixel.R;

                    if (pixel.G < kgMin)
                        kgMin = pixel.G;

                    if (pixel.B < kbMin)
                        kbMin = pixel.B;
                }

                    int[] LutR = new int[256];
            int[] LutG = new int[256];
            int[] LutB = new int[256];
            for (int i = krMin; i < 256; i++)
            {
                LutR[i] = (int)(((double)(i - krMin) / (double)(krMax - krMin)) * 255);
            }
            for (int i = kgMin; i < 256; i++)
            {
                LutG[i] = (int)(((double)(i - kgMin) / (double)(kgMax - kgMin)) * 255);
            }
            for (int i = kbMin; i < 256; i++)
            {
                LutB[i] = (int)(((double)(i - kbMin) / (double)(kbMax - kbMin)) * 255);
            }
            Dictionary<String, int[]> LutDictionary = new Dictionary<string, int[]>();
            LutDictionary.Add("R", LutR);
            LutDictionary.Add("G", LutG);
            LutDictionary.Add("B", LutB);
            return LutDictionary;

        }

        private void btnAlign_Click(object sender, RoutedEventArgs e)
        {
           List<int[]> histograms= Histogram.calculateHistograms(currentImage.bitmap);
            double[] cumulativeDistributionRed = CalculateCumulativeDistribution(histograms[0]);
            double[] cumulativeDistributionBlue = CalculateCumulativeDistribution(histograms[2]);
            double[] cumulativeDistributionGreen = CalculateCumulativeDistribution(histograms[1]);
            int[] LutR = GetAlignLut(cumulativeDistributionRed);
            int[] LutG = GetAlignLut(cumulativeDistributionGreen);
            int[] LutB = GetAlignLut(cumulativeDistributionBlue);
            System.Drawing.Color pixel;
            System.Drawing.Color color;
           
            for (int i = 0; i < currentImage.bitmap.Width; i++)
                for (int j = 0; j < currentImage.bitmap.Height; j++)
                {
                    pixel = currentImage.bitmap.GetPixel(i, j);
                    color = System.Drawing.Color.FromArgb(255, LutR[pixel.R], LutG[pixel.G], LutB[pixel.B]);
                    currentImage.bitmap.SetPixel(i, j, color);
                }
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

        public int[] GetAlignLut(double[] cumulativeDistribuation)
        {
            int[] lut = new int[256];
            double minValue = 0;
            foreach (var value in cumulativeDistribuation)
                if (value > minValue)
                {
                    minValue = value;
                    break;
                }

            for (int i=0;i<256;i++)
            {
                lut[i] = (int)((double)((double)(cumulativeDistribuation[i] - minValue) / (double)(1 - minValue)) * (255));
            }
            return lut;
        }

        public double[] CalculateCumulativeDistribution(int[] histogram)
        {
            long countAllPixels=0;
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

        private void ManualyBinarize_Click(object sender, RoutedEventArgs e)
        {
            int manualValue = Int32.Parse(manualBinarizationValueBox.Text);

            PutToGreyScaleComboBox(ColorComoBox.SelectedIndex);
            Binarization.manualBinarization(currentImage.bitmap,manualValue);

            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

        private void OtsuBinarize_Click(object sender, RoutedEventArgs e)
        {
            PutToGreyScaleComboBox(ColorComoBox.SelectedIndex);
            int[] histogram = Histogram.calculateHistograms(currentImage.bitmap)[ColorComoBox.SelectedIndex];
                Binarization.otsuBinarization(currentImage.bitmap, histogram);
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);

        }

        public void PutToGreyScaleComboBox(int index)
        {
            currentImage.bitmap = new Bitmap(currentImage.orygBitmap);
            switch (index)
            {
                case 0: currentImage.PutToGreyScaleBaseRed(); break;
                case 1: currentImage.PutToGreyScaleBaseGreen(); break;
                case 2: currentImage.PutToGreyScaleBaseBlue(); break;
                case 3: currentImage.PutToGreyScaleBaseAverage(); break;
            }
        }

        private void BernsenBinarize_Click(object sender, RoutedEventArgs e)
        {
            PutToGreyScaleComboBox(ColorComoBox.SelectedIndex);
            Binarization.BernsenBinarization(currentImage.bitmap, Int32.Parse(ContrastValueBox.Text), Int32.Parse(ThresholdValueBox.Text));
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

        private void Prewitta_Click(object sender, RoutedEventArgs e)
        {
            Filtr.ConvolutionalFilter(currentImage.bitmap, currentImage.orygBitmap, getMaskPrewitt());
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

        private void Sobela_Click(object sender, RoutedEventArgs e)
        {
            Filtr.ConvolutionalFilter(currentImage.bitmap, currentImage.orygBitmap, getMaskSobel());
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

        private void Laplace_Click(object sender, RoutedEventArgs e)
        {
            Filtr.ConvolutionalFilter(currentImage.bitmap, currentImage.orygBitmap, getMaskLaplace());
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

        private void Corner_Click(object sender, RoutedEventArgs e)
        {
            Filtr.ConvolutionalFilter(currentImage.bitmap, currentImage.orygBitmap, getMaskCorner());
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }
        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            Filtr.ConvolutionalFilter(currentImage.bitmap, currentImage.orygBitmap, getMaskBlur());
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

        #region maski
        public int[,] getMaskPrewitt()
        {
            int[,] mask;
            switch(MaskPrewittComoBox.SelectedIndex)
            {
                case 0:
                    mask = new int[,] { { -1, 0, 1 },
                                            { -1, 0, 1 },
                                            { -1, 0, 1 }}; setTextBoxesForMask(mask); return mask; break;
                case 1:
                    mask = new int[,] { { 0,1,1},
                                            {-1,0,1 },
                                            {-1,-1,0} }; setTextBoxesForMask(mask); return mask; break;
                case 2:
                    mask = new int[,] { { 1,1,1},
                                        {0,0,0 },
                                        {-1,-1,-1} }; setTextBoxesForMask(mask); return mask; break;
                case 3:
                    mask = new int[,] { { 1,1,0},
                                        {1,0,-1},
                                        {0,-1,-1} }; setTextBoxesForMask(mask); return mask; break;
                case 4:
                    mask = new int[,] { { int.Parse(CornerLT.Text), int.Parse(Top.Text), int.Parse(CornerRT.Text) },
                                { int.Parse(Left.Text), int.Parse(Center.Text), int.Parse(Right.Text) },
                                { int.Parse(CornerLB.Text), int.Parse(Bottom.Text), int.Parse(CornerRB.Text) } }; setTextBoxesForMask(mask); return mask; break;
                    
            }
            return null;

        }

        public int[,] getMaskSobel()
        {
            int[,] mask;
            switch (MaskSobelComoBox.SelectedIndex)
            {
                case 0:
                    mask = new int[,] { { -1, 0, 1 },
                                            { -2, 0, 2 },
                                            { -1, 0, 1 }}; setTextBoxesForMask(mask); return mask; break;
                case 1:
                    mask = new int[,] { { 0,1,2},
                                            {-1,0,1 },
                                            {-2,-1,0} }; setTextBoxesForMask(mask); return mask; break;
                case 2:
                    mask = new int[,] { { 1,2,1},
                                        {0,0,0 },
                                        {-1,-2,-1} }; setTextBoxesForMask(mask); return mask; break;
                case 3:
                    mask = new int[,] { { 2,1,0},
                                        {1,0,-1},
                                        {0,-1,-2} }; setTextBoxesForMask(mask); return mask; break;
                case 4:
                    mask = new int[,] { { int.Parse(CornerLT.Text), int.Parse(Top.Text), int.Parse(CornerRT.Text) },
                                { int.Parse(Left.Text), int.Parse(Center.Text), int.Parse(Right.Text) },
                                { int.Parse(CornerLB.Text), int.Parse(Bottom.Text), int.Parse(CornerRB.Text) } }; setTextBoxesForMask(mask); return mask; break;

            }
            return null;
        }
        public int[,] getMaskLaplace()
        {
            int[,] mask;
            switch (MaskLaplaceComoBox.SelectedIndex)
            {
                case 0:
                    mask = new int[,] { { 0,-1,0 },
                                            { -1,4,-1},
                                            {0,-1,0 }}; setTextBoxesForMask(mask); return mask; break;
                case 1:
                    mask = new int[,] { { -1,-1,-1},
                                            {-1,8,-1 },
                                            {-1,-1,-1} }; setTextBoxesForMask(mask); return mask; break;
                case 2:
                    mask = new int[,] { { 1,-2,1},
                                        {-2,4,-2},
                                        {1,-2,1} }; setTextBoxesForMask(mask); return mask; break;
                case 3:
                    mask = new int[,] { { int.Parse(CornerLT.Text), int.Parse(Top.Text), int.Parse(CornerRT.Text) },
                                { int.Parse(Left.Text), int.Parse(Center.Text), int.Parse(Right.Text) },
                                { int.Parse(CornerLB.Text), int.Parse(Bottom.Text), int.Parse(CornerRB.Text) } }; setTextBoxesForMask(mask); return mask; break;

            }
            return null;

        }

        public int[,] getMaskCorner()
        {
            int[,] mask;
            switch (MaskCornerComoBox.SelectedIndex)
            {
                case 0:
                    mask = new int[,] { { 1,1,1 },
                                            { 1, -2, -1 },
                                            { 1,-1,-1 }}; setTextBoxesForMask(mask); return mask; break;
                case 1:
                    mask = new int[,] { { 1,1,1},
                                            {-1,-2,1 },
                                            {-1,-1,1} }; setTextBoxesForMask(mask); return mask; break;
                case 2:
                    mask = new int[,] { { 1,-1,-1},
                                        {1,-2,-1},
                                        {1,1,1} }; setTextBoxesForMask(mask); return mask; break;
                case 3:
                    mask = new int[,] { { -1,-1,1},
                                        {-1,-2,1},
                                        {1,1,1} }; setTextBoxesForMask(mask); return mask; break;
                case 4:
                    mask = new int[,] { { int.Parse(CornerLT.Text), int.Parse(Top.Text), int.Parse(CornerRT.Text) },
                                { int.Parse(Left.Text), int.Parse(Center.Text), int.Parse(Right.Text) },
                                { int.Parse(CornerLB.Text), int.Parse(Bottom.Text), int.Parse(CornerRB.Text) } }; setTextBoxesForMask(mask); return mask; break;

            }
            return null;

        }
        public int[,] getMaskBlur()
        {
            int[,] mask = new int[,] { { 1,1,1},
                                        {1,1,1},
                                        {1,1,1} }; setTextBoxesForMask(mask);
            return mask;

            

        }
        public void setTextBoxesForMask(int[,] mask)
        {
            CornerLT.Text = mask[0, 0].ToString();
            Top.Text = mask[0, 1].ToString();
            CornerRT.Text = mask[0, 2].ToString();
            Left.Text = mask[1, 0].ToString();
            Center.Text = mask[1, 1].ToString();
            Right.Text = mask[1, 2].ToString();
            CornerLB.Text = mask[2, 0].ToString();
            Bottom.Text = mask[2, 1].ToString();
            CornerRB.Text = mask[2, 2].ToString();
        }

        #endregion

        private void Kuwahar_Click(object sender, RoutedEventArgs e)
        {
            Filtr.KuwahraFilter(currentImage.bitmap, currentImage.orygBitmap);
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

        private void procentageSelection_Click(object sender, RoutedEventArgs e)
        {
            int[] histogram = Histogram.calculateHistograms(currentImage.orygBitmap)[ColorComoBox.SelectedIndex];
            int percentage =Int32.Parse( PercentageTextBox.Text);
            int[] LUT = new int[256];
            long all = 0;


            all = currentImage.bitmap.Width * currentImage.bitmap.Height;
           
                double limes = all*percentage *0.01;
            int nextSum = 0;
            for (int i = 0; i < 256; ++i)
            {
                nextSum = nextSum + histogram[i];
                if (nextSum < limes)
                {
                    LUT[i] = 0;
                }
                else
                {
                    LUT[i] = 255;
                }
            }

            System.Drawing.Color pixel,color;
            for (int i = 0; i < currentImage.bitmap.Width; i++)
                for (int j = 0; j < currentImage.bitmap.Height; j++)
                {
                    pixel = currentImage.orygBitmap.GetPixel(i, j);
                    color = System.Drawing.Color.FromArgb(255, LUT[pixel.G], LUT[pixel.G], LUT[pixel.G]);
                    currentImage.bitmap.SetPixel(i, j, color);
                }
            imgPhoto.Source = ImageSourceForBitmap(currentImage.bitmap);
        }

    }
}
