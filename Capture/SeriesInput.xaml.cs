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
        }

        public string data
        {
            set {
                var data = ConvertTextToLine(value);

                //check for irrelivant data, usually caused by the head being bumped or is return bounce
                if (data.ElementAt(5).Value > 40000)
                {
                    //MessageBox.Show("data error: " + value.Substring(0, 100));
                    //return;
                }
                
                DrawLine(data);
                

                Calculator core = new Calculator();
                var intData = ConvertData(value);

                score.Content = core.GetTorque(intData).ToString();

                //string[] x = core.diffList.Select(n => n.ToString()).ToArray();
                UserData.Text = value; //string.Join(",", x); //value;
            }
        }

        private void SampleButton_Click(object sender, RoutedEventArgs e)
        {
            //OpenFile();
            Main.lineChart.Series.Remove(LineSeries);
            LineSeries = null;
            score.Content = "";
            UserData.Text = "";
        }

        private void DrawLine(KeyValuePair<int, Double>[] source)
        {
            if (LineSeries == null)
            {
                LineSeries = new LineSeries();
                LineSeries.DependentValuePath = "Value";
                LineSeries.IndependentValuePath = "Key";
                LineSeries.ItemsSource = source;
                Main.lineChart.Series.Add(LineSeries);
            }
           
            LineSeries.Title = this.UserName.Text;

        }

        private KeyValuePair<int, Double>[] ConvertTextToLine(string text)
        {

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

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LineSeries != null)
            {
                LineSeries.Title = this.UserName.Text;
            }
        }

        private Double[] ConvertData(string dataText)
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
