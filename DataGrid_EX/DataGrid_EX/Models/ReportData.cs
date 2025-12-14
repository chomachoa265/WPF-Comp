using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_EX.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ReportData
    {
        public int Age { get; set; } = 24;
        public double Revenue { get; set; } = 24000.5f;
        public int Familys { get; set; } = 5;
        public double Energy { get; set; } = 5.5;
        public double Force { get; set; } = 43;
        public double Mass { get; set; } = 12.5;
        public double Radius { get; set; } = 5.3;

    }
    [AddINotifyPropertyChangedInterface]
    public class StatsData
    {

        public StatsData(string name)
        {
            Name = name;
        }
        public string Name { get; set; } = "";
        public Dictionary<string, double> ValuesByColumn { get; } = new Dictionary<string, double>();

        public void SetStat(string columnName, double? value)
        {

            if(value.HasValue)
            {
                ValuesByColumn[columnName] = value.Value;
            }
            else
            {
                ValuesByColumn[columnName] = 0;
            }
        }
    }
}
