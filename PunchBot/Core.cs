using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PunchBot.Core
{
    public class Core
    {
        int[] Data { get; set; }

        public Core(int[] data)
        {
            Data = data;
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
            //pulse = Optical Rotary Encoder signal pulse
            //Pulses in one full revolution of the ORE
            var pulsesPerRev = 1024;     
       
            //Radians per 360 degreee rovolution
            var radPerRev = 6.283185307;

            //Number of Radians in a Pulse
            var radPerPulse = radPerRev / pulsesPerRev;
            
            //the end of acceleration within the data
            var index = GetEndIndex(data);

            double time = GetAccelerationSeconds(data, index);
           // var radians = GetAccelerationRadians(data, index);

            return time;
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

                if(diff >= 0)
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
