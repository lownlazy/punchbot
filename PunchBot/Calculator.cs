using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using PunchBot;


namespace PunchBot.Core
{
    public class Calculator
    {
        int[] Data { get; set; }
        
        //pulse = Optical Rotary Encoder signal pulse
        //Pulses in one full revolution of the ORE
        int PulsesPerRev = Settings1.Default.PulsesPerRevolution;

        //Radians per 360 degreee rovolution
        decimal RadPerRev = 6.283185307M;

        decimal MicroSecondsInOneSecond = 1000000M;

        //value from excel worksheet
        decimal momentOfInertia = 0.077547302902M;



        public Calculator()
        {
           
        }

        public int[] convertData(string dataText)
        {
            if (dataText.Length < 20)
            {
                return null;
            }

            string[] data = dataText.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int[] intArray = Array.ConvertAll(data, s => int.Parse(s));
            return intArray;
        }

        public decimal GetTorque(int[] data)
        {
            

            decimal radiansPerSecondSquared = getRadiansPerSecondSquared(data);

            decimal torque = radiansPerSecondSquared * momentOfInertia;
         
            return torque;
        }

        public decimal getRadiansPerSecondSquared(int[] data)
        {
            var diffList = GetDataDifferences(data);

            //the end of acceleration within the data
            var index = GetEndIndex(diffList);

            decimal AccelerationTime = data[index+1] / MicroSecondsInOneSecond;
            decimal EndRadiansPerSecond = GetRadiansPerSecond(diffList, index);

            //MessageBox.Show("index: " + index + " - diff @ index" + diffList[index].ToString());
            //MessageBox.Show(EndRadiansPerSecond.ToString());

            //decimal averageRadiansPerSecond = radians / time;
            decimal radiansPerSecondSquared = EndRadiansPerSecond / AccelerationTime;

            return radiansPerSecondSquared;
        }
        

        //get the rad/sec speed of the last point of acceleration
        private decimal GetRadiansPerSecond(List<int> diffList, int index)
        {
            decimal timeInSeconds = diffList[index] / MicroSecondsInOneSecond;
            decimal multiplier = 1 / timeInSeconds;

            //Number of Radians in a Pulse
            decimal radPerPulse = RadPerRev / PulsesPerRev;
            decimal radiansPerSecond = radPerPulse * multiplier;

            return radiansPerSecond;
        }



        private int GetEndIndex(List<int> diffList)
        {
            //start at 2 because 1 has an unknown quantity 
            for (int i = 2; i < diffList.Count; i++)
            {
                if (diffList[i + 1] - diffList[i] >= 0)
                {
                    return i;
                }
            }

            return 0;
            
        }

        //a list of the time differences between each data sample
        private List<int> GetDataDifferences(int[] data)
        {
            var list = new List<int>();

            for (int i=0; i < data.Length-1; i++ )
            {
                list.Add(data[i+1] - data[i]);
            }

            return list;
        }


        //Development helper
        public static string[] GetData(string path)
        {           
             string fileText = System.IO.File.ReadAllText(path);

             return fileText.Split(',');
        }
    }
}
