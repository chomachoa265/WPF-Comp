using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace DataGrid_EX.Models
{
    public class ResizeAdorner:Adorner
    {
        private readonly Thumb _bottomRightThumb;
        private readonly VisualCollection _visuals;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visuals = new VisualCollection(this);

            // 初始化手柄 (Thumb)
            _bottomRightThumb = new Thumb
            {
                Cursor = System.Windows.Input.Cursors.SizeNWSE,
                Width = 10,
                Height = 10,
                Background = Brushes.Red // 紅色手柄以便識別
            };

            // 監聽拖曳事件
            _bottomRightThumb.DragDelta += OnDragDelta;

            _visuals.Add(_bottomRightThumb);
        }
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = (FrameworkElement)AdornedElement;

            // 計算新尺寸 (確保不小於 10)
            var newWidth = Math.Max(element.Width + e.HorizontalChange, 10);
            var newHeight = Math.Max(element.Height + e.VerticalChange, 10);

            // 修改 View 的尺寸
            // 因為 ViewModel 與此 View 是 TwoWay Binding，所以 VM 數據會自動更新
            element.Width = newWidth;
            element.Height = newHeight;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            var element = (FrameworkElement)AdornedElement;

            // 將手柄放置在右下角
            _bottomRightThumb.Arrange(new Rect(
                element.Width - 5,  // 稍微偏移以置中
                element.Height - 5,
                _bottomRightThumb.Width,
                _bottomRightThumb.Height));

            return finalSize;
        }

        // 必要的 Override，讓 WPF 知道有子元素
        protected override int VisualChildrenCount => _visuals.Count;
        protected override Visual GetVisualChild(int index) => _visuals[index];
    }
}
