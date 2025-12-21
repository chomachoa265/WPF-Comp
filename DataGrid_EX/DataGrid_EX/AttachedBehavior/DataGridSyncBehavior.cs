using Microsoft.Xaml.Behaviors;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DataGrid_EX.AttachedBehavior
{
    /// <summary>
    /// 用於同步兩個 DataGrid 的捲動、欄寬與欄位順序
    /// </summary>
    public class DataGridSyncBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty TargetGridProperty =
            DependencyProperty.Register(nameof(TargetGrid), typeof(DataGrid), typeof(DataGridSyncBehavior), new PropertyMetadata(null));

        public DataGrid TargetGrid
        {
            get { return (DataGrid)GetValue(TargetGridProperty); }
            set { SetValue(TargetGridProperty, value); }
        }

        private ScrollViewer? _sourceScrollViewer;
        private ScrollViewer? _targetScrollViewer;
        private bool _isSyncingColumnOrder = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            Cleanup(); // 統一清理資源
            base.OnDetaching();
        }

        private void Cleanup()
        {
            this.AssociatedObject.Loaded -= OnLoaded;

            // 解除 Columns 變動監聽
            if (this.AssociatedObject.Columns is INotifyCollectionChanged sourceCols)
                sourceCols.CollectionChanged -= OnColumnsCollectionChanged;

            if (TargetGrid != null && TargetGrid.Columns is INotifyCollectionChanged targetCols)
                targetCols.CollectionChanged -= OnColumnsCollectionChanged;

            // 解除捲動與排序監聽
            if (_sourceScrollViewer != null) _sourceScrollViewer.ScrollChanged -= OnSourceScrollChanged;
            if (_targetScrollViewer != null) _targetScrollViewer.ScrollChanged -= OnTargetScrollChanged;

            this.AssociatedObject.ColumnDisplayIndexChanged -= OnSourceColumnMoved;
            if (TargetGrid != null) TargetGrid.ColumnDisplayIndexChanged -= OnTargetColumnMoved;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (TargetGrid == null) return;

            // 1. 初始化 ScrollViewer 同步
            InitializeScrollSync();

            // 2. 初始化欄位監聽 (新增的部分)
            InitializeColumnCollectionSync();

            // 3. 第一次執行寬度綁定
            SyncColumnWidths();

            // 4. 訂閱順序變更
            this.AssociatedObject.ColumnDisplayIndexChanged += OnSourceColumnMoved;
            TargetGrid.ColumnDisplayIndexChanged += OnTargetColumnMoved;
        }

        // --- 新增：監聽欄位數量變化 ---
        private void InitializeColumnCollectionSync()
        {
            if (this.AssociatedObject.Columns is INotifyCollectionChanged sourceCols)
                sourceCols.CollectionChanged += OnColumnsCollectionChanged;

            if (TargetGrid.Columns is INotifyCollectionChanged targetCols)
                targetCols.CollectionChanged += OnColumnsCollectionChanged;
        }

        private void OnColumnsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // 當任一邊的欄位數量變動時，重新執行綁定
            // 這裡可以做 Debounce (防抖) 優化，但簡單起見直接呼叫
            SyncColumnWidths();
        }
        // -----------------------------

        private void SyncColumnWidths()
        {
            // 確保只同步雙方都存在的欄位索引
            int count = System.Math.Min(this.AssociatedObject.Columns.Count, TargetGrid.Columns.Count);

            for (int i = 0; i < count; i++)
            {
                var sourceCol = this.AssociatedObject.Columns[i];
                var targetCol = TargetGrid.Columns[i];

                // 先清除舊綁定 (避免重複綁定造成記憶體問題或衝突)
                BindingOperations.ClearBinding(targetCol, DataGridColumn.WidthProperty);
                BindingOperations.ClearBinding(targetCol, DataGridColumn.MinWidthProperty);
                BindingOperations.ClearBinding(targetCol, DataGridColumn.VisibilityProperty);

                // 重新綁定
                BindingOperations.SetBinding(targetCol, DataGridColumn.WidthProperty,
                    new Binding("Width") { Source = sourceCol, Mode = BindingMode.TwoWay });

                BindingOperations.SetBinding(targetCol, DataGridColumn.MinWidthProperty,
                    new Binding("MinWidth") { Source = sourceCol, Mode = BindingMode.TwoWay });

                BindingOperations.SetBinding(targetCol, DataGridColumn.VisibilityProperty,
                    new Binding("Visibility") { Source = sourceCol, Mode = BindingMode.TwoWay });
            }
        }

        // ... (以下 Scroll 與 Order 相關程式碼保持不變，與上個版本相同) ...
        #region Scroll & Order Logic (Keep Existing Code)
        private void InitializeScrollSync()
        {
            _sourceScrollViewer = GetScrollViewer(this.AssociatedObject);
            _targetScrollViewer = GetScrollViewer(TargetGrid);
            if (_sourceScrollViewer != null) _sourceScrollViewer.ScrollChanged += OnSourceScrollChanged;
            if (_targetScrollViewer != null) _targetScrollViewer.ScrollChanged += OnTargetScrollChanged;
        }
        private void OnSourceScrollChanged(object sender, ScrollChangedEventArgs e) { if (e.HorizontalChange != 0 && _targetScrollViewer != null) _targetScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset); }
        private void OnTargetScrollChanged(object sender, ScrollChangedEventArgs e) { if (e.HorizontalChange != 0 && _sourceScrollViewer != null) _sourceScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset); }
        private static ScrollViewer? GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer viewer) return viewer;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) { var child = VisualTreeHelper.GetChild(depObj, i); var result = GetScrollViewer(child); if (result != null) return result; }
            return null;
        }
        private void OnSourceColumnMoved(object? sender, DataGridColumnEventArgs e) => SyncOrder(this.AssociatedObject, TargetGrid, e.Column);
        private void OnTargetColumnMoved(object? sender, DataGridColumnEventArgs e) => SyncOrder(TargetGrid, this.AssociatedObject, e.Column);
        private void SyncOrder(DataGrid source, DataGrid target, DataGridColumn sourceCol)
        {
            if (_isSyncingColumnOrder) return;
            _isSyncingColumnOrder = true;
            try
            {
                var targetCol = target.Columns.FirstOrDefault(c => Equals(c.Header, sourceCol.Header));
                if (targetCol != null) targetCol.DisplayIndex = sourceCol.DisplayIndex;
            }
            finally { _isSyncingColumnOrder = false; }
        }
        #endregion
    }
}