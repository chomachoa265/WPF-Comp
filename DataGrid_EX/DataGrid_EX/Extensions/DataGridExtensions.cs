using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DataGrid_EX.Extensions
{
    // 簡單的欄位定義模型 (放在這裡方便閱讀，實務上可放在 Models 資料夾)
    public class ColumnConfig
    {
        public string Header { get; set; } = "";
        public string BindingPath { get; set; } = "";
        public double Width { get; set; } = 100;
    }
    public static class DataGridExtensions
    {
        public static readonly DependencyProperty BindableColumnsProperty =
            DependencyProperty.RegisterAttached(
                "BindableColumns",
                typeof(ObservableCollection<ColumnConfig>),
                typeof(DataGridExtensions),
                new PropertyMetadata(null, OnBindableColumnsChanged));

        public static ObservableCollection<ColumnConfig> GetBindableColumns(DependencyObject obj)
        {
            return (ObservableCollection<ColumnConfig>)obj.GetValue(BindableColumnsProperty);
        }

        public static void SetBindableColumns(DependencyObject obj, ObservableCollection<ColumnConfig> value)
        {
            obj.SetValue(BindableColumnsProperty, value);
        }
        private static void OnBindableColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid grid) return;

            // 處理舊集合 (移除監聽)
            if (e.OldValue is ObservableCollection<ColumnConfig> oldColl)
            {
                oldColl.CollectionChanged -= (s, args) => HandleCollectionChanged(grid, args);
            }

            // 處理新集合 (初始載入 + 加入監聽)
            if (e.NewValue is ObservableCollection<ColumnConfig> newColl)
            {
                grid.Columns.Clear();
                // 初始載入所有欄位
                foreach (var colConfig in newColl)
                {
                    grid.Columns.Add(CreateColumn(colConfig));
                }

                // 監聽後續變動
                newColl.CollectionChanged += (s, args) => HandleCollectionChanged(grid, args);
            }
        }
        private static void HandleCollectionChanged(DataGrid grid, NotifyCollectionChangedEventArgs e)
        {
            // 處理新增
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (ColumnConfig item in e.NewItems)
                {
                    grid.Columns.Add(CreateColumn(item));
                }
            }
            // 處理移除
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (ColumnConfig item in e.OldItems)
                {
                    // 這裡簡單假設 Header 唯一，實務上可用更嚴謹的對應
                    var colToRemove = grid.Columns.FirstOrDefault(c => (string)c.Header == item.Header);
                    if (colToRemove != null)
                    {
                        grid.Columns.Remove(colToRemove);
                    }
                }
            }
            // 處理重置 (Clear)
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                grid.Columns.Clear();
            }
        }

        private static DataGridColumn CreateColumn(ColumnConfig config)
        {
            // 這裡建立 DataGridTextColumn，如果需要 TemplateColumn 可在此擴充
            var col = new DataGridTextColumn
            {
                Header = config.Header,
                Width = new DataGridLength(config.Width),
                // 這裡的 Binding 是綁定到 RowData 的屬性名稱 (例如 Dictionary["Key"] 或 Property)
                Binding = new Binding(config.BindingPath),
                MinWidth = 50
            };
            return col;
        }

    }
}
