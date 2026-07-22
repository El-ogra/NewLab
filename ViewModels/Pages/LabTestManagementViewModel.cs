using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;

namespace NewLab.ViewModels.Pages
{
    public partial class LabTestManagementViewModel : ObservableObject
    {
        private readonly ILabTestService _labTestService;
        private readonly IReferralService _referralService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IValidator<LabTest> _labTestValidator;
        private readonly ICurrentUserService _currentUserService;
        private readonly Func<NormalRangeViewModel> _normalRangeVmFactory;

        public ObservableCollection<LabTest> Tests { get; } = new();
        public ObservableCollection<SpecimenType> AvailableSpecimenTypes { get; } = new();
        public ObservableCollection<Referral> AvailableReferrals { get; } = new();
        public ObservableCollection<TestGroup> AvailableGroups { get; } = new();
        public ObservableCollection<ReferralPrice> ReferralPrices { get; } = new();

        // Search fields
        [ObservableProperty] private string? searchByCode;
        [ObservableProperty] private string? searchByGroupName;
        [ObservableProperty] private string? searchByTestName;

        // Form fields (17 LabTest fields)
        [ObservableProperty] private LabTest? selectedTest;
        [ObservableProperty] private string formCode = string.Empty;
        [ObservableProperty] private string formTestName = string.Empty;
        [ObservableProperty] private string? formReportNameLarge;
        [ObservableProperty] private string? formReportNameSmall;
        [ObservableProperty] private string? formBillNameLarge;
        [ObservableProperty] private string? formBillNameSmall;
        [ObservableProperty] private string? formHistoryName;
        [ObservableProperty] private string? formArabicName;
        [ObservableProperty] private int? formTestGroupId;
        [ObservableProperty] private string? formLogGroup;
        [ObservableProperty] private string? formCollection;
        [ObservableProperty] private int formTestTimeDays;
        [ObservableProperty] private int formArrangeNumber;
        [ObservableProperty] private decimal formPatientPrice;
        [ObservableProperty] private decimal formLabToLabPrice;
        [ObservableProperty] private bool formIsRoutine;
        [ObservableProperty] private bool formIsSeeReport;
        [ObservableProperty] private bool formIsPrintWithOther;
        [ObservableProperty] private bool formIsAddWithGroup;
        [ObservableProperty] private bool formIsMainTest;
        [ObservableProperty] private int? formParentLabTestId;
        [ObservableProperty] private int? formDefaultSpecimenTypeId;
        [ObservableProperty] private bool formIsSentExternal;
        [ObservableProperty] private int? formExternalReferralId;
        [ObservableProperty] private decimal? formExternalCost;
        [ObservableProperty] private string? formPromptQuestion;
        [ObservableProperty] private bool formIsActive = true;

        // Referral price fields
        [ObservableProperty] private bool isEditMode;
        [ObservableProperty] private bool isAddMode = true;
        [ObservableProperty] private ReferralPrice? selectedReferralPrice;
        [ObservableProperty] private int? newReferralPrice_ReferralId;
        [ObservableProperty] private decimal newReferralPrice_Price;

        public bool IsAdmin => _currentUserService.IsAdmin;

        private int? _editingTestId;

        public LabTestManagementViewModel(
            ILabTestService labTestService,
            IReferralService referralService,
            IDialogService dialogService,
            INavigationService navigationService,
            IValidator<LabTest> labTestValidator,
            ICurrentUserService currentUserService,
            Func<NormalRangeViewModel> normalRangeVmFactory)
        {
            _labTestService = labTestService;
            _referralService = referralService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _labTestValidator = labTestValidator;
            _currentUserService = currentUserService;
            _normalRangeVmFactory = normalRangeVmFactory;

            _ = LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            await SearchAsync();
            await LoadDropdownsAsync();
        }

        private async Task LoadDropdownsAsync()
        {
            var referrals = await _referralService.SearchByNameAsync("");
            AvailableReferrals.Clear();
            foreach (var r in referrals)
                AvailableReferrals.Add(r);
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            var results = await _labTestService.SearchAsync(SearchByCode, SearchByGroupName, SearchByTestName);
            Tests.Clear();
            foreach (var t in results)
                Tests.Add(t);
        }

        [RelayCommand]
        private void Add()
        {
            ClearForm();
            IsAddMode = true;
            IsEditMode = false;
        }

        [RelayCommand]
        private async Task EditAsync()
        {
            if (SelectedTest == null) return;

            var test = await _labTestService.GetByIdAsync(SelectedTest.Id);
            if (test == null)
            {
                _dialogService.ShowMessage("خطأ", "لم يتم العثور على التحليل");
                return;
            }

            LoadTestToForm(test);
            IsEditMode = true;
            IsAddMode = false;
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteAsync()
        {
            if (SelectedTest == null) return;

            var confirmed = _dialogService.ShowConfirmation("تأكيد الحذف", "هل أنت متأكد من حذف هذا التحليل؟");
            if (!confirmed) return;

            try
            {
                await _labTestService.DeleteAsync(SelectedTest.Id);
                _dialogService.ShowMessage("نجاح", "تم حذف التحليل بنجاح");
                ClearForm();
                await SearchAsync();
            }
            catch (UnauthorizedAccessException ex)
            {
                _dialogService.ShowMessage("خطأ", ex.Message);
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", "حدث خطأ أثناء الحذف: " + ex.Message);
            }
        }

        private bool CanDelete() => IsAdmin;

        [RelayCommand]
        private async Task SaveAsync()
        {
            var labTest = BuildTestFromForm();

            var result = _labTestValidator.Validate(labTest);
            if (!result.IsValid)
            {
                _dialogService.ShowMessage("خطأ في التحقق", string.Join("\n", result.Errors.Select(e => e.ErrorMessage)));
                return;
            }

            try
            {
                if (IsEditMode && _editingTestId != null)
                {
                    labTest.Id = _editingTestId.Value;
                    await _labTestService.UpdateAsync(labTest);
                    _dialogService.ShowMessage("نجاح", "تم تحديث بيانات التحليل بنجاح");
                }
                else
                {
                    await _labTestService.AddAsync(labTest);
                    _dialogService.ShowMessage("نجاح", "تم إضافة التحليل بنجاح");
                }

                ClearForm();
                await SearchAsync();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", "حدث خطأ أثناء الحفظ: " + ex.Message);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            ClearForm();
        }

        [RelayCommand]
        private async Task OpenNormalRangeAsync()
        {
            if (SelectedTest == null)
            {
                _dialogService.ShowMessage("خطأ", "اختر تحليلاً أولاً");
                return;
            }

            var vm = _normalRangeVmFactory();
            var test = await _labTestService.GetByIdAsync(SelectedTest.Id);
            if (test == null) return;
            await vm.LoadForTest(test);

            var window = new Views.Windows.NormalRangeView
            {
                DataContext = vm,
                Owner = System.Windows.Application.Current.MainWindow
            };
            window.ShowDialog();
        }

        [RelayCommand(CanExecute = nameof(CanAddReferralPrice))]
        private async Task AddReferralPriceAsync()
        {
            if (SelectedTest == null || NewReferralPrice_ReferralId == null) return;

            try
            {
                await _labTestService.SetReferralPriceAsync(SelectedTest.Id, NewReferralPrice_ReferralId.Value, NewReferralPrice_Price);
                NewReferralPrice_ReferralId = null;
                NewReferralPrice_Price = 0;
                await LoadReferralPricesAsync();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", "حدث خطأ: " + ex.Message);
            }
        }

        private bool CanAddReferralPrice() => SelectedTest != null && NewReferralPrice_ReferralId != null && NewReferralPrice_Price >= 0;

        [RelayCommand(CanExecute = nameof(CanDeleteReferralPrice))]
        private async Task DeleteReferralPriceAsync()
        {
            if (SelectedTest == null || SelectedReferralPrice == null) return;

            try
            {
                await _labTestService.DeleteReferralPriceAsync(SelectedTest.Id, SelectedReferralPrice.ReferralId);
                SelectedReferralPrice = null;
                await LoadReferralPricesAsync();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", "حدث خطأ: " + ex.Message);
            }
        }

        private bool CanDeleteReferralPrice() => SelectedReferralPrice != null;

        partial void OnSelectedTestChanged(LabTest? value)
        {
            if (value != null)
            {
                LoadTestToForm(value);
            }
            _ = LoadReferralPricesAsync();
            DeleteCommand.NotifyCanExecuteChanged();
            AddReferralPriceCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedReferralPriceChanged(ReferralPrice? value)
        {
            DeleteReferralPriceCommand.NotifyCanExecuteChanged();
        }

        partial void OnNewReferralPrice_ReferralIdChanged(int? value)
        {
            AddReferralPriceCommand.NotifyCanExecuteChanged();
        }

        partial void OnNewReferralPrice_PriceChanged(decimal value)
        {
            AddReferralPriceCommand.NotifyCanExecuteChanged();
        }

        private async Task LoadReferralPricesAsync()
        {
            ReferralPrices.Clear();
            if (SelectedTest == null) return;

            var prices = await _labTestService.GetReferralPricesAsync(SelectedTest.Id);
            foreach (var rp in prices)
                ReferralPrices.Add(rp);
        }

        private LabTest BuildTestFromForm()
        {
            return new LabTest
            {
                Code = FormCode,
                TestName = FormTestName,
                ReportNameLarge = FormReportNameLarge,
                ReportNameSmall = FormReportNameSmall,
                BillNameLarge = FormBillNameLarge,
                BillNameSmall = FormBillNameSmall,
                HistoryName = FormHistoryName,
                ArabicName = FormArabicName,
                TestGroupId = FormTestGroupId,
                LogGroup = FormLogGroup,
                Collection = FormCollection,
                TestTimeDays = FormTestTimeDays,
                ArrangeNumber = FormArrangeNumber,
                PatientPrice = FormPatientPrice,
                LabToLabPrice = FormLabToLabPrice,
                IsRoutine = FormIsRoutine,
                IsSeeReport = FormIsSeeReport,
                IsPrintWithOther = FormIsPrintWithOther,
                IsAddWithGroup = FormIsAddWithGroup,
                IsMainTest = FormIsMainTest,
                ParentLabTestId = FormParentLabTestId,
                DefaultSpecimenTypeId = FormDefaultSpecimenTypeId,
                IsSentExternal = FormIsSentExternal,
                ExternalReferralId = FormExternalReferralId,
                ExternalCost = FormExternalCost,
                PromptQuestion = FormPromptQuestion,
                IsActive = FormIsActive
            };
        }

        private void LoadTestToForm(LabTest test)
        {
            _editingTestId = test.Id;
            FormCode = test.Code;
            FormTestName = test.TestName;
            FormReportNameLarge = test.ReportNameLarge;
            FormReportNameSmall = test.ReportNameSmall;
            FormBillNameLarge = test.BillNameLarge;
            FormBillNameSmall = test.BillNameSmall;
            FormHistoryName = test.HistoryName;
            FormArabicName = test.ArabicName;
            FormTestGroupId = test.TestGroupId;
            FormLogGroup = test.LogGroup;
            FormCollection = test.Collection;
            FormTestTimeDays = test.TestTimeDays;
            FormArrangeNumber = test.ArrangeNumber;
            FormPatientPrice = test.PatientPrice;
            FormLabToLabPrice = test.LabToLabPrice;
            FormIsRoutine = test.IsRoutine;
            FormIsSeeReport = test.IsSeeReport;
            FormIsPrintWithOther = test.IsPrintWithOther;
            FormIsAddWithGroup = test.IsAddWithGroup;
            FormIsMainTest = test.IsMainTest;
            FormParentLabTestId = test.ParentLabTestId;
            FormDefaultSpecimenTypeId = test.DefaultSpecimenTypeId;
            FormIsSentExternal = test.IsSentExternal;
            FormExternalReferralId = test.ExternalReferralId;
            FormExternalCost = test.ExternalCost;
            FormPromptQuestion = test.PromptQuestion;
            FormIsActive = test.IsActive;
        }

        private void ClearForm()
        {
            _editingTestId = null;
            FormCode = string.Empty;
            FormTestName = string.Empty;
            FormReportNameLarge = null;
            FormReportNameSmall = null;
            FormBillNameLarge = null;
            FormBillNameSmall = null;
            FormHistoryName = null;
            FormArabicName = null;
            FormTestGroupId = null;
            FormLogGroup = null;
            FormCollection = null;
            FormTestTimeDays = 0;
            FormArrangeNumber = 0;
            FormPatientPrice = 0;
            FormLabToLabPrice = 0;
            FormIsRoutine = false;
            FormIsSeeReport = false;
            FormIsPrintWithOther = false;
            FormIsAddWithGroup = false;
            FormIsMainTest = false;
            FormParentLabTestId = null;
            FormDefaultSpecimenTypeId = null;
            FormIsSentExternal = false;
            FormExternalReferralId = null;
            FormExternalCost = null;
            FormPromptQuestion = null;
            FormIsActive = true;
            SelectedTest = null;
            SelectedReferralPrice = null;
            NewReferralPrice_ReferralId = null;
            NewReferralPrice_Price = 0;
            ReferralPrices.Clear();
            IsEditMode = false;
            IsAddMode = true;
        }
    }
}
