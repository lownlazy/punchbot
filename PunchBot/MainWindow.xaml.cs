using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls;

namespace PunchBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<KeyValuePair<string, int>> ValueList { get; private set; }
        public PointCollection Points = new PointCollection(new Point[] { new Point(1, 1), new Point(2, 2) });
        private Input data;

        public MainWindow()
        {
            data = new Input();
            InitializeComponent();
            this.DataContext = data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //PointCollection points = new PointCollection(new Point[] { new Point(1, 1), new Point(2, 9) });
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.data.Add(new KeyValuePair<string, int>("XXX", 27));
        }



    }
}
