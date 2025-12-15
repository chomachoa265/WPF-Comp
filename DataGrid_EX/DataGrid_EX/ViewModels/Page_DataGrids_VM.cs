using DataGrid_EX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_EX.ViewModels
{


    public class Page_DataGrids_VM
    {
        public ObservableCollection<ReportData> Rows { get; set; } = new ObservableCollection<ReportData>();
        public ObservableCollection<StatsData> StatRows { get; set; } = new ObservableCollection<StatsData>();
        public ObservableCollection<ColumnInfo> ColumnInfos { get; } = new ObservableCollection<ColumnInfo>();
        public Page_DataGrids_VM()
        {
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());


            StatRows.Add(new StatsData("Mean"));
            RecalculateStats();
        }
        private void RecalculateStats()
        {
            Type rowType = typeof(ReportData);

            foreach (var colInfo in ColumnInfos)
            {
                string propName = colInfo.Header.Title;
                PropertyInfo? prop = rowType.GetProperty(propName);
                if (prop == null) { continue; }
                List<double> values = new();
                foreach (var row in Rows)
                {
                    object? raw = prop.GetValue(row);

                    if (raw is double d)
                    {
                        values.Add(d);
                    }
                }

                double max = values.Max();
                double min = values.Min();


            }
        }
        private void OnAddColumn()
        {
            var propName = "Revenue";
            var newHeader = new CustomColumnHeader { Title = "Revenue", Unit = "NT" };
            ColumnInfos.Add(new ColumnInfo(newHeader, nameof(ReportData.Revenue)));

        }
    }
}


    
