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
        double radPerRev = 6.283185307;


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

        public double GetTorque(int[] data)
        {
            double rs2 = getAcceleration(data);
            double moment = 1.000000000; //value from excel worksheet
            double torque = rs2 * moment;
         
            return torque;
        }

        public double getAcceleration(int[] data)
        {          
            //the end of acceleration within the data
            var index = GetEndIndex(data);

            double time = GetAccelerationSeconds(data, index);
            double radians = GetAccelerationRadians(index);

            return radians / time;
        }

        private double GetAccelerationRadians(int index)
        {
            //Number of Radians in a Pulse
            double radPerPulse = radPerRev / pulsesPerRev;
            double radians = index + 1 * radPerPulse;

            return radians;
        }

        //in seconds - converted from microseconds
        private double GetAccelerationSeconds(int[] data, int index)
        {
            int microSecondsInOneSecond = 1000000;

            int[] trimmedData = data.Take(index + 1).ToArray();
            int sum = trimmedData.Sum(); //microseconds
            double seconds = sum / microSecondsInOneSecond;
            return seconds;
        }

        public int GetEndIndex(int[] data)
        {
            for (int i = 1; i < data.Length; i++)
            {
                int d1 = data[i + 1];
                int d0 = data[i];
                int diff = d1 - d0;

                if(diff <= 0)
                {
                    return i;
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
