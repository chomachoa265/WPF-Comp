using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataGrid_EX.Interfaces
{
    public interface INavigationService
    {
        void Navigate<TView>() where TView: Page, new();
        void GoBack();
    }
}
