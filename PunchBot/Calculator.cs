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
        int PPR = Settings1.Default.PulsesPerRevolution;


        //value from excel worksheet
        //I is the math notation for Moment of Intertia
        double I = 0.06413274;

        public Double Duration = 0;
        public Double Work = 0; //radians/sec
        public Double Acceleration = 0; //radians/sec/sec
        public Double Torque = 0; //radians/sec/sec
        public Double Power = 0;
        public Double Watts = 0; 

        public Calculator(Data data)
        {
            Duration = data.DurationOfTrend / Math.Pow(10, 6);
            var radians = Math.PI*2/PPR * data.LengthOfTrend;
            Work = radians / Duration;
            Acceleration = Work / Duration;
            Torque = I * Acceleration;
            Power = Work * Torque;
            Watts = Power * Duration;
        }


    }
}
