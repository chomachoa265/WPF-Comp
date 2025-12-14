using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_EX.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Box
    {

        public double Width = 100;
        public double Height = 50;
        public double Top = 50;
        public double Left = 50;
        public bool IsSelected;

    }
}
