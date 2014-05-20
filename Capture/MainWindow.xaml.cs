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

            addInput();
        }

        public void addInput()
        {
            SeriesInput seriesInput = new SeriesInput(this);
            controlPanel.Children.Add(seriesInput);
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
                     //DrawLine("");
                     SerialStream = "";
                     //seriesInput1. textBox1.Text = "reset";
                     placeData("reset", false);
                 }
                 else if (comData.IndexOf("end") > -1)
                {
                    //DrawLine(SerialStream);
                    //textBox1.Text = SerialStream;
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
            foreach(var child in this.controlPanel.Children)
            {
                if(child.GetType() == typeof(SeriesInput))
                {
                    SeriesInput child2 = child as SeriesInput;

                    if (child2.UserName.Text == "")
                    {
                        if (draw) child2.data = text;
                        else child2.UserData.Text = text;

                        break;
                    }
                }

               
            }
        }


      
    }
}
