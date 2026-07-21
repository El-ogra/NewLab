using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLab.Services.Interfaces;
using System.Windows;

namespace NewLab.ViewModels.Pages
{
    public partial class SetupViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IApplicationStartupService _startupService;

        [ObservableProperty] private string fullName = string.Empty;
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string confirmPassword = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string phoneNumber = string.Empty;
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private bool isBusy;

        public Action? OnSuccess { get; set; }

        public SetupViewModel(IAuthService authService, IApplicationStartupService startupService)
        {
            _authService = authService;
            _startupService = startupService;
        }

        [RelayCommand]
        private async Task CreateAdminAccountAsync()
        {
            ErrorMessage = string.Empty;

            // Basic Validation
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "يرجى ملء جميع الحقول المطلوبة (الاسم، اسم المستخدم، كلمة المرور)";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين";
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "يجب أن تتكون كلمة المرور من 6 أحرف على الأقل";
                return;
            }

            IsBusy = true;
            try
            {
                await _authService.CreateAdminAccountAsync(Username, Password, FullName, Email, PhoneNumber);
                await _startupService.SeedDefaultRolesAsync();

                OnSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
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
    }
}
