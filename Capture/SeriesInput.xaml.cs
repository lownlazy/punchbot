using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using Capture;
using PunchBot.Core;

namespace Capture
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SeriesInput : UserControl
    {
        MainWindow Main;
        LineSeries LineSeries;

        public SeriesInput(MainWindow main)
        {
            Main = main;
            InitializeComponent();

            string dataText = Calculator.GetData(@"C:\Users\Russell\Source\Repos\punchbot\Assets\hit1.txt");
            data = dataText;
        }

        public string data
        {
            set {
                var yData = GetYData(value);
                var xData = GetXData(yData);
                var xyData = GetXYData(value);

                //check for irrelivant data, usually caused by the head being bumped or is return bounce
                //if (data.ElementAt(5).Value > 40000)
                //{
                    //MessageBox.Show("data error: " + value.Substring(0, 100));
                    //return;
                //}
                
                DrawLine(xyData);

                Calculator core = new Calculator();

                score.Content = core.GetTorque(yData).ToString();

                KeyValuePair<int, Double>[] trend = core.GetTrendLine(xData,yData); 
                DrawLine(trend);


                //string[] x = core.diffList.Select(n => n.ToString()).ToArray();
                UserData.Text = value; //string.Join(",", x); //value;
            }
        }

        private void SampleButton_Click(object sender, RoutedEventArgs e)
        {
            Main.lineChart.Series.Remove(LineSeries);
            LineSeries = null;
            score.Content = "";
            UserData.Text = "";
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Main.OpenFile(this);

        }

        private void DrawLine(KeyValuePair<int, Double>[] source)
        {
            //if (LineSeries == null)
            //{
                LineSeries = new LineSeries();
                LineSeries.DependentValuePath = "Value";
                LineSeries.IndependentValuePath = "Key";
                LineSeries.ItemsSource = source;
                Main.lineChart.Series.Add(LineSeries);
            //}
           //LineSeries.
            //LineSeries.Title = this.UserName.Text;

        }

        //TODO change to take X and Y arrays
        private KeyValuePair<int, Double>[] GetXYData(string text)
        {
            var single = GetYData(text);

            string[] points = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            KeyValuePair<int, Double>[] pointsKVP = new KeyValuePair<int, Double>[points.Length];            

            for (int i = 0; i < 40; i++)
            {
                try
                {
                    var value = points[i];
                    //if (value == "end" || value == "reset") continue;
                    pointsKVP[i] = new KeyValuePair<int, Double>(i, Convert.ToDouble(points[i]));
                }
                catch (Exception)
                {
                    MessageBox.Show("Error converting text to data");
                }
            }
            
            return pointsKVP;
        }
        /*
         *        KeyValuePair<int, double>[] trend = new KeyValuePair<int, double>[yData.Length];

            for (int i = 0; i < Data.Length; i++)
            {
                trend[i] = new KeyValuePair<int, Double>(xData.ElementAt(i), yData.ElementAt(i));
            }
         * */

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LineSeries != null)
            {
                LineSeries.Title = this.UserName.Text;
            }
        }

         private int[] GetXData(Double[] yData, int startNumber = 0)
        {
             int[] xData = new int[yData.Length];

             for (int i = 0; i < yData.Length; i++)
             {
                 xData[i] = startNumber + i;
             }

             return xData;
        }

        private Double[] GetYData(string dataText)
        {
            if (dataText.Length < 20)
            {
                return null;
            }

            string[] data = dataText.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Double[] array = Array.ConvertAll(data, s => Convert.ToDouble(s));
            return array;
        }


    }
}
