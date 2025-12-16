using CommunityToolkit.Mvvm.Input;
using DataGrid_EX.Interfaces;
using DataGrid_EX.Models;
using DataGrid_EX.Views;
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
    public partial class MainWindow_VM
    {
        private readonly INavigationService _navigation;
        public ObservableCollection<ReportData> Rows { get; set; } = new ObservableCollection<ReportData>();
        public ObservableCollection<StatsData> StatRows { get; set; } = new ObservableCollection<StatsData>();
        public ObservableCollection<ColumnInfo> ColumnInfos { get; } = new ObservableCollection<ColumnInfo>();
        public MainWindow_VM()
        {

        }
        public MainWindow_VM(INavigationService navigationService)
        {
            _navigation = navigationService;
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());
            Rows.Add(new ReportData());

            Debug.WriteLine(Application.Current.Resources.Count);
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

                    if(raw is double d)
                    {
                        values.Add(d);
                    }
                }

                double max = values.Max();
                double min = values.Min();

                
            }
        }

        [RelayCommand]
        private void OnGoDialogPage()
        {
            _navigation.Navigate<Page_Dialogs>();
        }
        [RelayCommand]
        private void OnGoComboBoxPage()
        {
            _navigation.Navigate<Page_ComboBoxs>();
        }
        [RelayCommand]
        private void OnGoDataGridPage()
        {
            _navigation.Navigate<Page_DataGrids>();
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
