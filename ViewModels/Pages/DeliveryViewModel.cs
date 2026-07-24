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
        [ObservableProperty] private int unenteredCount;
        [ObservableProperty] private int undeliveredCount;
        [ObservableProperty] private int unprintedCount;
        [ObservableProperty] private decimal remainingBalance;
        [ObservableProperty] private DeliveryFilterMode filterMode = DeliveryFilterMode.Undelivered;
        [ObservableProperty] private DateTime? filterDateFrom = DateTime.Today;
        [ObservableProperty] private DateTime? filterDateTo = DateTime.Today;
        [ObservableProperty] private string searchCode = string.Empty;
        [ObservableProperty] private decimal settlementAmount;

        private DeliveryPatientRow? _pendingReceiptPatient;

        public bool IsAdmin => _currentUserService.IsAdmin;
        public DeliveryFilterMode[] FilterModes => Enum.GetValues<DeliveryFilterMode>();

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

            // Check remaining balance
            if (SelectedPatient.RemainingBalance > 0)
            {
                var continueWithBalance = _dialogService.ShowConfirmation("تنبيه باقي حساب",
                    $"يوجد باقي حساب {SelectedPatient.RemainingBalance:F2} — هل تريد الاستمرار في التسليم؟");
                if (!continueWithBalance) return;
            }

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
            if (string.IsNullOrWhiteSpace(raw)) return;

            var codeType = Helpers.BarcodeCodeTypeDetector.Detect(raw);

            if (codeType == Helpers.BarcodeCodeType.VisitCode)
            {
                // First scan: receipt code (starts with '1')
                var result = await _deliveryService.SearchByCodeAsync(raw);
                if (result != null)
                {
                    _pendingReceiptPatient = result;
                    _dialogService.ShowMessage("معلومة", $"تم تسجيل كود الإيصال: {result.FullName}\nالآن امسح كود الملف للتأكيد");
                }
                else
                {
                    _dialogService.ShowMessage("خطأ", "لم يتم العثور على مريض بهذا الكود");
                }
                return;
            }

            if (codeType == Helpers.BarcodeCodeType.FileCode && _pendingReceiptPatient != null)
            {
                // Second scan: file code (starts with '3') — compare with pending
                var result = await _deliveryService.SearchByCodeAsync(raw);
                if (result != null && result.PatientId == _pendingReceiptPatient.PatientId)
                {
                    // Match confirmed
                    var confirmed = _dialogService.ShowConfirmation("تأكيد التسليم", $"هل تريد تسليم النتائج لـ {_pendingReceiptPatient.FullName}؟");
                    if (confirmed)
                    {
                        await _deliveryService.DeliverAllResultsAsync(_pendingReceiptPatient.PatientId, _currentUserService.CurrentUser!.Id);
                        _dialogService.ShowMessage("نجاح", "تم التسليم بنجاح");
                    }
                    _pendingReceiptPatient = null;
                    await RefreshAsync();
                    return;
                }
                else
                {
                    _dialogService.ShowMessage("خطأ", "كود الملف لا يتطابق مع كود الإيصال");
                    _pendingReceiptPatient = null;
                    return;
                }
            }

            // Default: normal search
            var defaultResult = await _deliveryService.SearchByCodeAsync(raw);
            if (defaultResult != null)
            {
                Patients.Clear();
                Patients.Add(defaultResult);
                SelectedPatient = defaultResult;
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

        [RelayCommand]
        private async Task ClearAccountAsync()
        {
            if (SelectedPatient == null) return;
            var confirmed = _dialogService.ShowConfirmation("تأكيد تصفية الحساب", "هل أنت متأكد من تصفية حساب هذا المريض؟");
            if (!confirmed) return;

            var userId = _currentUserService.CurrentUser?.Id ?? 0;
            await _deliveryService.ClearAccountAsync(SelectedPatient.PatientId, userId);
            _dialogService.ShowMessage("نجاح", "تم تصفية الحساب بنجاح");
            await LoadTestsAndSummaryAsync(SelectedPatient.PatientId);
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

        partial void OnFilterModeChanged(DeliveryFilterMode value)
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
            UnenteredCount = state.Unentered;
            UndeliveredCount = state.Undelivered;
            UnprintedCount = state.Unprinted;
            RemainingBalance = state.Remaining;
        }

        private DeliveryFilter BuildFilter()
        {
            bool onlyUndelivered = FilterMode == DeliveryFilterMode.Undelivered;
            bool onlyLabToLab = FilterMode == DeliveryFilterMode.LabToLab;
            bool onlyIndividual = FilterMode == DeliveryFilterMode.Individual;
            bool onlyImportant = FilterMode == DeliveryFilterMode.Important;

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
