using System;
using System.Collections.Generic;
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

namespace SolarStation
{
    /// <summary>
    /// Interaction logic for DateReportxaml.xaml
    /// </summary>
    public partial class DateReportxaml : Window
    {
        MainWindow parent;
        public DateReportxaml()
        {
            InitializeComponent();
            parent = (MainWindow)Application.Current.MainWindow;
            MakeReport();
            FillChartForDate();
        }

        public void MakeReport()
        {
            Solar_Panels itemInf = parent.SolarPanelListCB.SelectedItem as Solar_Panels;
            headerReport.Text = itemInf.NamePanel.ToString();
            Start.Text = parent.startDate.SelectedDate.Value.ToShortDateString();
            End.Text = parent.endDate.SelectedDate.Value.ToShortDateString();
            double kWh = (double)itemInf.NominalPower_W / 1000;
            V.Text = kWh.ToString();
            PerDay.Text = parent.energySum.Text;
            double paid = itemInf.Price__ * parent.PanelAmountSl.Value;
            priceSum.Text = paid.ToString();
            if (parent.isTrackSun.IsChecked == true)
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

        public void FillChartForDate()
        {
            Style styleLegend = new Style { TargetType = typeof(Control) };
            styleLegend.Setters.Add(new Setter(Control.WidthProperty, 0d));
            styleLegend.Setters.Add(new Setter(Control.HeightProperty, 0d));
            StatisticChart.LegendStyle = styleLegend;

            List<KeyValuePair<string, double>> KeyValue = new List<KeyValuePair<string, double>>();
            DateTime startdate = parent.startDate.SelectedDate.Value;
            DateTime enddate = parent.endDate.SelectedDate.Value;
            double perDayPower = 0;
            double perdaySave = 0;

            double greenTax = 0.18;
            double greenMoney = 0;

            if (startdate.Year != 2019)
            {
                int years = 2019 - startdate.Year;
                startdate = startdate.AddYears(years);
            }
            perDayPower = 0;
            if (parent.isTrackSun.IsChecked == true)
            {
                for (DateTime current = startdate; current <= enddate; current = current.AddDays(1))
                {
                    foreach (var time in parent.sp.SolarInsalations.Where(x => x.Date == current))
                    {
                        double power = parent.SolarPanelSelected.CalculatePower((int)time.ETRN, parent.panelAmount);
                        perDayPower += power;
                        perdaySave += power;
                    }
                    KeyValue.Add(new KeyValuePair<string, double>(current.ToShortDateString(), perDayPower));
                    perDayPower = 0;
                }
                ((LineSeries)StatisticChart.Series[0]).ItemsSource = KeyValue;
            }
            else
            {
                for (DateTime current = startdate; current <= enddate; current = current.AddDays(1))
                {
                    foreach (var time in parent.sp.SolarInsalations.Where(x => x.Date == current))
                    {
                        double power = parent.SolarPanelSelected.CalculatePower((int)time.ETR, parent.panelAmount);
                        perDayPower += power;
                        perdaySave += power;
                    }
                    KeyValue.Add(new KeyValuePair<string, double>(current.ToShortDateString(), perDayPower));
                    perDayPower = 0;
                }
                ((LineSeries)StatisticChart.Series[0]).ItemsSource = KeyValue;
            }
            greenMoney = perdaySave * greenTax;
            greenPrice.Text = greenMoney.ToString("#.##");
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(SaveReportPerDate, "");
            }
        }
    }
}
