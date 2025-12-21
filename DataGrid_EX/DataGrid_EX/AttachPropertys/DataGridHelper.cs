using DataGrid_EX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DataGrid_EX.AttachPropertys
{
public static class DataGridHelper
    {
        public static readonly DependencyProperty DynamicColumnsProperty =
            DependencyProperty.RegisterAttached(
                "DynamicColumns",
                typeof(ObservableCollection<ColumnInfo>),
                typeof(DataGridHelper),
                new PropertyMetadata(null, OnDynamicColumnsChanged));

        public static void SetDynamicColumns(DependencyObject element, ObservableCollection<ColumnInfo> value)
        {
            element.SetValue(DynamicColumnsProperty, value);
        }

        public static ObservableCollection<ColumnInfo> GetDynamicColumns(DependencyObject element)
        {
            return (ObservableCollection<ColumnInfo>)element.GetValue(DynamicColumnsProperty);
        }

        private static void OnDynamicColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGrid;
            if (dataGrid == null) return;

            // 1. 如果有舊的集合，先解除訂閱 (避免記憶體洩漏)
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= (s, arg) => RegenerateColumns(dataGrid, s as IEnumerable<ColumnInfo>);
            }

            // 2. 如果是新的集合，進行訂閱並初次生成
            if (e.NewValue is ObservableCollection<ColumnInfo> newCollection)
            {
                // 當集合變動 (Add/Remove) 時，重新生成欄位
                newCollection.CollectionChanged += (s, arg) => RegenerateColumns(dataGrid, newCollection);

                // 初次執行一次
                RegenerateColumns(dataGrid, newCollection);
            }
        }

        // 這是實際產生欄位的邏輯 (包含你之前的 HeaderTemplate 處理)
        private static void RegenerateColumns(DataGrid dataGrid, IEnumerable<ColumnInfo> infos)
        {
            if (infos == null) return;

            dataGrid.Columns.Clear();

            // 抓取你在 XAML 定義好的 HeaderTemplate
            var headerTemplate = Application.Current.TryFindResource("DynamicHeaderTemplate") as DataTemplate;
            foreach (var info in infos)
            {
                var path = info.DataBindingPath;
                var column = new DataGridTextColumn
                {
                    Header = info.Header,
                    Binding = new Binding(path),
                    HeaderTemplate = headerTemplate // 套用樣式
                };

                dataGrid.Columns.Add(column);
            }
        }
    }
}
