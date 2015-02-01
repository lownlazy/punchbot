﻿using System;
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


        public decimal GetTorque(Double[] data)
        {
           
            decimal radiansPerSecondSquared = getRadiansPerSecondSquared(data);

            decimal torque = radiansPerSecondSquared * momentOfInertia;
         
            return torque;
        }

  

        public decimal getRadiansPerSecondSquared(Double[] rawData)
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
        }

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



        private int GetEndIndex(Double[] rawData)
        {
            var diffList = GetDataDifferences(rawData);

            for (int i = 1; i < diffList.Count; i++)
            {
                if (diffList[i + 1] - diffList[i] >= 0)
                {
                    //+1 because we want the correct index inside the raw data
                    return i+1;
                }
            }

            return 0;
            
        }

        //a list of the time differences between each data sample
        private List<Double> GetDataDifferences(Double[] data)
        {
            var list = new List<Double>();

            for (int i=0; i < data.Length-1; i++ )
            {
                list.Add(data[i+1] - data[i]);
            }

            return list;
        }

        public KeyValuePair<int, double>[] GetTrendLine(int[] xData, double[] yData)
        {

            var xl = new Microsoft.Office.Interop.Excel.Application();
		    xl.Visible = true;

            var wsf = xl.WorksheetFunction;
            List<int> x = new List<int> { 1, 2, 3, 4 };
            List<int> y = new List<int> { 11, 12, 45, 42 };
            object o = wsf.LinEst(y.ToArray(), x.ToArray(), false, true);

            KeyValuePair<int, double>[] trend = new KeyValuePair<int, double>[yData.Length];

            for (int i = 0; i < yData.Length; i++)
            {
                trend[i] = new KeyValuePair<int, Double>(xData.ElementAt(i), yData.ElementAt(i));
            }

            return trend;
        }

        //Development helper
        public static string GetData(string path)
        {           
             string fileText = System.IO.File.ReadAllText(path);

             return fileText; //fileText.Split(',');
        }

      
    }
}
