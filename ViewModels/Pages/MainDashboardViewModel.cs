using System;
using System.Collections.Generic;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;

namespace NewLab.ViewModels.Pages
{
    public partial class MainDashboardViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty] private bool isToolbarVisible = true;
        [ObservableProperty] private bool isDashboardMode = true;
        [ObservableProperty] private ToolbarItem? selectedCategory;
        [ObservableProperty] private object? currentContent;

        public List<ToolbarItem> ToolbarCategories { get; }

        public MainDashboardViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ToolbarCategories = CreateToolbarCategories();
            SelectedCategory = ToolbarCategories[0];
            UpdateContent();
        }

        [RelayCommand]
        private void SelectCategory(ToolbarItem? category)
        {
            if (category == null) return;
            SelectedCategory = category;
            IsToolbarVisible = true;
            IsDashboardMode = true;
            CurrentFunction = null;
            UpdateContent();
        }

        [ObservableProperty] private FunctionDefinition? currentFunction;

        [RelayCommand]
        private void OpenFunction(FunctionDefinition? function)
        {
            if (function == null) return;
            CurrentFunction = function;
            IsToolbarVisible = false;
            IsDashboardMode = false;
            UpdateContent();
        }

        [RelayCommand]
        private void CloseFunction()
        {
            CurrentFunction = null;
            IsToolbarVisible = true;
            IsDashboardMode = true;
            UpdateContent();
        }

        [RelayCommand]
        private void Exit()
        {
            Application.Current.Shutdown();
        }

        private void UpdateContent()
        {
            CurrentContent = this;
        }

        private List<ToolbarItem> CreateToolbarCategories()
        {
            return new List<ToolbarItem>
            {
                new ToolbarItem
                {
                    IconKind = PackIconKind.PersonMultiple,
                    Label = "المرضى",
                    Category = "Patients",
                    Functions = new List<FunctionDefinition>
                    {
                        new FunctionDefinition { Name = "إدخال نتائج التحاليل", IconName = "Flask" },
                        new FunctionDefinition { Name = "إضافة وتعديل بيانات المرضى", IconName = "AccountEdit" },
                        new FunctionDefinition { Name = "بحث عن مريض", IconName = "Magnify" },
                        new FunctionDefinition { Name = "تسليم نتائج المرضى", IconName = "FileCheck" }
                    }
                },
                new ToolbarItem
                {
                    IconKind = PackIconKind.Wrench,
                    Label = "أدوات",
                    Category = "Tools",
                    Functions = new List<FunctionDefinition>()
                },
                new ToolbarItem
                {
                    IconKind = PackIconKind.FileDocument,
                    Label = "ورقة عمل",
                    Category = "Worksheet",
                    Functions = new List<FunctionDefinition>()
                },
                new ToolbarItem
                {
                    IconKind = PackIconKind.CreditCard,
                    Label = "حسابات",
                    Category = "Accounts",
                    Functions = new List<FunctionDefinition>()
                },
                new ToolbarItem
                {
                    IconKind = PackIconKind.ChartBar,
                    Label = "إحصائيات",
                    Category = "Statistics",
                    Functions = new List<FunctionDefinition>()
                },
                new ToolbarItem
                {
                    IconKind = PackIconKind.AccountGroup,
                    Label = "المستخدمين",
                    Category = "Users",
                    Functions = new List<FunctionDefinition>()
                },
                new ToolbarItem
                {
                    IconKind = PackIconKind.Database,
                    Label = "بيانات النظام",
                    Category = "SystemData",
                    Functions = new List<FunctionDefinition>()
                },
                new ToolbarItem
                {
                    IconKind = PackIconKind.Cog,
                    Label = "إعدادات",
                    Category = "Settings",
                    Functions = new List<FunctionDefinition>()
                },
                new ToolbarItem
                {
                    IconKind = PackIconKind.HelpCircle,
                    Label = "هل تعلم",
                    Category = "Tips",
                    Functions = new List<FunctionDefinition>()
                },
                new ToolbarItem
                {
                    IconKind = PackIconKind.Information,
                    Label = "نبذة",
                    Category = "About",
                    Functions = new List<FunctionDefinition>()
                },
                new ToolbarItem
                {
                    IconKind = PackIconKind.ExitToApp,
                    Label = "خروج",
                    Category = "Exit",
                    Functions = new List<FunctionDefinition>()
                }
            };
        }
    }
}