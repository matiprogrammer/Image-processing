using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Shapes;


namespace Biometria1
{
    /// <summary>
    /// Interaction logic for Histograms.xaml
    /// </summary>
    public partial class Histograms : Window
    {
        public SeriesCollection RedCollection { get; set; }
        public SeriesCollection BlueCollection { get; set; }
        public SeriesCollection GreenCollection { get; set; }
        public SeriesCollection AverageCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public Histograms(List<int[]> histograms)
        {
            InitializeComponent();

            RedCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = histograms[0].AsChartValues(),
                    ColumnPadding=0,
                   Title="Red",
                   Fill=Brushes.Red,

                }
            };
            
            BlueCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = histograms[2].AsChartValues(),
                    ColumnPadding=0,
                    Title="Blue",
                    Fill=Brushes.Blue,
                }
            };
            GreenCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = histograms[1].AsChartValues(),
                    ColumnPadding=0,
                    Title="Green",
                    Fill=Brushes.Green,
                }
            };
            AverageCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = histograms[3].AsChartValues(),
                    ColumnPadding=0,
                    Title="Average",
                    Fill=Brushes.Black,
                }
            };

            Formatter = value => value.ToString("N");

            DataContext = this;
        }

       
    }
    

}

