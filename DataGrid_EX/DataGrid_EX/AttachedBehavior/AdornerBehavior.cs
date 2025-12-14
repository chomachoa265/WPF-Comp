using DataGrid_EX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace DataGrid_EX.AttachedBehavior
{
    public static class AdornerBehavior
    {
        // 定義附加屬性
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.RegisterAttached(
                "IsActive",
                typeof(bool),
                typeof(AdornerBehavior),
                new PropertyMetadata(false, OnIsActiveChanged));

        public static bool GetIsActive(DependencyObject obj) => (bool)obj.GetValue(IsActiveProperty);
        public static void SetIsActive(DependencyObject obj, bool value) => obj.SetValue(IsActiveProperty, value);

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                // 確保元素已經載入，否則 AdornerLayer 可能為 null
                if (element.IsLoaded)
                    UpdateAdorner(element, (bool)e.NewValue);
                else
                    element.Loaded += (s, arg) => UpdateAdorner(element, GetIsActive(element));
            }
        }

        private static void UpdateAdorner(FrameworkElement element, bool show)
        {
            var layer = AdornerLayer.GetAdornerLayer(element);
            if (layer == null) return;

            // 1. 清除現有的 Adorner (避免重複)
            var adorners = layer.GetAdorners(element);
            if (adorners != null)
            {
                foreach (var existing in adorners)
                {
                    if (existing is ResizeAdorner) layer.Remove(existing);
                }
            }

            // 2. 如果 IsSelected 為 true，則新增 Adorner
            if (show)
            {
                layer.Add(new ResizeAdorner(element));
            }
        }
    }
}
