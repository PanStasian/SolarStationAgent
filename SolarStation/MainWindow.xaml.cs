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
        Solar_Panels SolarPanelSelectedInf;
        SolarPanelEntities sp = new SolarPanelEntities();
        public List<Solar_Panels> SolPal { get; set; }
        public List<Solar_Panels> SolPalInf { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DatePick.SelectedDate = DateTime.Now;
            BindCB();
            BindCBInfPanel();
            SolarPanelListCB.ItemsSource = sp.Solar_Panels.ToList();
            PanelAmountSl.ValueChanged += Slider_ValueChanged;
            PanelAmountSl.Value = 10;
            isTrackSun.Checked += CheckBox_Checked;
            isTrackSun.Unchecked += CheckBox_Unchecked;
            //FillChart();
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
        }



        public void FillChart()
        {
            Style styleLegend = new Style { TargetType = typeof(Control) };
            styleLegend.Setters.Add(new Setter(Control.WidthProperty, 0d));
            styleLegend.Setters.Add(new Setter(Control.HeightProperty, 0d));
            Chart.LegendStyle = styleLegend;

            List<KeyValuePair<string, double>> KeyValue = new List<KeyValuePair<string, double>>();
            DateTime date = DatePick.SelectedDate.Value;
            double sum = 0;


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
                    sum += power;
                    KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
                }
                ((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
            }
            else
            {
                foreach (var time in sp.SolarInsalations.Where(x => x.Date == date).ToList())
                {
                    double power = SolarPanelSelected.CalculatePower((int)time.ETR, panelAmount);
                    sum += power;
                    KeyValue.Add(new KeyValuePair<string, double>(time.Time, power));
                }
                ((LineSeries)Chart.Series[0]).ItemsSource = KeyValue;
            }
            day.Text = sum.ToString("#.##");

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

       
    }
}
