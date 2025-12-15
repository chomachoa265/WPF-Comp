using DataGrid_EX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataGrid_EX.Services
{
    public class FrameNavigationService: INavigationService
    {
        private readonly Frame _frame;

        public FrameNavigationService(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate<TView>() where TView: Page, new()
        {
            _frame.Navigate(new TView());
        }

        public void GoBack()
        {
            if (_frame.CanGoBack)
            {
                _frame.GoBack();
            }
        }
    }
}
