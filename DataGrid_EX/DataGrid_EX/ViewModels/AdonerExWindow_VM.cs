using CommunityToolkit.Mvvm.Input;
using DataGrid_EX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace DataGrid_EX.ViewModels
{
    public partial class AdonerExWindow_VM
    {

        public ObservableCollection<Box> Boxes { get; set; }
        public AdonerExWindow_VM()
        {
            Boxes = new ObservableCollection<Box>
            {
                
            };
        }
    }
}
