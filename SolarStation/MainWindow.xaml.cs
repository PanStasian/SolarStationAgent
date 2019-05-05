using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Data;
using System.Windows.Controls.DataVisualization.Charting;

namespace SolarStation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int panelAmount;
        Solar_Panels SolarPanelSelected;
        //Chart dynamicChart = new Chart();
        //LineSeries lineSeries = new LineSeries();
        SolarPanelEntities sp = new SolarPanelEntities();
        public List<Solar_Panels> SolPal { get; set; }
        //public List<SolarInsalation> SolIns { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            DatePick.SelectedDate = DateTime.Now;
            BindCB();
            SolarPanelListCB.ItemsSource = sp.Solar_Panels.ToList();
            PanelAmountSl.ValueChanged += Slider_ValueChanged;
            PanelAmountSl.Value = 10;
            isTrackSun.Checked += checkBox_Checked;
            isTrackSun.Unchecked += checkBox_Unchecked;
            //FillChart();
        }


        private void BindCB()
        {
            var item = sp.Solar_Panels.ToList();
            SolPal = item;
            DataContext = SolPal;
        }

        public void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SolarPanelSelected = SolarPanelListCB.SelectedItem as Solar_Panels;
            double kWh = (double)SolarPanelSelected.NominalPower_W / 1000;
            V.Text = kWh.ToString();
            FillChart();
        }

        public void InfBtn_Click(object sender, RoutedEventArgs e)
        {
            PanelInf infPan = new PanelInf();
            infPan.Show();
        }

        
        public void FillChart()
        {
            List<KeyValuePair<string, double>> KeyValue = new List<KeyValuePair<string, double>>();
            DateTime date = DatePick.SelectedDate.Value;

            if (date.Year != 2019)
            {
                int years = 2019 - date.Year;
                date=date.AddYears(years);
            }
            if (isTrackSun.IsChecked==true)
            {
                foreach (var time in sp.SolarInsalations.Where(x => x.Date == date).ToList())
                {
                    double power = SolarPanelSelected.CalculatePower((int)time.ETRN, panelAmount);
                    KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
                }
                ((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
            }
            else
            {
                foreach (var time in sp.SolarInsalations.Where(x => x.Date == date).ToList())
                {
                    double power = SolarPanelSelected.CalculatePower((int)time.ETR, panelAmount);
                    KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
                }
                ((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
            }
            
            //foreach(var time in sp.SolarInsalations.Where(x => x.Date == date).ToList())
            //{
            //    if(isTrackSun.IsChecked == true)
            //    {
            //        double power = SolarPanelSelected.CalculatePower((int)time.ETRN, panelAmount);
            //        KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
            //    }
            //    else
            //    {
            //        double power = SolarPanelSelected.CalculatePower((int)time.ETR, panelAmount);
            //        KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
            //    }
            //    
            //}
            //((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
            this.panelAmount = (int)PanelAmountSl.Value;
            FillChart();
        }
        private void CalculateBtn_Click(object sender, RoutedEventArgs e)
        {
            FillChart();
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            FillChart();
        }
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            FillChart();
        }

        
    }
}
