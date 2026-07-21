using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NewLab.ViewModels.Pages;

namespace NewLab.Views.Windows
{
    public partial class SetupView : Window
    {
        public SetupView(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            
            var vm = serviceProvider.GetRequiredService<SetupViewModel>();
            vm.OnSuccess = () =>
            {
                var mainWindow = new MainWindow();
                
                // When MainWindow closes, show LoginView (not SetupView again)
                mainWindow.Closed += (s, e) =>
                {
                    var loginView = new LoginView(serviceProvider);
                    loginView.Show();
                };
                
                this.Close();
                mainWindow.Show();
            };
            
            DataContext = vm;
        }
    }
}
