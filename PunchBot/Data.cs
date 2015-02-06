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
   public class AxesNames 
   {
        public const string RAW = "raw";
        public const string INTERVAL = "interval";
   } 

    public class Data
    {
        private int AverageSampleSize = 8;
        private string _source = "";
        private Double[] X;
        private Double[] Y;

        public KeyValuePair<double, double>[] AxesSource;
        public KeyValuePair<double, double>[] AxesAveraged;
        public KeyValuePair<double, double>[] AxesAveragedTrimmed;
        

        public string Source
        {
            set
            {
                _source = value;

                var ySource = GetYArray(value);
                Y = GetYIntervalArray(ySource);
                X = GetXArray(Y);
                AxesAveraged = GetAverageAxes(Y, AverageSampleSize);

                var endAccelerationLength = GetAccelerationEndIndex(AxesAveraged, AverageSampleSize);

                AxesSource = GetAxes(endAccelerationLength+1);
                AxesAveragedTrimmed = GetAxesAverageTrimmed(endAccelerationLength, AverageSampleSize);
            }
            get
            {
                return _source;
            }
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

            return array;
        }


        //2 axis of original raw data
        private KeyValuePair<Double, Double>[] GetAxes(int length)
        {
                var xArray = X;
                Double[] yArray = Y;
          
                KeyValuePair<double, double>[] xyArray = new KeyValuePair<double, double>[length];

                for (int i = 0; i < length; i++)
                {
                    xyArray[i] = new KeyValuePair<double, double>(xArray.ElementAt(i), yArray.ElementAt(i));
                }

                return xyArray;
            
        }


        public KeyValuePair<double, double>[] GetTrendInfo()
        {
            var xl = new Microsoft.Office.Interop.Excel.Application();
            var wsf = xl.WorksheetFunction;
            double[] x =  AxesSource.Select(pairs => pairs.Key).ToArray();
            double[] y =  AxesSource.Select(pairs => pairs.Value).ToArray();

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

        //Development helper
        public static string LoadSampleData(string path)
        {
            string fileText = System.IO.File.ReadAllText(path);

            return fileText; 
        }

        public int GetAccelerationEndIndex(KeyValuePair<double, double>[] axes, int averageSampleSize)
        {
     
            for (int i = 1; i < axes.Length-1; i++)
            {
                if (axes.ElementAt(i).Value - axes.ElementAt(i+1).Value <= 0)
                {
                    //convert back to the index equivilant in raw data
                    double x = axes.ElementAt(i).Key;
                    double realignedIndex = (x+0.5) - (averageSampleSize/2) + 1;
                    return Convert.ToInt32(realignedIndex);
                }
            }

            return 0;

        }

        // private ---------------------------------------------------

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

        public KeyValuePair<double, double>[] GetAverageAxes(double[] y, int AverageSampleSize)
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
        public KeyValuePair<double, double>[] GetAxesAverageTrimmed(int length, int AverageSampleSize)
        {
            KeyValuePair<double, double>[] subArray = new KeyValuePair<double, double>[length];

            for (int i = 0; i < length; i++)
            {
                subArray[i] = AxesAveraged.ElementAt(i);
            }

            return subArray;
        }

        private Double[] GetXArray(Double[] yData, Double startNumber = 1)
        {
            Double[] xData = new Double[yData.Length];

            for (int i = 0; i < yData.Length; i++)
            {
                xData[i] = startNumber + i;
            }

            return xData;
        }

    }

}
