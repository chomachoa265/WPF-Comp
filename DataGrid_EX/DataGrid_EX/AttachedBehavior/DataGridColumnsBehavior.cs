using DataGrid_EX.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DataGrid_EX.AttachedBehavior
{
    public static class DataGridColumnsBehavior
    {
        // 給上面的 DataGrid（DataRow）
        public static readonly DependencyProperty DataColumnInfosProperty =
            DependencyProperty.RegisterAttached(
                "DataColumnInfos",
                typeof(IEnumerable),
                typeof(DataGridColumnsBehavior),
                new PropertyMetadata(null, OnDataColumnInfosChanged));

        public static void SetDataColumnInfos(DependencyObject element, IEnumerable value)
            => element.SetValue(DataColumnInfosProperty, value);

        public static IEnumerable GetDataColumnInfos(DependencyObject element)
            => (IEnumerable)element.GetValue(DataColumnInfosProperty);

        // 給下面的 DataGrid（StatRow）
        public static readonly DependencyProperty StatColumnInfosProperty =
            DependencyProperty.RegisterAttached(
                "StatColumnInfos",
                typeof(IEnumerable),
                typeof(DataGridColumnsBehavior),
                new PropertyMetadata(null, OnStatColumnInfosChanged));

        public static void SetStatColumnInfos(DependencyObject element, IEnumerable value)
            => element.SetValue(StatColumnInfosProperty, value);

        public static IEnumerable GetStatColumnInfos(DependencyObject element)
            => (IEnumerable)element.GetValue(StatColumnInfosProperty);


        private static void OnDataColumnInfosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid grid) return;

            if (e.NewValue is IEnumerable newCols)
            {
                RebuildColumns(grid, newCols, useStatBinding: false);

                if (e.NewValue is INotifyCollectionChanged ncc)
                {
                    ncc.CollectionChanged += (_, __) =>
                        RebuildColumns(grid, newCols, useStatBinding: false);
                }
            }
        }

        private static void OnStatColumnInfosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid grid) return;

            if (e.NewValue is IEnumerable newCols)
            {
                RebuildColumns(grid, newCols, useStatBinding: true);

                if (e.NewValue is INotifyCollectionChanged ncc)
                {
                    ncc.CollectionChanged += (_, __) =>
                        RebuildColumns(grid, newCols, useStatBinding: true);
                }
            }
        }

        private static void RebuildColumns(DataGrid grid, IEnumerable columnInfos, bool useStatBinding)
        {
            grid.Columns.Clear();

            foreach (var obj in columnInfos)
            {
                if (obj is not ColumnInfo info)
                    continue;

                string path = useStatBinding ? info.StatBindingPath : info.DataBindingPath;
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                var col = new DataGridTextColumn
                {
                    Header = info.Header,
                    Binding = new Binding(path) { StringFormat = "0.#####" }
                };

                grid.Columns.Add(col);
            }
        }
    }
}
