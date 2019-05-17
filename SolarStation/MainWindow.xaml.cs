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
using System.Data.Entity;

namespace SolarStation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double sumCompare = 0;
        public int panelAmount;
        public Solar_Panels SolarPanelSelected;
        public Solar_Panels SolarPanelSelectedInf;
        public Solar_Panels SolarPanelSelectedCompareItem;
        public SolarPanelEntities sp = new SolarPanelEntities();
        public List<Solar_Panels> SolPal { get; set; }
        public List<Solar_Panels> SolPalInf { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            FillAbout();
            DatePick.SelectedDate = DateTime.Now;
            startDate.SelectedDate = DateTime.Now;
            BindCB();
            BindCBInfPanel();
            SolarPanelListCB.ItemsSource = sp.Solar_Panels.ToList();
            PanelAmountSl.ValueChanged += Slider_ValueChanged;
            PanelAmountSl.Value = 10;
            isTrackSun.Checked += CheckBox_Checked;
            isTrackSun.Unchecked += CheckBox_Unchecked;
        }
        
        #region Main
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

            compareThis.Text = SolarPanelSelected.NamePanel.ToString();
            PowerComparable.Text = kWh.ToString();
            PriceComparable.Text = SolarPanelSelected.Price__.ToString();
            EffComparable.Text = SolarPanelSelected.PanelEfficiency.ToString();
            maxPowComparable.Text = SolarPanelSelected.MaxSystemVoltage_V.ToString();
            CrystallComparable.Text = SolarPanelSelected.SolarCells.ToString();
            manufacturerComparable.Text = SolarPanelSelected.Manufacturer.ToString();
        }



        public void FillChart()
        {
            Style styleLegend = new Style { TargetType = typeof(Control) };
            styleLegend.Setters.Add(new Setter(Control.WidthProperty, 0d));
            styleLegend.Setters.Add(new Setter(Control.HeightProperty, 0d));
            Chart.LegendStyle = styleLegend;

            List<KeyValuePair<string, double>> KeyValue = new List<KeyValuePair<string, double>>();
            DateTime date = DatePick.SelectedDate.Value;
            double perDayPower = 0;

            if (date.Year != 2019)
            {
                int years = 2019 - date.Year;
                date = date.AddYears(years);
            }
            if (isTrackSun.IsChecked == true)
            {
                foreach (var time in sp.SolarInsalations.Where(x => x.Date == date).ToList())
                {
                    double power = SolarPanelSelected.CalculatePower((int)time.ETRN, panelAmount);
                    perDayPower += power;
                    KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
                }
                ((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
            }
            else
            {
                foreach (var time in sp.SolarInsalations.Where(x => x.Date == date).ToList())
                {
                    double power = SolarPanelSelected.CalculatePower((int)time.ETR, panelAmount);
                    perDayPower += power;
                    KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
                }
                ((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
            }
            day.Text = perDayPower.ToString("#.##");


        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
            this.panelAmount = (int)PanelAmountSl.Value;
            FillChart();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            FillChart();
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            FillChart();
        }


        private void OpenReport_Click(object sender, RoutedEventArgs e)
        {
            Report reportWindow = new Report();
            reportWindow.Show();
        }
        #endregion

        #region Information Tab
        private void BindCBInfPanel()
        {
            var itemInf = sp.Solar_Panels.ToList();
            SolPalInf = itemInf;
            DataContext = SolPalInf;
        }

        public void ComboBox_SelectionInfChanged(object sender, SelectionChangedEventArgs e)
        {
            SolarPanelSelectedInf = SolarPanelListInfCB.SelectedItem as Solar_Panels;
            double kWh = (double)SolarPanelSelected.NominalPower_W / 1000;
            ShowInfo();
            //V.Text = kWh.ToString();
            //FillChart();
        }

        public void ShowInfo()
        {
            Solar_Panels itemInf = SolarPanelListInfCB.SelectedItem as Solar_Panels;
            //PanelNameTxt.Text = item.NamePanel.ToString();

            Header.Content = itemInf.NamePanel.ToString();

            double kWh = (double)itemInf.NominalPower_W / 1000;
            NominalPowTxt.Text = kWh.ToString();

            RatedVoltageTxt.Text = itemInf.RatedVoltage_V.ToString();

            RatedCurrentTxt.Text = itemInf.RatedCurrent_A.ToString();

            OpenCircuitVoltageTxt.Text = itemInf.OpenCircuitVoltage_V.ToString();

            MaxSystemVoltageTxt.Text = itemInf.MaxSystemVoltage_V.ToString();

            PanelEfficiencyTxt.Text = itemInf.PanelEfficiency.ToString();

            SolarCellsTxt.Text = itemInf.SolarCells.ToString();

            ManufacturerTxt.Text = itemInf.Manufacturer.ToString();

            PriceTxt.Text = itemInf.Price__.ToString();

            MinTempTxt.Text = itemInf.MinTemperature.ToString();

            MaxTempTxt.Text = itemInf.MaxTemperature.ToString();
        }
        #endregion




        private void CalculateBtn_Click(object sender, RoutedEventArgs e)
        {
            FillChart();
        }

        #region Compare
        private void SolarPanelCompareList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SolarPanelSelectedCompareItem = SolarPanelCompareList.SelectedItem as Solar_Panels;
            //compareTo.Text = SolarPanelSelectedCompareItem.NamePanel.ToString();
            //double kWh = (double)SolarPanelSelectedCompareItem.NominalPower_W / 1000;
            //PowerComparaTo.Text = kWh.ToString();
            //priceCompareTo.Text = SolarPanelSelectedCompareItem.Price__.ToString();

        }

        public void Compare()
        {
            if (SolarPanelSelected.NominalPower_W.Value > SolarPanelSelectedCompareItem.NominalPower_W.Value)
            {
                NomPowComparable.Background = Brushes.Green;
                NomPowCompareTo.Background = Brushes.DarkRed;
            }
            else if (SolarPanelSelected.NominalPower_W.Value < SolarPanelSelectedCompareItem.NominalPower_W.Value)
            {
                NomPowComparable.Background = Brushes.DarkRed;
                NomPowCompareTo.Background = Brushes.Green;
            }

            if (SolarPanelSelected.Price__ < SolarPanelSelectedCompareItem.Price__)
            {
                PriceListComparable.Background = Brushes.Green;
                PriceListCompareTo.Background = Brushes.DarkRed;
            }
            else if (SolarPanelSelected.NominalPower_W.Value > SolarPanelSelectedCompareItem.NominalPower_W.Value)
            {
                PriceListComparable.Background = Brushes.DarkRed;
                PriceListCompareTo.Background = Brushes.Green;
            }

            if (SolarPanelSelected.PanelEfficiency > SolarPanelSelectedCompareItem.PanelEfficiency)
            {
                EfficientyListComparable.Background = Brushes.Green;
                EfficientyListCompareTo.Background = Brushes.DarkRed;
            }
            else if (SolarPanelSelected.PanelEfficiency < SolarPanelSelectedCompareItem.PanelEfficiency)
            {
                EfficientyListComparable.Background = Brushes.DarkRed;
                EfficientyListCompareTo.Background = Brushes.Green;
            }

            if (SolarPanelSelected.MaxSystemVoltage_V > SolarPanelSelectedCompareItem.MaxSystemVoltage_V)
            {
                maxPowerListComparable.Background = Brushes.Green;
                maxPowerListCompareTo.Background = Brushes.DarkRed;
            }
            else if (SolarPanelSelected.MaxSystemVoltage_V < SolarPanelSelectedCompareItem.MaxSystemVoltage_V)
            {
                maxPowerListComparable.Background = Brushes.DarkRed;
                maxPowerListCompareTo.Background = Brushes.Green;
            }


        }





        public void CalculateForCompare()
        {
            List<KeyValuePair<string, double>> KeyValue = new List<KeyValuePair<string, double>>();
            DateTime date = DatePick.SelectedDate.Value;


            if (date.Year != 2019)
            {
                int years = 2019 - date.Year;
                date = date.AddYears(years);
            }
            if (isTrackSun.IsChecked == true)
            {
                foreach (var time in sp.SolarInsalations.Where(x => x.Date == date).ToList())
                {
                    double power = SolarPanelSelectedCompareItem.CalculatePower((int)time.ETRN, panelAmount);
                    sumCompare += power;
                    //KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
                }
                //((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
            }
            else
            {
                foreach (var time in sp.SolarInsalations.Where(x => x.Date == date).ToList())
                {
                    double power = SolarPanelSelectedCompareItem.CalculatePower((int)time.ETR, panelAmount);
                    sumCompare += power;
                    //KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
                }
                //((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
            }
        }

        private void CompareBtn_Click(object sender, RoutedEventArgs e)
        {
            SolarPanelSelectedCompareItem = SolarPanelCompareList.SelectedItem as Solar_Panels;
            compareTo.Text = SolarPanelSelectedCompareItem.NamePanel.ToString();
            double kWh = (double)SolarPanelSelectedCompareItem.NominalPower_W / 1000;
            PowerComparaTo.Text = kWh.ToString();
            priceCompareTo.Text = SolarPanelSelectedCompareItem.Price__.ToString();
            EffCompareTo.Text = SolarPanelSelectedCompareItem.PanelEfficiency.ToString();
            maxPowCompareTo.Text = SolarPanelSelectedCompareItem.MaxSystemVoltage_V.ToString();
            CrystallCompareTo.Text = SolarPanelSelectedCompareItem.SolarCells.ToString();
            manufacturerCompareTo.Text = SolarPanelSelectedCompareItem.Manufacturer.ToString();

            CalculateForCompare();
            Compare();
        }
        #endregion


        #region Predict and statistic

        public void FillChartForDate()
        {
            Style styleLegend = new Style { TargetType = typeof(Control) };
            styleLegend.Setters.Add(new Setter(Control.WidthProperty, 0d));
            styleLegend.Setters.Add(new Setter(Control.HeightProperty, 0d));
            StatisticChart.LegendStyle = styleLegend;

            List<KeyValuePair<string, double>> KeyValue = new List<KeyValuePair<string, double>>();
            DateTime startdate = startDate.SelectedDate.Value;
            DateTime enddate = endDate.SelectedDate.Value;
            double perDayPower = 0;
            double perdaySave = 0;

            if (startdate >= enddate)
            {
                MessageBox.Show("Дата початку не може перевищувати або співпадати з кінцевою датою.");
            }

            

            if (startdate.Year != 2019)
            {
                int years = 2019 - startdate.Year;
                startdate = startdate.AddYears(years);
            }
            perDayPower = 0;
            if (isTrackSun.IsChecked == true)
            {
                for (DateTime current = startdate; current <= enddate; current = current.AddDays(1))
                {
                    foreach (var time in sp.SolarInsalations.Where(x => x.Date == current))
                    {
                        double power = SolarPanelSelected.CalculatePower((int)time.ETRN, panelAmount);
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
                    foreach (var time in sp.SolarInsalations.Where(x => x.Date == current))
                    {
                        double power = SolarPanelSelected.CalculatePower((int)time.ETR, panelAmount);
                        perDayPower += power;
                        perdaySave += power;
                    }
                    KeyValue.Add(new KeyValuePair<string, double>(current.ToShortDateString(), perDayPower));
                    perDayPower = 0;
                }
                ((LineSeries)StatisticChart.Series[0]).ItemsSource = KeyValue;
            }
            energySum.Text = perdaySave.ToString("#.##");
        }
        private void SetPeriodCalc_Click(object sender, RoutedEventArgs e)
        {
            if (endDate.SelectedDate == null || endDate.SelectedDate==startDate.SelectedDate)
            {
                MessageBox.Show("Виберіть кінцеву дату, відмінну від дати початку.");
            }
            else
            {
                FillChartForDate();
                ResultTXT.Visibility = Visibility.Visible;
            }
            
        }


        private void ReportOpen_Click(object sender, RoutedEventArgs e)
        {
            if (endDate.SelectedDate == null)
            {
                MessageBox.Show("Виберіть кінцеву дату");
            }
            else
            {
                DateReportxaml dateReport = new DateReportxaml();
                dateReport.Show();
            }
        }

        #endregion




        #region Tab Navigation
        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);
            switch (index)
            {
                case 0:
                    infGrid.Visibility = Visibility.Collapsed;
                    infGrid.IsEnabled = false;
                    compareGrid.Visibility = Visibility.Collapsed;
                    compareGrid.IsEnabled = false;
                    predictGrid.Visibility = Visibility.Collapsed;
                    predictGrid.IsEnabled = false;

                    mainGrid.Visibility = Visibility.Visible;
                    mainGrid.IsEnabled = true;
                    break;
                case 1:
                    mainGrid.Visibility = Visibility.Collapsed;
                    mainGrid.IsEnabled = false;
                    compareGrid.Visibility = Visibility.Collapsed;
                    compareGrid.IsEnabled = false;
                    predictGrid.Visibility = Visibility.Collapsed;
                    predictGrid.IsEnabled = false;

                    infGrid.Visibility = Visibility.Visible;
                    infGrid.IsEnabled = true;
                    break;
                case 2:
                    mainGrid.Visibility = Visibility.Collapsed;
                    mainGrid.IsEnabled = false;
                    infGrid.Visibility = Visibility.Collapsed;
                    infGrid.IsEnabled = false;
                    predictGrid.Visibility = Visibility.Collapsed;
                    predictGrid.IsEnabled = false;

                    compareGrid.Visibility = Visibility.Visible;
                    compareGrid.IsEnabled = true;
                    break;
                case 3:
                    mainGrid.Visibility = Visibility.Collapsed;
                    mainGrid.IsEnabled = false;
                    infGrid.Visibility = Visibility.Collapsed;
                    infGrid.IsEnabled = false;
                    compareGrid.Visibility = Visibility.Collapsed;
                    compareGrid.IsEnabled = false;

                    predictGrid.Visibility = Visibility.Visible;
                    predictGrid.IsEnabled = true;
                    break;

            }
        }
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }





        #endregion

        private void SaveGraph_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(Chart, "");
            }
        }

        private void FillAbout()
        {
            About.Text = "   Я програмний продукт, розроблений для швидкого " +
                "і зручного моделювання роботи сонячної електричної станції." +
                " Для коректної роботи мені потрібно знати:" +
                " рівень сонячної інсоляціїї зо поточний час, налаштування Вашої сонячної станції " +
                "(назва сонячної панелі, їх кількість, наявність встановленого сонячного трекера)." +
                " Я буду моделювати графіки генерації електричної енергії та надам повний звіт з роботи системи." +
                " Я допоможу вибрати найефективнішу систему, та на основі цих даних зможу розробити статистику генерації енергії за вибраний Вами період часу.";
        }

        private void SaveComparisonBtn_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(SaveCompare, "");
            }
        }

        
    }
}
