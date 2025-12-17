using CommunityToolkit.Mvvm.Input;
using DataGrid_EX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_EX.ViewModels
{


    public partial class Page_DataGrids_VM
    {
        public ObservableCollection<ReportData> Rows { get; set; } = new ObservableCollection<ReportData>();
        public ObservableCollection<ReportData> Rows2 { get; set; } = new ObservableCollection<ReportData>();
        public ObservableCollection<StatsData> StatRows { get; set; } = new ObservableCollection<StatsData>();
        public ObservableCollection<ColumnInfo> ColumnInfos { get; } = new ObservableCollection<ColumnInfo>();
        public Page_DataGrids_VM()
        {
            ColumnInfos = new ObservableCollection<ColumnInfo>
            {
                new ColumnInfo(header : new CustomColumnHeader{ Title="123", SubTitle="456", Unit="°"},propName:"Age"),
                new ColumnInfo(header : new CustomColumnHeader{ Title="123", SubTitle="456", Unit="°"},propName:"Revenue"),
            };
            
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());

            Rows2.Add(new ReportData
            {
                Revenue = 1234.5f,
                Age = 32,
            });

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
        [RelayCommand]
        private void OnAddColumn()
        {
            var newHeader = new CustomColumnHeader { Title = "Revenue", Unit = "NT" };
            ColumnInfos.Add(new ColumnInfo(newHeader, nameof(ReportData.Revenue)));
        }
        [RelayCommand]
        private void OnDeleteColumn()
        {
            if (ColumnInfos.Any())
            {
                ColumnInfos.RemoveAt(ColumnInfos.Count - 1);
            }
        }
    }
}


    
