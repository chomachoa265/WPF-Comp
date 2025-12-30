using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataGrid_EX.Extensions;
using DataGrid_EX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataGrid_EX.ViewModels
{


    public partial class Page_DataGrids_VM : ObservableObject
    {

        // 綁定給 DataGrid 的欄位設定集合
        [ObservableProperty]
        private ObservableCollection<ColumnConfig> _gridColumns = new();


        [ObservableProperty]
        private ObservableCollection<RowItem> _gridData = new();

        // ★ 核心：用來存儲「目前被點選」的那個欄位設定
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveSelectedColumnCommand))] // 當選取變更時，檢查刪除按鈕能不能按
        private ColumnConfig? _selectedColumnConfig;

        // 顯示目前選了誰 (給 UI 顯示用)
        [ObservableProperty]
        private string _statusMessage = "請點選任一欄位標頭...";

        // --- 顯示選取結果 (共用) ---
        [ObservableProperty]
        private ObservableCollection<string> _selectedColumnValues = new();

        [ObservableProperty]
        private string _selectedColumnTitle = "尚未選取";


        [ObservableProperty]
        private RowItem? _selectedRow;

        public Page_DataGrids_VM()
        {
            AddColumn("ID", "Id");
            AddColumn("Name", "Name");
            AddColumn("Score", "Score");
            AddRow(); // 會觸發 CollectionChanged -> 觸發計算
            AddRow();
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


        // 輔助方法：建立欄位並綁定點擊事件
        private void AddColumn(string header, string bindingPath)
        {
            GridColumns.Add(new ColumnConfig
            {
                Header = header,
                BindingPath = bindingPath,
                Width = 100,
                // ★ 這裡把每個欄位的 Command 都綁定到同一個方法
                Command = ColumnHeaderClickCommand
            });
        }

        // --- Command 1: 點選欄位標頭 ---
        [RelayCommand]
        private void ColumnHeaderClick(ColumnConfig col)
        {
            // 1. 紀錄被點選的欄位實例
            SelectedColumnConfig = col;

            // 2. 更新狀態訊息
            StatusMessage = $"已選取欄位: {col.Header}";
        }

        // --- Command 2: 刪除選取欄位 ---
        // CanExecute: 只有當 SelectedColumnConfig 不為空時，按鈕才給按
        [RelayCommand(CanExecute = nameof(CanRemoveColumn))]
        private void RemoveSelectedColumn()
        {
            if (SelectedColumnConfig != null && GridColumns.Contains(SelectedColumnConfig))
            {
                string removedName = SelectedColumnConfig.Header;

                foreach (var column in GridColumns)
                {
                }
                // ★ 這一行執行後，DataGridExtensions 會監聽到變動，自動把畫面上的 Column 刪掉
                //GridColumns.Remove(SelectedColumnConfig);

                // 清空選取狀態
                //SelectedColumnConfig = null;
                StatusMessage = $"已刪除欄位: {removedName}";
            }
        }

        [RelayCommand]
        private void AddCol()
        {
            AddColumn("Score", "Score");
        }

        private bool CanRemoveColumn() => SelectedColumnConfig != null;
    }
}


    
