using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NewLab.ViewModels.Pages;

namespace NewLab.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            
            var vm = serviceProvider.GetRequiredService<MainDashboardViewModel>();
            DataContext = vm;
        }
    }
}
