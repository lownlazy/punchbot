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
using PunchBot.Core;
using Microsoft.Office.Interop.Excel;


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
        decimal momentOfInertia = 0.06413274M;

        public List<int> diffList;

        public Calculator()
        {
           
        }


        /*public decimal GetTorque(Double[] data)
        {
           
            decimal radiansPerSecondSquared = getRadiansPerSecondSquared(data);

            decimal torque = radiansPerSecondSquared * momentOfInertia;
         
            return torque;
        }*/

  

        /*public decimal getRadiansPerSecondSquared(Double[] rawData)
        {
            //the end of acceleration within the data
            var accelEndIndex = GetEndIndex(rawData);

            var startTime = rawData[1]; //0-1 range is invalid
            var endTime = rawData[accelEndIndex];

            decimal AccelTimeInSec = (Convert.ToDecimal(endTime) - Convert.ToDecimal(startTime)) / MicroSecondsInOneSecond;
            // - 1 to remove invalid first value, then + 1 because we want the count (not the index)
            decimal radians = GetRadians(accelEndIndex - 1+1);
            decimal radiansPerSecond = radians / AccelTimeInSec;

            MessageBox.Show("accelEndIndex: " + accelEndIndex);
            //MessageBox.Show(EndRadiansPerSecond.ToString());

            decimal radiansPerSecondSquared = radiansPerSecond / AccelTimeInSec;

            return radiansPerSecondSquared;
        }*/

        //radians covered during the acceleration period
        private decimal GetRadians(int pulsesInAcccelPeriod)
        {
            decimal radPerPulse = RadPerRev / PulsesPerRev * pulsesInAcccelPeriod;

            return radPerPulse;
        }

        //get the rad/sec speed of the last point of acceleration
        //private decimal GetRadiansPerSecond(int[] rawData, int accelEndIndex)
        //{
        //    va
        //    decimal accelTimeInSeconds = rawData[accelEndIndex] / MicroSecondsInOneSecond;
        //    decimal multiplier = 1 / accelTimeInSeconds;

        //    //Number of Radians in a Pulse
        //    decimal radPerPulse = RadPerRev / PulsesPerRev;
        //    decimal radiansPerSecond = radPerPulse * multiplier;

        //    return radiansPerSecond;
        //}





    }
}
