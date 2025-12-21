using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DataGrid_EX.AttachPropertys // 請替換為你的 Namespace
{
        public static class DataGridSharedSizeHelper
        {
            // 優化重點：使用 WeakReference 避免記憶體洩漏
            private static readonly Dictionary<string, List<WeakReference<DataGridColumn>>> SharedColumns = new();

            // 避免遞迴更新的鎖
            private static bool _isUpdating = false;

            #region SharedSizeGroup Attached Property

            public static readonly DependencyProperty SharedSizeGroupProperty =
                DependencyProperty.RegisterAttached(
                    "SharedSizeGroup",
                    typeof(string),
                    typeof(DataGridSharedSizeHelper),
                    new PropertyMetadata(null, OnSharedSizeGroupChanged));

            public static string GetSharedSizeGroup(DependencyObject obj)
            {
                return (string)obj.GetValue(SharedSizeGroupProperty);
            }

            public static void SetSharedSizeGroup(DependencyObject obj, string value)
            {
                obj.SetValue(SharedSizeGroupProperty, value);
            }

            #endregion

            private static void OnSharedSizeGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                if (d is not DataGridColumn column) return;

                string oldGroup = e.OldValue as string;
                string newGroup = e.NewValue as string;

            Debug.WriteLine("OnSharedSizeGroupChagned");
                if (!string.IsNullOrEmpty(oldGroup))
                {
                    UnregisterColumn(column, oldGroup);
                }

                if (!string.IsNullOrEmpty(newGroup))
                {
                    RegisterColumn(column, newGroup);
                }
            }

            private static void RegisterColumn(DataGridColumn column, string groupName)
            {
                if (!SharedColumns.ContainsKey(groupName))
                {
                    SharedColumns[groupName] = new List<WeakReference<DataGridColumn>>();
                }

                // 加入弱參考
                SharedColumns[groupName].Add(new WeakReference<DataGridColumn>(column));

                // 監聽寬度變化
                var dpd = DependencyPropertyDescriptor.FromProperty(DataGridColumn.ActualWidthProperty, typeof(DataGridColumn));
                if (dpd != null)
                {
                    dpd.AddValueChanged(column, OnColumnWidthChanged);
                }
            }

            private static void UnregisterColumn(DataGridColumn column, string groupName)
            {
                if (SharedColumns.ContainsKey(groupName))
                {
                    var list = SharedColumns[groupName];

                    // 移除對應的弱參考
                    var target = list.FirstOrDefault(w => w.TryGetTarget(out var c) && c == column);
                    if (target != null)
                    {
                        list.Remove(target);
                    }

                    var dpd = DependencyPropertyDescriptor.FromProperty(DataGridColumn.ActualWidthProperty, typeof(DataGridColumn));
                    if (dpd != null)
                    {
                        dpd.RemoveValueChanged(column, OnColumnWidthChanged);
                    }
                }
            }
            private static void OnColumnWidthChanged(object? sender, EventArgs e)
            {
                if (_isUpdating) return;
                if (sender is not DataGridColumn column) return;

                string groupName = GetSharedSizeGroup(column);
                if (string.IsNullOrEmpty(groupName)) return;

                SynchronizeWidths(groupName);
            }
            private static void SynchronizeWidths(string groupName)
            {
            Debug.WriteLine("Sync. Width");
                if (!SharedColumns.ContainsKey(groupName)) return;

                try
                {
                    _isUpdating = true;

                    var weakList = SharedColumns[groupName];
                    var aliveColumns = new List<DataGridColumn>();
                    var deadReferences = new List<WeakReference<DataGridColumn>>();

                    // 1. 整理存活的欄位，並標記死亡的參考
                    foreach (var weakRef in weakList)
                    {
                        if (weakRef.TryGetTarget(out var col))
                        {
                            aliveColumns.Add(col);
                        }
                        else
                        {
                            deadReferences.Add(weakRef);
                        }
                    }

                    // 2. 清理死亡的參考 (Lazy Cleanup)
                    foreach (var dead in deadReferences)
                    {
                        weakList.Remove(dead);
                    }

                    // 如果沒有存活的欄位，就結束
                    if (!aliveColumns.Any()) return;

                    // 3. 計算最大寬度
                    double maxWidth = aliveColumns.Max(c => c.ActualWidth);
                //Debug.WriteLine(maxWidth);
                    // 4. 套用寬度
                    foreach (var col in aliveColumns)
                    {
                        Debug.WriteLine(col.ActualWidth);
                        // 只在差異大於 1 像素時更新，避免浮點數誤差造成的無限迴圈
                        if (Math.Abs(col.Width.Value - maxWidth) > 1.0)
                        {
                            col.Width = new DataGridLength(maxWidth);
                        }
                    }
                }
                finally
                {
                    _isUpdating = false;
                }
            }
        }
}
