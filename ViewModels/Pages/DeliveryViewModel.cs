using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;
using NewLab.Services.Interfaces;

namespace NewLab.ViewModels.Pages
{
    public partial class DeliveryViewModel : ObservableObject
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ICurrentUserService _currentUserService;

        [ObservableProperty] private ObservableCollection<DeliveryPatientRow> patients = new();
        [ObservableProperty] private DeliveryPatientRow? selectedPatient;
        [ObservableProperty] private ObservableCollection<DeliveryPatientTestRow> patientTests = new();
        [ObservableProperty] private int undeliveredCount;
        [ObservableProperty] private int unprintedCount;
        [ObservableProperty] private decimal remainingBalance;
        [ObservableProperty] private string filterMode = "Undelivered";
        [ObservableProperty] private DateTime? filterDateFrom = DateTime.Today;
        [ObservableProperty] private DateTime? filterDateTo = DateTime.Today;
        [ObservableProperty] private string searchCode = string.Empty;
        [ObservableProperty] private decimal settlementAmount;

        public bool IsAdmin => _currentUserService.IsAdmin;

        public DeliveryViewModel(
            IDeliveryService deliveryService,
            IDialogService dialogService,
            INavigationService navigationService,
            ICurrentUserService currentUserService)
        {
            _deliveryService = deliveryService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _currentUserService = currentUserService;

            _ = RefreshAsync();
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            var filter = BuildFilter();
            var items = await _deliveryService.FilterAsync(filter);
            Patients.Clear();
            foreach (var item in items)
                Patients.Add(item);
        }

        [RelayCommand(CanExecute = nameof(CanDeliver))]
        private async Task DeliverManuallyAsync()
        {
            if (SelectedPatient == null) return;

            var confirmed = _dialogService.ShowConfirmation("تأكيد التسليم", "هل أنت متأكد من تسليم جميع نتائج هذا المريض؟");
            if (!confirmed) return;

            try
            {
                await _deliveryService.DeliverAllResultsAsync(SelectedPatient.PatientId, _currentUserService.CurrentUser!.Id);
                _dialogService.ShowMessage("نجاح", "تم تسليم جميع النتائج بنجاح");
                await RefreshAsync();
                SelectedPatient = null;
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", "حدث خطأ أثناء التسليم: " + ex.Message);
            }
        }

        [RelayCommand(CanExecute = nameof(CanSettle))]
        private async Task SettleAccountAsync()
        {
            if (SelectedPatient == null || SettlementAmount <= 0) return;

            try
            {
                await _deliveryService.SettleAccountAsync(SelectedPatient.PatientId, SettlementAmount, _currentUserService.CurrentUser!.Id);
                _dialogService.ShowMessage("نجاح", $"تم تسجيل تسديد مبلغ {SettlementAmount}");
                SettlementAmount = 0;
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", "حدث خطأ أثناء التسديد: " + ex.Message);
            }
        }

        [RelayCommand(CanExecute = nameof(CanUnmark))]
        private async Task UnmarkDeliveredAsync()
        {
            if (SelectedPatient == null) return;

            var confirmed = _dialogService.ShowConfirmation("تأكيد إلغاء التسليم", "هل أنت متأكد من إلغاء التسليم؟");
            if (!confirmed) return;

            try
            {
                await _deliveryService.UnmarkDeliveredAsync(SelectedPatient.PatientId, _currentUserService.CurrentUser!.Id);
                _dialogService.ShowMessage("نجاح", "تم إلغاء التسليم بنجاح");
                await RefreshAsync();
                SelectedPatient = null;
            }
            catch (UnauthorizedAccessException ex)
            {
                _dialogService.ShowMessage("خطأ", ex.Message);
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", "حدث خطأ: " + ex.Message);
            }
        }

        [RelayCommand]
        private async Task SearchByCodeAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchCode)) return;

            var result = await _deliveryService.SearchByCodeAsync(SearchCode);
            if (result != null)
            {
                Patients.Clear();
                Patients.Add(result);
                SelectedPatient = result;
            }
            else
            {
                _dialogService.ShowMessage("خطأ", "لم يتم العثور على مريض بهذا الكود");
            }
        }

        [RelayCommand]
        private async Task ScanBarcodeAsync(string raw)
        {
            var result = await _deliveryService.SearchByCodeAsync(raw);
            if (result != null)
            {
                Patients.Clear();
                Patients.Add(result);
                SelectedPatient = result;
            }
            else
            {
                _dialogService.ShowMessage("خطأ", "لم يتم العثور على مريض بهذا الكود");
            }
        }

        [RelayCommand]
        private void BackToMain()
        {
            _navigationService.GoBack();
        }

        private bool CanDeliver() => SelectedPatient != null;
        private bool CanSettle() => SelectedPatient != null && SettlementAmount > 0;
        private bool CanUnmark() => IsAdmin && SelectedPatient != null && SelectedPatient.AggregateStatus >= TestStatus.Delivered;

        partial void OnSelectedPatientChanged(DeliveryPatientRow? value)
        {
            if (value != null) _ = LoadTestsAndSummaryAsync(value.PatientId);
            DeliverManuallyCommand.NotifyCanExecuteChanged();
            SettleAccountCommand.NotifyCanExecuteChanged();
            UnmarkDeliveredCommand.NotifyCanExecuteChanged();
        }

        partial void OnFilterModeChanged(string value)
        {
            _ = RefreshAsync();
        }

        private async Task LoadTestsAndSummaryAsync(int patientId)
        {
            var tests = await _deliveryService.GetPatientTestsAsync(patientId);
            PatientTests.Clear();
            foreach (var test in tests)
                PatientTests.Add(test);

            var state = await _deliveryService.GetPatientDeliveryStateAsync(patientId);
            UndeliveredCount = state.Undelivered;
            UnprintedCount = state.Unprinted;
            RemainingBalance = state.Remaining;
        }

        private DeliveryFilter BuildFilter()
        {
            bool onlyUndelivered = FilterMode == "Undelivered";
            bool onlyLabToLab = FilterMode == "LabToLab";
            bool onlyIndividual = FilterMode == "Individual";
            bool onlyImportant = FilterMode == "Important";

            return new DeliveryFilter(
                OnlyUndelivered: onlyUndelivered,
                OnlyLabToLab: onlyLabToLab,
                OnlyIndividual: onlyIndividual,
                OnlyImportant: onlyImportant,
                DateFrom: FilterDateFrom,
                DateTo: FilterDateTo);
        }
    }
}
