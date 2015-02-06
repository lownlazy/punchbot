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
        LineSeries series;
        public Data _data;

        public SeriesInput(MainWindow main)
        {
            Main = main;
            InitializeComponent();

    
        }

        

        public string data
        {
            set {
                
             _data = new Data();
            //data.Source = Data.LoadSampleData(@"C:\Users\Russell\Source\Repos\punchbot\Assets\hit1.txt");
            _data.Source = value;

            //DrawLine(_data.AxesSource, "source");
            //DrawLine(_data.AxesAveragedTrimmed, "average");
            DrawLine(_data.GetTrendInfo(), "trend");

            //Calculator core = new Calculator();
            //score.Content = core.GetTorque(data.Y).ToString();
          
            //string[] x = core.diffList.Select(n => n.ToString()).ToArray();
            UserData.Text = _data.Source; //string.Join(",", x); //value;

                //check for irrelivant data, usually caused by the head being bumped or is return bounce
                //if (data.ElementAt(5).Value > 40000)
                //{
                    //MessageBox.Show("data error: " + value.Substring(0, 100));
                    //return;
                //}
                
                
            }
        }

        private void SampleButton_Click(object sender, RoutedEventArgs e)
        {
            Main.lineChart.Series.Remove(series);
            series = null;
            score.Content = "";
            UserData.Text = "";
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Main.OpenFile(this);

        }

        private void DrawLine(KeyValuePair<Double, Double>[] source, string title = "")
        {
            if (series == null)
            {
                series = new LineSeries();
                series.DependentValuePath = "Value";
                series.IndependentValuePath = "Key";
                series.ItemsSource = source;

       
                Main.lineChart.Series.Add(series);
            }
           
                if (title == "")
                {
                    series.Title = this.UserName.Text;
                }
                else {
                    series.Title = title;
                }
        }

  
        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (series != null)
            {
                series.Title = this.UserName.Text;
            }
        }

  

    }
}
