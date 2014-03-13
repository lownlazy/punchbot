using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.IO.Ports;

namespace PunchBot
{
    public partial class MainWindow : Window
    {
        private SerialPort Serial;
        private String SerialStream = "";

        public MainWindow()
        {
            InitializeComponent();

            string comPort = Properties.Settings.Default.Com;
            int baud = Properties.Settings.Default.Baud;

            initSerialRead(comPort, baud);

        }



        //initialise -----------------------------------------

        private void initSerialRead(string ComPort, int BaudRate)
        {
            if (Serial == null)
            {
                Serial = new SerialPort(ComPort);
                Serial.BaudRate = BaudRate;

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
        

        //Event Handlers -------------------------------------------------

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }


        private void SerialDataHandler(object sender, SerialDataReceivedEventArgs e)
        {
            String comData = Serial.ReadLine();
            
            this.Dispatcher.Invoke((Action)(() =>
             {
                 if (comData.IndexOf("reset") > -1)
                 {
                     DrawLine("");
                     SerialStream = "";
                     textBox1.Text = "reset";
                 }
                 else if (comData.IndexOf("end") > -1)
                {
                    DrawLine(SerialStream);
                    textBox1.Text = SerialStream;
                }
                else
                {
                    SerialStream += comData + "\n";
                }
          }));

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
                try
                {
                    var value = points[i];
                    //if (value == "end" || value == "reset") continue;
                    pointsKVP[i] = new KeyValuePair<int, int>(i, Convert.ToInt32(points[i]));
                }
                catch(Exception ex)
                {
                    continue;
                }
            }

            return pointsKVP;
        }
    }
}
