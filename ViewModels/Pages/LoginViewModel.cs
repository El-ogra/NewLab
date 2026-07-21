using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLab.Services.Interfaces;

namespace NewLab.ViewModels.Pages
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private bool showPassword;
        [ObservableProperty] private bool rememberLogin;
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private bool isBusy;

        public Action? OnSuccess { get; set; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task SignInAsync()
        {
            ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "يرجى إدخال اسم المستخدم وكلمة المرور";
                return;
            }

            IsBusy = true;
            try
            {
                var user = await _authService.ValidateCredentialsAsync(Username, Password);
                if (user != null)
                {
                    OnSuccess?.Invoke();
                }
                else
                {
                    ErrorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "حدث خطأ أثناء تسجيل الدخول: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        // Command to handle Enter key press
        [RelayCommand]
        private void HandleEnterKey()
        {
            SignInCommand.Execute(null);
        }
    }
}
