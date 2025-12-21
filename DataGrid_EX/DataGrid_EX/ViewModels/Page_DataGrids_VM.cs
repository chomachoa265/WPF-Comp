using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataGrid_EX.Extensions;
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


    public partial class Page_DataGrids_VM : ObservableObject
    {

        // 這是給 DataGrid 欄位定義用的集合
        [ObservableProperty]
        private ObservableCollection<ColumnConfig> _gridColumns = new();


        // 這是 DataGrid 的實際資料 (Rows)
        // 為了支援動態欄位，通常使用 Dictionary 或 ExpandoObject，這裡為求簡單用 dynamic
        [ObservableProperty]
        private ObservableCollection<RowItem> _gridData = new();


        [ObservableProperty]
        private RowItem? _selectedRow;

        public ObservableCollection<ReportData> Rows { get; set; } = new ObservableCollection<ReportData>();
        public ObservableCollection<ReportData> Rows2 { get; set; } = new ObservableCollection<ReportData>();
        public ObservableCollection<StatsData> StatRows { get; set; } = new ObservableCollection<StatsData>();
        public ObservableCollection<ColumnInfo> ColumnInfos { get; } = new ObservableCollection<ColumnInfo>();
        public Page_DataGrids_VM()
        {
            // 初始欄位
            GridColumns.Add(new ColumnConfig { Header = "ID", BindingPath = "Id", Width = 50 });
            GridColumns.Add(new ColumnConfig { Header = "Name", BindingPath = "Name", Width = 150 });

            // 初始資料
            GridData.Add(new { Id = 1, Name = "Linbaba", Extra = "Init" });
            GridData.Add(new { Id = 2, Name = "User", Extra = "Init" });
            //ColumnInfos = new ObservableCollection<ColumnInfo>
            //{
            //    new ColumnInfo(header : new CustomColumnHeader{ Title="123", SubTitle="456", Unit="°"},propName:"Age"),
            //    new ColumnInfo(header : new CustomColumnHeader{ Title="123", SubTitle="456", Unit="°"},propName:"Revenue"),
            //};

            //Rows.Add(new ReportData());
            //Rows.Add(new ReportData());
            //Rows.Add(new ReportData());
            //Rows.Add(new ReportData());

            //Rows2.Add(new ReportData
            //{
            //    Revenue = 1234.5f,
            //    Age = 32,
            //});

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
        public void AddRow()
        {
            // 新增一筆空白或預設資料
            var newRow = new RowItem
            {
                Name = $"Item {System.DateTime.Now.Second}",
                Description = "New Entry"
            };

            GridData.Add(newRow);

            // ★ 關鍵：重新編號
            RefreshRowIds();
        }

        [RelayCommand]
        public void RemoveRow()
        {
            // 如果有選中某列，刪除選中的；否則刪除最後一列
            var target = SelectedRow ?? GridData.LastOrDefault();

            if (target != null)
            {
                GridData.Remove(target);

                // ★ 關鍵：刪除後也要重新編號
                RefreshRowIds();
            }
        }

        /// <summary>
        /// 自動重新計算所有 Row 的 ID
        /// </summary>
        private void RefreshRowIds()
        {
            for (int i = 0; i < GridData.Count; i++)
            {
                // 因為 RowItem 繼承了 ObservableObject，
                // 這裡賦值會觸發 PropertyChanged，UI 的 "No." 欄位會瞬間更新
                GridData[i].Id = i + 1;
            }
        }

        [RelayCommand]
        public void AddColumn()
        {
            // 動態新增一個欄位
            string newColName = $"Col {GridColumns.Count + 1}";
            GridColumns.Add(new ColumnConfig
            {
                Header = newColName,
                BindingPath = "Name", // 這裡暫時重複綁 Name 作示範
                Width = 100
            });

            // 注意：因為兩個 DataGrid 都綁定到同一個 GridColumns 集合
            // 所以兩個 Grid 會同時新增欄位 -> 觸發 CollectionChanged -> 觸發 SyncBehavior 重綁
        }
        [RelayCommand]
        public void RemoveColumn()
        {
            if (GridColumns.Count > 0)
            {
                GridColumns.RemoveAt(GridColumns.Count - 1);
            }
        }
    }
}


    
