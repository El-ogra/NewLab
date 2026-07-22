using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation;
using NewLab.Data;
using NewLab.Models.Domain;
using NewLab.Models.Validation;
using NewLab.Services.Implementations;
using NewLab.Services.Interfaces;
using NewLab.ViewModels.Pages;
using NewLab.Views.Windows;

namespace NewLab
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // 1. Register DbContext with the connection string from appsettings.json
                    services.AddDbContext<NewLabDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                    // 2. Register our new Services (Scoped because they use DbContext)
                    services.AddScoped<IApplicationStartupService, ApplicationStartupService>();
                    services.AddScoped<IAuthService, AuthService>();
                    services.AddScoped<IPatientService, PatientService>();
                    services.AddScoped<IReferralService, ReferralService>();

                    // 2.1 Register Validators
                    services.AddScoped<IValidator<Patient>, PatientValidator>();
                    
                    // 3. Register existing services (from previous phases)
                    services.AddSingleton<ICurrentUserService, CurrentUserService>();
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddSingleton<IDialogService, DialogService>();

                    // 4. Register ViewModels
                    services.AddTransient<SetupViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<MainDashboardViewModel>();
                    services.AddTransient<PatientEntryViewModel>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Use a separate scope for initial database checks
            bool isFirstRun;
            using (var scope = _host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NewLabDbContext>();
                await dbContext.Database.MigrateAsync();

                var startupService = scope.ServiceProvider.GetRequiredService<IApplicationStartupService>();
                isFirstRun = await startupService.IsFirstRunAsync();
            }
            // Scope is disposed here, but that's OK because we only needed it for the check

            if (isFirstRun)
            {
                var setupWindow = new SetupView(_host.Services);
                setupWindow.Show();
            }
            else
            {
                var loginWindow = new LoginView(_host.Services);
                loginWindow.Show();
            }

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}
