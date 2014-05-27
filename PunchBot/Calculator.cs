using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PunchBot.Core
{
    public class Calculator
    {
        int[] Data { get; set; }
        
        //pulse = Optical Rotary Encoder signal pulse
        //Pulses in one full revolution of the ORE
        int pulsesPerRev = 1024;

        //Radians per 360 degreee rovolution
        decimal radPerRev = 6.283185307M;


        public Calculator()
        {
            //Data = data;
        }

        public int[] convertData(string dataText)
        {
            string[] data = dataText.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int[] intArray = Array.ConvertAll(data, s => int.Parse(s));
            return intArray;
        }

        public decimal GetTorque(int[] data)
        {
            decimal radiansPerSecondSquared = getRadiansPerSecondSquared(data);

            //get radians per second squared
            decimal moment = 0.07614M; //value from excel worksheet
            decimal torque = radiansPerSecondSquared * moment;
         
            return torque;
        }

        public decimal getRadiansPerSecondSquared(int[] data)
        {          
            //the end of acceleration within the data
            var index = GetEndIndex(data);

            decimal time = GetAccelerationSeconds(data, index);
            decimal radians = GetRadians(index);

            decimal radiansPerSecond = radians / time;
            decimal radiansPerSecondSquared = radiansPerSecond / time;

            return radiansPerSecondSquared;
        }

        private decimal GetRadians(int index)
        {
            //Number of Radians in a Pulse
            decimal radPerPulse = radPerRev / pulsesPerRev;
            decimal radians = index * radPerPulse;

            return radians;
        }

        //in seconds - converted from microseconds
        private decimal GetAccelerationSeconds(int[] data, int index)
        {
            decimal microSecondsInOneSecond = 1000000M;

            int[] trimmedData = data.Take(index + 1).ToArray();
            int sum = trimmedData.Sum(); //microseconds
            decimal seconds = sum / microSecondsInOneSecond;
            return seconds;
        }

        public int GetEndIndex(int[] data)
        {
            for (int i = 1; i < data.Length; i++)
            {
                int d0 = data[i];
                int d1 = data[i + 1];
                int d2 = data[i + 2];
                
                int currentSpan = d1 - d0;
                int nextSpan = d2 - d1;

                if (currentSpan <= nextSpan)
                {
                    return i+1;
                }
            }

            //throw new Error()
            return 0;
        }



        //Development helper
        public static string[] GetData(string path)
        {           
             string fileText = System.IO.File.ReadAllText(path);

             return fileText.Split(',');
        }
    }
}
