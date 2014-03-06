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
using System.IO.Ports;
using System.Windows.Threading;

namespace PunchBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string comportnum;
        public delegate void NoArgDelegate();
        public static SerialPort serialX;
        public DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            initTimer();
            comportnum = "COM" + 3; 

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
                if (points[i] == "end") break;
                pointsKVP[i] = new KeyValuePair<int, int>(i, Convert.ToInt32(points[i]));
            }

            return pointsKVP;
        }

        private void readSerialButton_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "";

            if (serialX == null)
            {
                serialX = new SerialPort(comportnum);
                serialX.BaudRate = 9600;

                try
                {
                    serialX.Open();

                    serialX.DataReceived += new SerialDataReceivedEventHandler(serialX_DataReceived);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        void serialX_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String comdata;
            comdata = serialX.ReadLine();
            Dispatcher.Invoke((Action)(() => textBox1.Text += comdata + "\n"));

            dispatcherTimer.Start();
        }

        void initTimer()
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ((LineSeries)lineChart.Series[1]).ItemsSource = ConvertTextToLine(textBox1.Text);
        }
        

    }
}
