using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.IO.Ports;
using Capture;

namespace Capture
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

            //addInput();
        }

        public SeriesInput addInput()
        {
            SeriesInput seriesInput = new SeriesInput(this);
            seriesInput.UserName.Text = controlPanel.Children.Count.ToString();
            controlPanel.Children.Add(seriesInput);
            return seriesInput;
        }

        public void OpenFile(SeriesInput seriesInput)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                string fileText = System.IO.File.ReadAllText(filename);

                seriesInput.data = fileText;

                //seriesInput.DrawLine(UserData.Text);
            }
        }

        //initialise -----------------------------------------

        private void initSerialRead(string ComPort, int BaudRate)
        {
            if (Serial == null)
            {
                Serial = new SerialPort(ComPort, BaudRate);

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

        private void AddInputButton_Click(object sender, RoutedEventArgs e)
        {
            addInput();
        }

        private void SerialDataHandler(object sender, SerialDataReceivedEventArgs e)
        {
            String comData = Serial.ReadLine();
            
            this.Dispatcher.Invoke((Action)(() =>
             {
                 if (comData.IndexOf("reset") > -1)
                 {
                     SerialStream = "";
                     placeData("reset", false);
                 }
                 else if (comData.IndexOf("end") > -1)
                {
                    placeData(SerialStream);
                }
                else
                {
                    SerialStream += comData + "\n";
                }
          }));

        }



        //helper methods --------------------------------------------
        
        private void placeData(string text, bool draw = true)
        {
            SeriesInput seriesInput = null;

            foreach(var item in this.controlPanel.Children)
            {
                if (item.GetType() == typeof(SeriesInput))
                {
                    var temp = item as SeriesInput;
                    if (temp.UserData.Text.Length < 20)
                    {
                        seriesInput = temp;
                        break;
                    }
                }
            }

            if (seriesInput == null) seriesInput = addInput();

            if (draw) seriesInput.data = text;
            else seriesInput.UserData.Text = text;
        }


      
    }
}
