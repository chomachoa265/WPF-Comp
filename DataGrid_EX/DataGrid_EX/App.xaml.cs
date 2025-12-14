using System.Configuration;
using System.Data;
using System.Windows;

namespace DataGrid_EX
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow();
            this.MainWindow = window;
            window.Show();
        }
    }

}
