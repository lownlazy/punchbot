using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            initSerialRead("COM4", 9600);
        }

        private SerialPort Serial;

        private void initSerialRead(string ComPort, int BaudRate)
        {
            if (Serial == null)
            {
                Serial = new SerialPort(ComPort, BaudRate);//,Parity.None,8,StopBits.One);
                //Serial.Handshake = Handshake.None;
                //Serial.RtsEnable = true;
                //Serial.DtrEnable = true;

                try
                {
                    Serial.Open();
                    Serial.DataReceived += new SerialDataReceivedEventHandler(SerialDataHandler);
                    Serial.ErrorReceived += new SerialErrorReceivedEventHandler(sPort_ErrorReceived);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void sPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }


        //Event Handlers -------------------------------------------------

    

        private void SerialDataHandler(object sender, SerialDataReceivedEventArgs e)
        {
            String comData = Serial.ReadLine();

            

        }

    }
}
