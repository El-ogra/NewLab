using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;
using NewLab.Services.Interfaces;

namespace NewLab.ViewModels.Pages
{
    public partial class PatientEntryViewModel : ObservableObject
    {
        private readonly IPatientService _patientService;
        private readonly IReferralService _referralService;
        private readonly ILabTestService _labTestService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IValidator<Patient> _patientValidator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBarcodeService _barcodeService;
        private readonly IReceiptPdfService _receiptPdfService;
        private readonly IReceiptSettingsService _receiptSettingsService;
        private readonly Func<BarcodeViewModel> _barcodeViewModelFactory;

        private DateTime _lastEnterAt = DateTime.MinValue;

        // Patient fields
        [ObservableProperty] private string fullName = string.Empty;
        [ObservableProperty] private string? title;
        [ObservableProperty] private Gender gender = Gender.Male;
        [ObservableProperty] private decimal ageValue;
        [ObservableProperty] private AgeUnit ageUnit = AgeUnit.Year;
        [ObservableProperty] private BillingSystem billingSystem = BillingSystem.Individual;
        [ObservableProperty] private bool isImportant;
        [ObservableProperty] private int? referralId;
        [ObservableProperty] private bool referralHiddenOnReport;
        [ObservableProperty] private string? phoneNumber;
        [ObservableProperty] private string? nationalId;
        [ObservableProperty] private string? labId;
        [ObservableProperty] private string? fileCode;
        [ObservableProperty] private string? visitCode;
        [ObservableProperty] private string? notes;
        [ObservableProperty] private int? externalSpecimenTypeId;
        [ObservableProperty] private decimal discountValue;
        [ObservableProperty] private bool discountIsPercent;
        [ObservableProperty] private decimal paidAmount;
        [ObservableProperty] private decimal totalAmount;

        // Boolean medical fields (Decision 1)
        [ObservableProperty] private bool isFasting;
        [ObservableProperty] private int? fastingHours;
        [ObservableProperty] private bool isOnAnticoagulant;
        [ObservableProperty] private bool hasLiverTreatment;
        [ObservableProperty] private bool hasAntiviralTreatment;
        [ObservableProperty] private bool hasAntibiotic;
        [ObservableProperty] private bool isPregnant;
        [ObservableProperty] private bool isSmoker;

        // Screen state
        [ObservableProperty] private bool isEditMode;
        [ObservableProperty] private bool isAddMode = true;
        [ObservableProperty] private string searchText = string.Empty;
        [ObservableProperty] private decimal remaining;
        [ObservableProperty] private string selectedReferralName = string.Empty;

        // Collections
        public ObservableCollection<PatientTestRow> SelectedTests { get; } = new();
        public ObservableCollection<LabTest> AvailableTests { get; } = new();
        public ObservableCollection<Referral> ReferralSuggestions { get; } = new();
        public ObservableCollection<SpecimenType> AvailableSpecimenTypes { get; } = new();

        // Permissions (Decision 2)
        public bool IsAdmin => _currentUserService.IsAdmin;
        public TestListMode[] AvailableTestListModes => Enum.GetValues<TestListMode>();

        [ObservableProperty] private LabTest? selectedAvailableTest;
        [ObservableProperty] private TestListMode selectedTestListMode = TestListMode.Routine;
        [ObservableProperty] private string testListFilter = string.Empty;
        [ObservableProperty] private ObservableCollection<Patient> todayPatients = new();

        private int? _editingPatientId;

        public PatientEntryViewModel(
            IPatientService patientService,
            IReferralService referralService,
            ILabTestService labTestService,
            IDialogService dialogService,
            INavigationService navigationService,
            IValidator<Patient> patientValidator,
            ICurrentUserService currentUserService,
            IBarcodeService barcodeService,
            IReceiptPdfService receiptPdfService,
            IReceiptSettingsService receiptSettingsService,
            Func<BarcodeViewModel> barcodeViewModelFactory)
        {
            _patientService = patientService;
            _referralService = referralService;
            _labTestService = labTestService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _patientValidator = patientValidator;
            _currentUserService = currentUserService;
            _barcodeService = barcodeService;
            _receiptPdfService = receiptPdfService;
            _receiptSettingsService = receiptSettingsService;
            _barcodeViewModelFactory = barcodeViewModelFactory;

            _ = LoadAvailableTestsAsync();
            _ = LoadSpecimenTypesAsync();
        }

        partial void OnGenderChanged(Gender value)
        {
            Title = value == Gender.Male ? "السيد" : "السيدة";
        }

        private async Task LoadSpecimenTypesAsync()
        {
            var types = await _labTestService.GetSpecimenTypesAsync();
            AvailableSpecimenTypes.Clear();
            foreach (var type in types)
                AvailableSpecimenTypes.Add(type);
        }

        [RelayCommand]
        private void ShowTestInfo()
        {
            if (SelectedAvailableTest == null)
            {
                _dialogService.ShowMessage("خطأ", "اختر تحليلاً أولاً");
                return;
            }
            _dialogService.ShowMessage("بيانات التحليل",
                $"الكود: {SelectedAvailableTest.Code}\n" +
                $"الاسم: {SelectedAvailableTest.TestName}\n" +
                $"سعر المريض: {SelectedAvailableTest.PatientPrice}\n" +
                $"سعر المعمل للمعمل: {SelectedAvailableTest.LabToLabPrice}");
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadAvailableTestsAsync();
        }

        [RelayCommand]
        private void AddPatient()
        {
            ClearForm();
            IsAddMode = true;
            IsEditMode = false;
        }

        [RelayCommand]
        private async Task EditAsync()
        {
            if (_editingPatientId == null) return;

            var patient = await _patientService.GetByIdAsync(_editingPatientId.Value);
            if (patient == null)
            {
                _dialogService.ShowMessage("خطأ", "لم يتم العثور على المريض");
                return;
            }

            LoadPatientToForm(patient);
            IsEditMode = true;
            IsAddMode = false;
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteAsync()
        {
            if (_editingPatientId == null) return;

            var confirmed = _dialogService.ShowConfirmation("تأكيد الحذف", "هل أنت متأكد من حذف هذا المريض؟");
            if (!confirmed) return;

            try
            {
                await _patientService.DeleteAsync(_editingPatientId.Value);
                _dialogService.ShowMessage("نجاح", "تم حذف المريض بنجاح");
                ClearForm();
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
        private void Cancel()
        {
            ClearForm();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            // Auto-assign default referral if none selected
            if (ReferralId == null && BillingSystem == BillingSystem.LabToLab && string.IsNullOrWhiteSpace(SelectedReferralName))
            {
                var defaultLab = await _referralService.GetDefaultLabAsync();
                ReferralId = defaultLab.Id;
            }
            else if (ReferralId == null && BillingSystem == BillingSystem.LabToLab && !string.IsNullOrWhiteSpace(SelectedReferralName))
            {
                var referral = await _referralService.GetOrCreateAsync(SelectedReferralName);
                ReferralId = referral.Id;
            }

            var patient = BuildPatientFromForm();

            var result = _patientValidator.Validate(patient);
            if (!result.IsValid)
            {
                _dialogService.ShowMessage("خطأ في التحقق", string.Join("\n", result.Errors.Select(e => e.ErrorMessage)));
                return;
            }

            try
            {
                if (IsEditMode && _editingPatientId != null)
                {
                    patient.Id = _editingPatientId.Value;
                    await _patientService.UpdateAsync(patient);
                    _dialogService.ShowMessage("نجاح", "تم تحديث بيانات المريض بنجاح");
                }
                else
                {
                    patient.CreatedByUserId = 1;
                    await _patientService.AddAsync(patient);
                    _dialogService.ShowMessage("نجاح", "تم إضافة المريض بنجاح");
                }

                ClearForm();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", "حدث خطأ أثناء الحفظ: " + ex.Message);
            }
        }

        [RelayCommand]
        private async Task ConfirmAsync()
        {
            await SaveAsync();
        }

        [RelayCommand]
        private async Task KeyboardEnterAsync()
        {
            var now = DateTime.Now;
            if ((now - _lastEnterAt).TotalMilliseconds < 500)
            {
                await ConfirmAsync();
                _lastEnterAt = DateTime.MinValue;
            }
            else
            {
                _lastEnterAt = now;
            }
        }

        [RelayCommand]
        private async Task PrintReceiptAsync()
        {
            if (_editingPatientId == null && string.IsNullOrWhiteSpace(FullName))
            {
                _dialogService.ShowMessage("خطأ", "يرجى حفظ المريض أولاً");
                return;
            }

            Patient? patient;
            if (_editingPatientId != null)
            {
                patient = await _patientService.GetByIdAsync(_editingPatientId.Value);
                if (patient == null)
                {
                    _dialogService.ShowMessage("خطأ", "لم يتم العثور على المريض");
                    return;
                }
            }
            else
            {
                patient = BuildPatientFromForm();
            }

            var settings = await _receiptSettingsService.GetAsync();
            var pdfBytes = await _receiptPdfService.GenerateReceiptAsync(patient, SelectedTests.ToList(), settings);
            var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"Receipt_{patient.FullName}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            await System.IO.File.WriteAllBytesAsync(tempPath, pdfBytes);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = tempPath,
                UseShellExecute = true
            });
        }

        [RelayCommand]
        private async Task PrintBarcodeAsync()
        {
            if (_editingPatientId == null && string.IsNullOrWhiteSpace(FullName))
            {
                _dialogService.ShowMessage("خطأ", "يرجى حفظ المريض أولاً");
                return;
            }

            Patient? patient;
            if (_editingPatientId != null)
            {
                patient = await _patientService.GetByIdAsync(_editingPatientId.Value);
                if (patient == null)
                {
                    _dialogService.ShowMessage("خطأ", "لم يتم العثور على المريض");
                    return;
                }
            }
            else
            {
                patient = BuildPatientFromForm();
            }

            var vm = _barcodeViewModelFactory();
            await vm.LoadForPatientAsync(patient);

            var window = new NewLab.Views.Windows.BarcodeView
            {
                DataContext = vm,
                Owner = System.Windows.Application.Current.MainWindow
            };
            window.ShowDialog();
        }

        [RelayCommand]
        private void AddSelectedTest()
        {
            if (SelectedAvailableTest == null) return;

            decimal price = BillingSystem switch
            {
                BillingSystem.Individual => SelectedAvailableTest.PatientPrice,
                BillingSystem.LabToLab => SelectedAvailableTest.LabToLabPrice,
                BillingSystem.Free => 0m,
                _ => SelectedAvailableTest.PatientPrice
            };

            var row = new PatientTestRow(SelectedAvailableTest.Id, SelectedAvailableTest.TestName, price);
            SelectedTests.Add(row);
            _ = RecalculateTotalAsync();
        }

        [RelayCommand]
        private void RemoveTest(LabTest? test)
        {
            if (test == null) return;
            var row = SelectedTests.FirstOrDefault(t => t.LabTestId == test.Id);
            if (row != null)
            {
                SelectedTests.Remove(row);
            }
        }

        [RelayCommand]
        private void RemoveAllTests()
        {
            SelectedTests.Clear();
        }

        [RelayCommand]
        private void MarkAsPaid()
        {
            PaidAmount = TotalAmount;
            Remaining = TotalAmount - PaidAmount;
        }

        [RelayCommand]
        private async Task TodayPatientsAsync()
        {
            var patients = await _patientService.GetTodayPatientsAsync();
            TodayPatients.Clear();
            foreach (var p in patients)
                TodayPatients.Add(p);
        }

        [RelayCommand]
        private void SelectTodayPatient(Patient? patient)
        {
            if (patient == null) return;
            _editingPatientId = patient.Id;
            LoadPatientToForm(patient);
            IsEditMode = true;
            IsAddMode = false;
        }

        [RelayCommand]
        private async Task LookupLabIdAsync()
        {
            if (_editingPatientId == null)
            {
                _dialogService.ShowMessage("خطأ", "يرجى حفظ المريض أولاً");
                return;
            }

            var patient = await _patientService.GetByIdAsync(_editingPatientId.Value);
            if (patient == null)
            {
                _dialogService.ShowMessage("خطأ", "لم يتم العثور على المريض");
                return;
            }

            if (!string.IsNullOrEmpty(patient.LabId))
            {
                LabId = patient.LabId;
                _dialogService.ShowMessage("معلومة", $"Lab ID موجود مسبقاً: {patient.LabId}");
                return;
            }

            var updated = await _barcodeService.GetOrCreateLabIdAsync(patient);
            LabId = updated.LabId;
            _dialogService.ShowMessage("نجاح", $"تم إنشاء Lab ID: {updated.LabId}");
        }

        [RelayCommand]
        private async Task SearchReferralAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedReferralName) || SelectedReferralName.Length < 2)
            {
                ReferralSuggestions.Clear();
                return;
            }

            var results = await _referralService.SearchByNameAsync(SelectedReferralName);
            ReferralSuggestions.Clear();
            foreach (var r in results)
            {
                ReferralSuggestions.Add(r);
            }
        }

        partial void OnBillingSystemChanged(BillingSystem value)
        {
            _ = RecalculateTotalAsync();
        }

        partial void OnReferralIdChanged(int? value)
        {
            _ = RecalculateTotalAsync();
        }

        partial void OnPaidAmountChanged(decimal value)
        {
            Remaining = TotalAmount - value;
        }

        private async Task RecalculateTotalAsync()
        {
            if (!SelectedTests.Any()) return;

            Referral? referral = null;
            if (ReferralId != null)
            {
                referral = ReferralSuggestions.FirstOrDefault(r => r.Id == ReferralId);
            }

            TotalAmount = await _patientService.CalculateTotalAsync(
                SelectedTests, BillingSystem, referral, DiscountValue, DiscountIsPercent);
            Remaining = TotalAmount - PaidAmount;
        }

        private async Task LoadAvailableTestsAsync()
        {
            var tests = SelectedTestListMode switch
            {
                TestListMode.Routine => await _labTestService.GetRoutineTestsAsync(),
                TestListMode.All => await _labTestService.GetAllAsync(),
                _ => await _labTestService.GetRoutineTestsAsync()
            };

            if (!string.IsNullOrWhiteSpace(TestListFilter))
            {
                tests = tests.Where(t => t.TestName.Contains(TestListFilter) || t.Code.Contains(TestListFilter)).ToList();
            }

            AvailableTests.Clear();
            foreach (var t in tests)
                AvailableTests.Add(t);
        }

        partial void OnSelectedTestListModeChanged(TestListMode value)
        {
            _ = LoadAvailableTestsAsync();
        }

        partial void OnTestListFilterChanged(string value)
        {
            _ = LoadAvailableTestsAsync();
        }

        private Patient BuildPatientFromForm()
        {
            return new Patient
            {
                FullName = FullName,
                Title = Title,
                Gender = Gender,
                AgeValue = AgeValue,
                AgeUnit = AgeUnit,
                BillingSystem = BillingSystem,
                IsImportant = IsImportant,
                ReferralId = ReferralId,
                ReferralHiddenOnReport = ReferralHiddenOnReport,
                PhoneNumber = PhoneNumber,
                NationalId = NationalId,
                LabId = LabId,
                FileCode = FileCode,
                VisitCode = VisitCode,
                Notes = Notes,
                ExternalSpecimenTypeId = ExternalSpecimenTypeId,
                DiscountValue = DiscountValue,
                DiscountIsPercent = DiscountIsPercent,
                PaidAmount = PaidAmount,
                TotalAmount = TotalAmount,
                IsFasting = IsFasting,
                FastingHours = FastingHours,
                IsOnAnticoagulant = IsOnAnticoagulant,
                HasLiverTreatment = HasLiverTreatment,
                HasAntiviralTreatment = HasAntiviralTreatment,
                HasAntibiotic = HasAntibiotic,
                IsPregnant = IsPregnant,
                IsSmoker = IsSmoker
            };
        }

        private void LoadPatientToForm(Patient patient)
        {
            _editingPatientId = patient.Id;
            FullName = patient.FullName;
            Title = patient.Title;
            Gender = patient.Gender;
            AgeValue = patient.AgeValue;
            AgeUnit = patient.AgeUnit;
            BillingSystem = patient.BillingSystem;
            IsImportant = patient.IsImportant;
            ReferralId = patient.ReferralId;
            ReferralHiddenOnReport = patient.ReferralHiddenOnReport;
            PhoneNumber = patient.PhoneNumber;
            NationalId = patient.NationalId;
            LabId = patient.LabId;
            FileCode = patient.FileCode;
            VisitCode = patient.VisitCode;
            Notes = patient.Notes;
            ExternalSpecimenTypeId = patient.ExternalSpecimenTypeId;
            DiscountValue = patient.DiscountValue;
            DiscountIsPercent = patient.DiscountIsPercent;
            PaidAmount = patient.PaidAmount;
            TotalAmount = patient.TotalAmount;
            IsFasting = patient.IsFasting;
            FastingHours = patient.FastingHours;
            IsOnAnticoagulant = patient.IsOnAnticoagulant;
            HasLiverTreatment = patient.HasLiverTreatment;
            HasAntiviralTreatment = patient.HasAntiviralTreatment;
            HasAntibiotic = patient.HasAntibiotic;
            IsPregnant = patient.IsPregnant;
            IsSmoker = patient.IsSmoker;

            if (patient.Referral != null)
            {
                SelectedReferralName = patient.Referral.Name;
                ReferralId = patient.ReferralId;
            }

            Remaining = TotalAmount - PaidAmount;
        }

        private void ClearForm()
        {
            _editingPatientId = null;
            FullName = string.Empty;
            Title = null;
            Gender = Gender.Male;
            AgeValue = 0;
            AgeUnit = AgeUnit.Year;
            BillingSystem = BillingSystem.Individual;
            IsImportant = false;
            ReferralId = null;
            ReferralHiddenOnReport = false;
            PhoneNumber = null;
            NationalId = null;
            LabId = null;
            FileCode = null;
            VisitCode = null;
            Notes = null;
            ExternalSpecimenTypeId = null;
            DiscountValue = 0;
            DiscountIsPercent = false;
            PaidAmount = 0;
            TotalAmount = 0;
            IsFasting = false;
            FastingHours = null;
            IsOnAnticoagulant = false;
            HasLiverTreatment = false;
            HasAntiviralTreatment = false;
            HasAntibiotic = false;
            IsPregnant = false;
            IsSmoker = false;
            SelectedReferralName = string.Empty;
            SelectedTests.Clear();
            AvailableTests.Clear();
            ReferralSuggestions.Clear();
            IsEditMode = false;
            IsAddMode = true;
        }
    }
}
