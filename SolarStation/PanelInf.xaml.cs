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
    public partial class PanelInf : Window
    {
        MainWindow parent;
        public PanelInf()
        {
            InitializeComponent();
            parent = (MainWindow)Application.Current.MainWindow;
            ShowInfo();
        }

        public void ShowInfo()
        {
            var item = parent.SolarPanelListCB.SelectedItem as Solar_Panels;
            //PanelNameTxt.Text = item.NamePanel.ToString();

            Header.Content= item.NamePanel.ToString();
            
            NominalPowTxt.Text = item.NominalPower_W.ToString();

            RatedVoltageTxt.Text = item.RatedVoltage_V.ToString();

            RatedCurrentTxt.Text = item.RatedCurrent_A.ToString();

            OpenCircuitVoltageTxt.Text = item.OpenCircuitVoltage_V.ToString();

            MaxSystemVoltageTxt.Text = item.MaxSystemVoltage_V.ToString();

            PanelEfficiencyTxt.Text = item.PanelEfficiency.ToString();

            SolarCellsTxt.Text = item.SolarCells.ToString();

            ManufacturerTxt.Text = item.Manufacturer.ToString();

            PriceTxt.Text = item.Price__.ToString();

            MinTempTxt.Text = item.MinTemperature.ToString();

            MaxTempTxt.Text = item.MaxTemperature.ToString();
        }
    }
}
