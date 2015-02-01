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
        public Data data;

        public SeriesInput(MainWindow main)
        {
            Main = main;
            InitializeComponent();

            data = new Data();
            data.Source = Data.LoadSampleData(@"C:\Users\Russell\Source\Repos\punchbot\Assets\hit1.txt");

            DrawLine(data.GetAxes());

            Calculator core = new Calculator();

            //score.Content = core.GetTorque(data.Y).ToString();

            KeyValuePair<int, Double>[] trend = data.GetTrendLine();
            DrawLine(trend);


            //string[] x = core.diffList.Select(n => n.ToString()).ToArray();
            UserData.Text = data.Source; //string.Join(",", x); //value;
        }

        

        /*public string data
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
                
                
            }
        }*/

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

  
        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LineSeries != null)
            {
                LineSeries.Title = this.UserName.Text;
            }
        }

  

    }
}
