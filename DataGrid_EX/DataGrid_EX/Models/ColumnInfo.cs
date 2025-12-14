using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_EX.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ColumnInfo
    {
        public CustomColumnHeader Header { get; set; }
        public string PropName { get; set; }

        public ColumnInfo(CustomColumnHeader header, string propName)
        {
            Header = header;
            PropName = propName;
        }

        public string DataBindingPath => PropName;
        public string StatBindingPath => $"ValuesByColumn[{PropName}]";
     }
}
