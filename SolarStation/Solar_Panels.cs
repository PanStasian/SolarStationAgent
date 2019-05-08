//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SolarStation
{
    using System;
    using System.Collections.Generic;
    
    public partial class Solar_Panels
    {
        public int Id_panel { get; set; }
        public string NamePanel { get; set; }
        public Nullable<double> NominalPower_W { get; set; }
        public double RatedVoltage_V { get; set; }
        public double RatedCurrent_A { get; set; }
        public double OpenCircuitVoltage_V { get; set; }
        public double MaxSystemVoltage_V { get; set; }
        public double PanelEfficiency { get; set; }
        public string SolarCells { get; set; }
        public string Manufacturer { get; set; }
        public double Price__ { get; set; }
        public Nullable<int> MinTemperature { get; set; }
        public Nullable<int> MaxTemperature { get; set; }

        public double CalculatePower(int Insolation, int panelAmount)
        {
            double Ko= 1.11;
            double Klost = 0.8;
            double E = (double)(panelAmount * Insolation * (NominalPower_W / 1000) * Ko * Klost) / 24;
            return E;
        }

        
    }
}
