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
    public partial class MainWindow : Window
    {
        private SerialPort Serial;
        private DispatcherTimer dispatcherTimer;
        private String SerialStream = "";

        public MainWindow()
        {
            InitializeComponent();
            initTimer();
            initSerialRead("COM3", 9600);
        }

        //initialise -----------------------------------------

        private void initSerialRead(string ComPort, int BaudRate)
        {
            if (Serial == null)
            {
                Serial = new SerialPort(ComPort);
                Serial.BaudRate = 9600;

                try
                {
                    Serial.Open();
                    Serial.DataReceived += new SerialDataReceivedEventHandler(SerialDataHandler);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        
        private void initTimer()
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(TimerHandler);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }


        //Event Handlers -------------------------------------------------

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void TimerHandler(object sender, EventArgs e)
        {
            DrawLine(textBox1.Text);
        }

        void SerialDataHandler(object sender, SerialDataReceivedEventArgs e)
        {
            String comData = Serial.ReadLine();
            Dispatcher.Invoke((Action)(() => textBox1.Text += comData + "\n"));

            if (comData.IndexOf("end") > 0)
            {
                //DrawLine(SerialStream);
                //SerialStream = "";
                textBox1.Text += "xx";
            }
        }

        //helper methods --------------------------------------------
        
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

                textBox1.Text = fileText;

                DrawLine(textBox1.Text);
            }
        }

        private void DrawLine(string text)
        {
            ((LineSeries)lineChart.Series[0]).ItemsSource = ConvertTextToLine(text);
        }

        private KeyValuePair<int, int>[] ConvertTextToLine(string text)
        {

            string[] points = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            KeyValuePair<int, int>[] pointsKVP = new KeyValuePair<int, int>[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == "end") break;
                pointsKVP[i] = new KeyValuePair<int, int>(i, Convert.ToInt32(points[i]));
            }

            return pointsKVP;
        }
    }
}
