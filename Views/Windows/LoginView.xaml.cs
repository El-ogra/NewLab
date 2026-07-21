using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using NewLab.ViewModels.Pages;

namespace NewLab.Views.Windows
{
    public partial class LoginView : Window
    {
        public LoginView(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            
            var vm = serviceProvider.GetRequiredService<LoginViewModel>();
            vm.OnSuccess = () =>
            {
                var mainWindow = new MainWindow(serviceProvider);
                
                // When MainWindow closes, show LoginView again
                mainWindow.Closed += (s, e) =>
                {
                    var newLoginView = new LoginView(serviceProvider);
                    newLoginView.Show();
                };
                
                this.Close();
                mainWindow.Show();
            };
            
            DataContext = vm;

            PwdBox.PasswordChanged += (s, e) =>
            {
                if (DataContext is LoginViewModel viewModel)
                {
                    viewModel.Password = PwdBox.Password;
                }
            };
        }
    }
}
