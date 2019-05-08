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
using System.Windows.Shapes;
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
            MakeReport();
        }

        public void MakeReport()
        {
            //parent.ShowInfo();
            Solar_Panels itemInf = parent.SolarPanelListCB.SelectedItem as Solar_Panels;
            headerReport.Text= itemInf.NamePanel.ToString();
            double kWh = (double)itemInf.NominalPower_W / 1000;
            V.Text = kWh.ToString();
            PerDay.Text = parent.day.Text;
            priceOne.Text= itemInf.Price__.ToString();
            double paid = itemInf.Price__ * parent.PanelAmountSl.Value;
            priceSum.Text = paid.ToString();
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
        }



    }
}
