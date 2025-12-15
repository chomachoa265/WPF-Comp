using DataGrid_EX.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Page_ComboBoxs.xaml 的互動邏輯
    /// </summary>
    public partial class Page_ComboBoxs : Page
    {
        public Page_ComboBoxs()
        {
            InitializeComponent();
            DataContext = new Page_ComboBoxes_VM();
        }
    }
}
