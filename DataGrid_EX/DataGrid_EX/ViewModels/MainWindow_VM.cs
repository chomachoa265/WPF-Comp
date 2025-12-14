using CommunityToolkit.Mvvm.Input;
using DataGrid_EX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;

namespace DataGrid_EX.ViewModels
{
    public partial class AdornerExWindow_VM
    {
        public ObservableCollection<ReportData> Rows { get; set; } = new ObservableCollection<ReportData>();
        public ObservableCollection<StatsData> StatRows { get; set; } = new ObservableCollection<StatsData>();
        public ObservableCollection<ColumnInfo> ColumnInfos { get; } = new ObservableCollection<ColumnInfo>();
        public AdornerExWindow_VM()
        {
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());


            StatRows.Add(new StatsData("Mean"));
            //StatRows.Add(new StatsData { Name = "Max" });
            //StatRows.Add(new StatsData { Name = "Min" });
            //StatRows.Add(new StatsData { Name = "Stdev" });
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

                    if(raw is double d)
                    {
                        values.Add(d);
                    }
                }

                //if(values.Count == 0)
                //{

                //}
                double mean = values.Average();
                double max = values.Max();
                double min = values.Min();
                StatRows[0].SetStat(propName, mean);

                
            }
        }

        [RelayCommand]
        private void OnColumnHeaderClick(object param)
        {
            Debug.WriteLine(param);
        }
        [RelayCommand]
        private void OnAddColumn()
        {
            var propName = "Revenue";
            var newHeader = new CustomColumnHeader { Title = "Revenue", Unit = "NT" };
            ColumnInfos.Add(new ColumnInfo(newHeader, nameof(ReportData.Revenue)));

        }
    }
}
