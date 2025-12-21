using DataGrid_EX.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataGrid_EX.Views
{
    /// <summary>
    /// Page_DataGrids.xaml 的互動邏輯
    /// </summary>
    public partial class Page_DataGrids : Page
    {
        public Page_DataGrids()
        {
            InitializeComponent();
            Debug.WriteLine("Page_DataGridsTirgerr");
            DataContext = new Page_DataGrids_VM();

            // 模擬一些資料 (僅作示範，實際應來自 ViewModel)
        //    var data = new[] {
        //    new { Id = 1, Name = "Item A", Description = "Test data for synchronization" },
        //    new { Id = 2, Name = "Item B", Description = "Resize the columns to see magic" }
        //};

        //    TopGrid.ItemsSource = data;
        //    BottomGrid.ItemsSource = data;
        }
    }
}
