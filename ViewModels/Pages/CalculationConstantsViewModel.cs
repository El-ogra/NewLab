using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;

namespace NewLab.ViewModels.Pages
{
    public partial class CalculationConstantsViewModel : ObservableObject
    {
        private readonly IAutoCalculationService _autoCalculationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty] private ObservableCollection<CalculationConstant> constants = new();

        public CalculationConstantsViewModel(
            IAutoCalculationService autoCalculationService,
            IDialogService dialogService)
        {
            _autoCalculationService = autoCalculationService;
            _dialogService = dialogService;

            _ = LoadConstantsAsync();
        }

        [RelayCommand]
        private async Task LoadConstantsAsync()
        {
            var items = await _autoCalculationService.GetConstantsAsync();
            Constants.Clear();
            foreach (var item in items)
                Constants.Add(item);
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                foreach (var constant in Constants)
                {
                    await _autoCalculationService.UpdateConstantAsync(constant.Id, constant.ConstantValue);
                }
                _dialogService.ShowMessage("نجاح", "تم حفظ الثوابت بنجاح");
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", ex.Message);
            }
        }
    }
}
