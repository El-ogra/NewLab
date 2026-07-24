using System;
using System.Collections.Generic;
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
    public partial class NormalRangeViewModel : ObservableObject
    {
        private readonly INormalRangeService _normalRangeService;
        private readonly IDialogService _dialogService;
        private readonly IValidator<NormalRange> _validator;
        private readonly ICurrentUserService _currentUserService;

        public LabTest? ParentTest { get; private set; }
        public ObservableCollection<NormalRange> Ranges { get; } = new();

        [ObservableProperty] private NormalRange? selectedRange;
        [ObservableProperty] private bool isAddMode = true;
        [ObservableProperty] private bool isEditMode;

        // Form fields
        [ObservableProperty] private string formTestName = string.Empty;
        [ObservableProperty] private string? formTestUnit;
        [ObservableProperty] private Gender formGender = Gender.Male;
        [ObservableProperty] private int formAgeFrom;
        [ObservableProperty] private int formAgeTo;
        [ObservableProperty] private AgeUnit formAgeUnit = AgeUnit.Year;
        [ObservableProperty] private string? formNormalRangeText;
        [ObservableProperty] private decimal formLowLimit;
        [ObservableProperty] private decimal formHighLimit;
        [ObservableProperty] private string? formLowFlag;
        [ObservableProperty] private string? formHighFlag;
        [ObservableProperty] private string? formLowComment;
        [ObservableProperty] private string? formHighComment;
        [ObservableProperty] private string? formCriticalComment;
        [ObservableProperty] private string? formCriticalRangeText;
        [ObservableProperty] private decimal? formCriticalLowLimit;
        [ObservableProperty] private decimal? formCriticalHighLimit;
        [ObservableProperty] private string? formCriticalFlag;
        [ObservableProperty] private bool formForPregnancyOnly;

        public IEnumerable<Gender> AvailableGenders => new[] { Gender.Male, Gender.Female };
        public IEnumerable<AgeUnit> AvailableAgeUnits => Enum.GetValues<AgeUnit>();
        public bool IsAdmin => _currentUserService.IsAdmin;

        private int? _editingRangeId;

        public NormalRangeViewModel(
            INormalRangeService normalRangeService,
            IDialogService dialogService,
            IValidator<NormalRange> validator,
            ICurrentUserService currentUserService)
        {
            _normalRangeService = normalRangeService;
            _dialogService = dialogService;
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task LoadForTest(LabTest test)
        {
            ParentTest = test;
            FormTestName = test.TestName;
            FormTestUnit = null;
            await LoadRangesAsync();
        }

        [RelayCommand]
        private void AddRange()
        {
            ClearForm();
            IsAddMode = true;
            IsEditMode = false;
        }

        [RelayCommand]
        private void Edit()
        {
            if (SelectedRange == null) return;
            LoadRangeToForm(SelectedRange);
            IsEditMode = true;
            IsAddMode = false;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (ParentTest == null) return;

            var range = BuildRangeFromForm();
            range.LabTestId = ParentTest.Id;

            var result = _validator.Validate(range);
            if (!result.IsValid)
            {
                _dialogService.ShowMessage("خطأ في التحقق", string.Join("\n", result.Errors.Select(e => e.ErrorMessage)));
                return;
            }

            try
            {
                if (IsEditMode && _editingRangeId != null)
                {
                    range.Id = _editingRangeId.Value;
                    await _normalRangeService.UpdateAsync(range);
                    _dialogService.ShowMessage("نجاح", "تم تحديث المعدل بنجاح");
                }
                else
                {
                    await _normalRangeService.AddAsync(range);
                    _dialogService.ShowMessage("نجاح", "تم إضافة المعدل بنجاح");
                }

                ClearForm();
                await LoadRangesAsync();
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

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteAsync()
        {
            if (SelectedRange == null) return;

            var confirmed = _dialogService.ShowConfirmation("تأكيد الحذف", "هل أنت متأكد من حذف هذا المعدل؟");
            if (!confirmed) return;

            try
            {
                await _normalRangeService.DeleteAsync(SelectedRange.Id);
                _dialogService.ShowMessage("نجاح", "تم حذف المعدل بنجاح");
                ClearForm();
                await LoadRangesAsync();
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
        private void BackToTests()
        {
            // Handled in code-behind via Window.Close()
        }

        [RelayCommand]
        private async Task AddSixRangesWizardAsync()
        {
            if (ParentTest == null) return;

            var dialog = new Views.Windows.NormalRangeSixWizardDialog
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                var ranges = new List<NormalRange>();
                var genders = new[] { Gender.Male, Gender.Female };
                var ageConfigs = new (int From, int To, AgeUnit Unit)[]
                {
                    (0, 120, AgeUnit.Year),
                    (1, 29, AgeUnit.Day),
                    (1, 11, AgeUnit.Month)
                };

                foreach (var gender in genders)
                {
                    foreach (var (from, to, unit) in ageConfigs)
                    {
                        ranges.Add(new NormalRange
                        {
                            LabTestId = ParentTest.Id,
                            TestName = ParentTest.TestName,
                            TestUnit = dialog.Unit,
                            Gender = gender,
                            AgeFrom = from,
                            AgeTo = to,
                            AgeUnit = unit,
                            NormalRangeText = dialog.NormalRangeText,
                            LowLimit = dialog.LowLimit,
                            HighLimit = dialog.HighLimit,
                            LowFlag = dialog.LowFlag,
                            HighFlag = dialog.HighFlag,
                            LowComment = dialog.LowComment,
                            HighComment = dialog.HighComment
                        });
                    }
                }

                foreach (var range in ranges)
                {
                    await _normalRangeService.AddAsync(range);
                }

                await LoadRangesAsync();
                _dialogService.ShowMessage("نجاح", $"تم إنشاء {ranges.Count} معدلات بنجاح");
            }
        }

        partial void OnSelectedRangeChanged(NormalRange? value)
        {
            DeleteCommand.NotifyCanExecuteChanged();
        }

        private async Task LoadRangesAsync()
        {
            Ranges.Clear();
            if (ParentTest == null) return;

            var ranges = await _normalRangeService.GetForTestAsync(ParentTest.Id);
            foreach (var r in ranges)
                Ranges.Add(r);
        }

        private void LoadRangeToForm(NormalRange range)
        {
            _editingRangeId = range.Id;
            FormTestName = range.TestName;
            FormTestUnit = range.TestUnit;
            FormGender = range.Gender;
            FormAgeFrom = range.AgeFrom;
            FormAgeTo = range.AgeTo;
            FormAgeUnit = range.AgeUnit;
            FormNormalRangeText = range.NormalRangeText;
            FormLowLimit = range.LowLimit;
            FormHighLimit = range.HighLimit;
            FormLowFlag = range.LowFlag;
            FormHighFlag = range.HighFlag;
            FormLowComment = range.LowComment;
            FormHighComment = range.HighComment;
            FormCriticalComment = range.CriticalComment;
            FormCriticalRangeText = range.CriticalRangeText;
            FormCriticalLowLimit = range.CriticalLowLimit;
            FormCriticalHighLimit = range.CriticalHighLimit;
            FormCriticalFlag = range.CriticalFlag;
            FormForPregnancyOnly = range.ForPregnancyOnly;
        }

        private NormalRange BuildRangeFromForm()
        {
            return new NormalRange
            {
                TestName = FormTestName,
                TestUnit = FormTestUnit,
                Gender = FormGender,
                AgeFrom = FormAgeFrom,
                AgeTo = FormAgeTo,
                AgeUnit = FormAgeUnit,
                NormalRangeText = FormNormalRangeText,
                LowLimit = FormLowLimit,
                HighLimit = FormHighLimit,
                LowFlag = FormLowFlag,
                HighFlag = FormHighFlag,
                LowComment = FormLowComment,
                HighComment = FormHighComment,
                CriticalComment = FormCriticalComment,
                CriticalRangeText = FormCriticalRangeText,
                CriticalLowLimit = FormCriticalLowLimit,
                CriticalHighLimit = FormCriticalHighLimit,
                CriticalFlag = FormCriticalFlag,
                ForPregnancyOnly = FormForPregnancyOnly
            };
        }

        private void ClearForm()
        {
            _editingRangeId = null;
            FormTestName = ParentTest?.TestName ?? string.Empty;
            FormTestUnit = null;
            FormGender = Gender.Male;
            FormAgeFrom = 0;
            FormAgeTo = 120;
            FormAgeUnit = AgeUnit.Year;
            FormNormalRangeText = null;
            FormLowLimit = 0;
            FormHighLimit = 0;
            FormLowFlag = null;
            FormHighFlag = null;
            FormLowComment = null;
            FormHighComment = null;
            FormCriticalComment = null;
            FormCriticalRangeText = null;
            FormCriticalLowLimit = null;
            FormCriticalHighLimit = null;
            FormCriticalFlag = null;
            FormForPregnancyOnly = false;
            SelectedRange = null;
            IsEditMode = false;
            IsAddMode = true;
        }
    }
}
