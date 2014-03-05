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
using Microsoft.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;

namespace PunchBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //data = new Input();
            InitializeComponent();

            //var series = new LineSeries();
            //series.Title = "fred";
            //lineChart.Series.Add(series);
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                string filename = dlg.FileName;
                string fileText = System.IO.File.ReadAllText(filename);

                textBox1.Text = fileText;

                ((LineSeries)lineChart.Series[1]).ItemsSource = ConvertTextToLine(fileText);
            } 
        }

        private KeyValuePair<int, int>[]  ConvertTextToLine(string text)
        {
            
            string[] points = text.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            KeyValuePair<int, int>[] pointsKVP = new KeyValuePair<int, int>[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                pointsKVP[i] = new KeyValuePair<int, int>(i, Convert.ToInt32(points[i]));
            }

            return pointsKVP;
        }

  


    }
}
