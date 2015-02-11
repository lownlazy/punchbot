using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PunchBot.Extensions;
using System.Reflection;
using System.Collections;

namespace PunchBot.Core
{

    public class Data
    {
       
        private int SampleSizeForAverage = Settings1.Default.SampleSizeForAverage;
        private string _source = "";
        private Double[] X;
        private Double[] RawYInterval;

        public KeyValuePair<double, double>[] RawAxes;
        public KeyValuePair<double, double>[] TrendAxes;
        public double[] TrendY;
        public double TrendIntervalAverage = 0;

        public string Source
        {
            set
            {
                _source = value;

                Double[] ySource = GetYArray(value);
                RawYInterval = GetYIntervalArray(ySource);
                double[] RawYAvg = GetYIntervalAverage(RawYInterval, SampleSizeForAverage);
                X = GetXArray(RawYInterval);
               // AxesAveraged = GetAverageAxes(Y, AverageSampleSize);

                var endAccelerationLength = GetAccelerationEndIndex(RawYAvg, SampleSizeForAverage);
                RawAxes = GetAxes(endAccelerationLength+1, X, ySource);
                TrendAxes = GetTrend(endAccelerationLength,X, ySource);
                TrendY = TrendAxes.Select(pairs => pairs.Value).ToArray();
            }
            get
            {
                return _source;
            }
        }

        //returns the average millisecond interval from the trend curve data
        public double AverageIntervalTimeFromTrend
        {
            get
            {
                //get average interval length
                double[] intervalY = GetYIntervalArray(TrendY);
                double trendIntervalAverage = intervalY.Average();
                return trendIntervalAverage;
            }
        }

        //in micro seconds
        public double DurationOfTrend
        {
            get
            {
                return TrendY[TrendY.Length-1];
            }
        }

        //in micro seconds
        public double LengthOfTrend
        {
            get
            {
                return TrendY.Length;
            }
        }

        // private ---------------------------------------------------

        private double[] GetYIntervalAverage(double[] y, int sampleSize)
        {
            double[] yAvg = new double[y.Length - sampleSize];

            for (int i = 0; i < y.Length - sampleSize; i++)
            {
                Double[] subArray = y.SubArray(i, sampleSize);
                double value = subArray.Average();

                yAvg[i] = value;
            }

            return yAvg;
        }

        private Double[] GetYArray(string dataText)
        {
            if (dataText.Length < 20)
            {
                return null;
            }

            string[] data = dataText.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            double[] array = Array.ConvertAll(data, s => Convert.ToDouble(s));
            //Array.Resize<double>(ref array, 25);

            //start the count from Zero
            var startValue = array[0];
            array = array.Select(s => s -startValue).ToArray();

            return array;
        }


        //2 axis of original raw data
        private KeyValuePair<Double, Double>[] GetAxes(int length, Double[] x, Double[] y)
        {
         
          
                KeyValuePair<double, double>[] xyArray = new KeyValuePair<double, double>[length];

                for (int i = 0; i < length; i++)
                {
                    xyArray[i] = new KeyValuePair<double, double>(x.ElementAt(i), y.ElementAt(i));
                }

                return xyArray; 
        }


        private KeyValuePair<double, double>[] GetTrend(int length, Double[] x, Double[] y)
        {
            var xl = new Microsoft.Office.Interop.Excel.Application();
            var wsf = xl.WorksheetFunction;
            //double[] x = source.Select(pairs => pairs.Key).ToArray().RemoveAt(0); 
            //double[] y = source.Select(pairs => pairs.Value).ToArray().RemoveAt(0); 
            x = x.SubArray(0, length-1);//.RemoveAt(0);
            y = y.SubArray(0, length).RemoveAt(0);

            double[] nlX = x.Select(i => wsf.Ln(i)).ToArray();
            double[] nlY = y.Select(i => wsf.Ln(i)).ToArray();

            xl.Visible = false;

            object o = wsf.LinEst(nlY, nlX, true, false);
            string[] array = ((IEnumerable)o).Cast<object>().Select(i => i.ToString()).ToArray();
            Double power = Convert.ToDouble(array[0]);
            Double val = Convert.ToDouble(array[1]);
            Double multiplier = Convert.ToDouble(wsf.ImExp(val));

            KeyValuePair<double, double>[] trend = new KeyValuePair<double, double>[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                double key = x.ElementAt(i);
                double trendCurveSmoothedValue = multiplier * Math.Pow(key, power);
                trend[i] = new KeyValuePair<double, double>(key, trendCurveSmoothedValue);
            }

            return trend;
        }


        private int GetAccelerationEndIndex(double[] array, int sampleSize)
        {
            var min = Convert.ToDouble(array.Min());
            var index = Array.IndexOf(array, min);

            return index + sampleSize/2;

        }


        //a list of the time differences between each data sample
        private static Double[] GetYIntervalArray(Double[] y)
        {
            var array = new Double[y.Length-1];

            for (int i = 0; i < y.Length - 1; i++)
            {
                array[i] = y[i + 1] - y[i];
            }

            return array;
        }

        private KeyValuePair<double, double>[] GetAverageAxes(double[] y, int AverageSampleSize)
        {
            KeyValuePair<double, double>[] avgAxes = new KeyValuePair<double, double>[y.Length];

            for (int i = 0; i < y.Length - AverageSampleSize; i++)
            {
                Double[] subArray = y.SubArray(i,AverageSampleSize);
                double value = subArray.Average();
                double key = i + (AverageSampleSize/2) + 0.5;

               avgAxes[i] = new KeyValuePair<Double, Double>(key, value);     
            }

            return avgAxes;
        }

        //trimmed to acceleration end
        //public KeyValuePair<double, double>[] GetAxesAverageTrimmed(KeyValuePair<double, double>[] axesAvg, int length, int AverageSampleSize)
        //{
        //    KeyValuePair<double, double>[] subArray = new KeyValuePair<double, double>[length];

        //    for (int i = 0; i < length; i++)
        //    {
        //        subArray[i] = axesAvg.ElementAt(i);
        //    }

        //    return subArray;
        //}

        private Double[] GetXArray(Double[] yData, Double startNumber = 1)
        {
            Double[] xData = new Double[yData.Length];

            for (int i = 0; i < yData.Length; i++)
            {
                xData[i] = startNumber + i;
            }

            return xData;
        }


        //Development helper
        public static string LoadSampleData(string path)
        {
            string fileText = System.IO.File.ReadAllText(path);

            return fileText;
        }

    }

}
