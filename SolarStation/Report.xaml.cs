using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using SolarStation;


namespace SolarStation
{
    /// <summary>
    /// Interaction logic for PanelInf.xaml
    /// </summary>
    public partial class Report : Window
    {
        MainWindow parent;
        public Report()
        {
            InitializeComponent();
            parent = (MainWindow)Application.Current.MainWindow;
            FillChart();
            MakeReport();
        }

        public void FillChart()
        {
            Style styleLegend = new Style { TargetType = typeof(Control) };
            styleLegend.Setters.Add(new Setter(Control.WidthProperty, 0d));
            styleLegend.Setters.Add(new Setter(Control.HeightProperty, 0d));
            Chart.LegendStyle = styleLegend;

            List<KeyValuePair<string, double>> KeyValue = new List<KeyValuePair<string, double>>();
            DateTime date = parent.DatePick.SelectedDate.Value;
            double perDayPower = 0;

            if (date.Year != 2019)
            {
                int years = 2019 - date.Year;
                date = date.AddYears(years);
            }
            if (parent.isTrackSun.IsChecked == true)
            {
                foreach (var time in parent.sp.SolarInsalations.Where(x => x.Date == date).ToList())
                {
                    double power = parent.SolarPanelSelected.CalculatePower((int)time.ETRN, parent.panelAmount);
                    perDayPower += power;
                    KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
                }
                ((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
            }
            else
            {
                foreach (var time in parent.sp.SolarInsalations.Where(x => x.Date == date).ToList())
                {
                    double power = parent.SolarPanelSelected.CalculatePower((int)time.ETR, parent.panelAmount);
                    perDayPower += power;
                    KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
                }
                ((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
            }
            //day.Text = perDayPower.ToString("#.##");


        }


        public void MakeReport()
        {
            //parent.ShowInfo();
            Solar_Panels itemInf = parent.SolarPanelListCB.SelectedItem as Solar_Panels;
            headerReport.Text= itemInf.NamePanel.ToString();
            double kWh = (double)itemInf.NominalPower_W / 1000;
            V.Text = kWh.ToString();
            PerDay.Text = parent.day.Text;
           //priceOne.Text= itemInf.Price__.ToString();
           //double paid = itemInf.Price__ * parent.PanelAmountSl.Value;
           //priceSum.Text = paid.ToString();
            if (parent.isTrackSun.IsChecked==true)
            {
                @checked.Visibility = Visibility.Visible;
                @checked.IsEnabled = true;
            }
            else
            {
                @unchecked.Visibility = Visibility.Visible;
                @unchecked.IsEnabled = true;
            }

            panelnumber.Text = parent.PanelAmountSl.Value.ToString();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(ReportGrid, "");
            }
        }

       

    }
}
