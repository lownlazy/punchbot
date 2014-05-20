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

        /*public decimal GetTorque(int[] data)
        {

        }*/

        public double getAcceleration(int[] data)
        {
            var microSecondsInOneSecond = 1000000;
            //Radians per 360 degreee rovolution
            var radPerRev = 6.283185307;
            //ticks = Optical rotary encoder signals
            //Ticker in one full revolution
            var ticksPerRev = 1024;
            var radPerTick = radPerRev / ticksPerRev;
            var AccelerationEndTime = data[GetEndIndex(data)]; //<-- differnet but better from paper version
            var secondsPerTick = AccelerationEndTime / microSecondsInOneSecond;
            var index = GetEndIndex(data);
            var time = GetTime(data, index);
            return time;
        }

        //in seconds - converted from microseconds
        private int GetTime(int[] data, int index)
        {
            int[] trimmedData = data.Take(index + 1).ToArray();
            int sum = trimmedData.Sum();
            return sum;
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
