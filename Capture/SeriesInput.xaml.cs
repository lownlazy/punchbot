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
using PunchBot.Capture;

namespace PunchBot
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
                DrawLine(value);
                UserData.Text = value;
            }
        }

        private void OpenFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                string fileText = System.IO.File.ReadAllText(filename);

                UserData.Text = fileText;

                DrawLine(UserData.Text);            
            }
        }

        private void SampleButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void DrawLine(string text)
        {
            if (LineSeries == null)
            {
                LineSeries = new LineSeries();
                LineSeries.DependentValuePath = "Value";
                LineSeries.IndependentValuePath = "Key";
                LineSeries.ItemsSource = ConvertTextToLine(text);
                Main.lineChart.Series.Add(LineSeries);
            }
           
            LineSeries.Title = this.UserName.Text;
            LineSeries.ItemsSource = ConvertTextToLine(text);
        }

        private KeyValuePair<int, int>[] ConvertTextToLine(string text)
        {

            string[] points = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            KeyValuePair<int, int>[] pointsKVP = new KeyValuePair<int, int>[points.Length];
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    var value = points[i];
                    //if (value == "end" || value == "reset") continue;
                    pointsKVP[i] = new KeyValuePair<int, int>(i, Convert.ToInt32(points[i]));
                }
                catch (Exception)
                {
                    continue;
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
    }
}
