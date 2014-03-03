using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PunchBot
{
    public class Input
    {
        public ObservableCollection<Point> Points { get; private set; }
        public ObservableCollection<KeyValuePair<string, int>> ValueList { get; private set; }

        public Input()
        {
            this.ValueList = new ObservableCollection<KeyValuePair<string, int>>();
            ValueList.Add(new KeyValuePair<string, int>("Developer", 60));
            ValueList.Add(new KeyValuePair<string, int>("Misc", 20));
            ValueList.Add(new KeyValuePair<string, int>("Tester", 50));
            ValueList.Add(new KeyValuePair<string, int>("QA", 30));
            ValueList.Add(new KeyValuePair<string, int>("Project Manager", 40));

            Points = new ObservableCollection<Point>(new Point[] { new Point(1, 1), new Point(2, 2) });
        }

        public void Add(KeyValuePair<string, int> data)
        {
            ValueList.Add(data);
        }
    }
}
