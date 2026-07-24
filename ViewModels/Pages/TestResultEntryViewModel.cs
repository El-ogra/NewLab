using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;
using NewLab.Services.Interfaces;

namespace NewLab.ViewModels.Pages
{
    public partial class TestResultEntryViewModel : ObservableObject
    {
        private readonly ITestResultEntryService _entryService;
        private readonly IAutoCalculationService _autoCalcService;
        private readonly IDialogService _dialogService;
        private readonly IValidator<TestResult> _validator;
        private readonly ICurrentUserService _currentUserService;
        private readonly INormalRangeService _normalRangeService;
        private readonly Func<CalculationConstantsViewModel> _constantsVmFactory;

        private Patient? _patient;
        private LabTest? _parentTest;
        private int _patientTestId;

        [ObservableProperty] private ObservableCollection<TestResultRow> resultRows = new();
        [ObservableProperty] private string? comment;
        [ObservableProperty] private bool isReviewed;
        [ObservableProperty] private string historyButtonLabel = "تاريخ مرضي";
        [ObservableProperty] private ObservableCollection<CalculationConstant> constants = new();
        [ObservableProperty] private ObservableCollection<SavedComment> savedComments = new();
        [ObservableProperty] private TestResultRow? selectedResultRow;
        [ObservableProperty] private string patientName = string.Empty;
        [ObservableProperty] private string patientLabId = string.Empty;
        [ObservableProperty] private string patientGender = string.Empty;
        [ObservableProperty] private string patientAge = string.Empty;
        [ObservableProperty] private DateTime visitDate;
        [ObservableProperty] private string testTitle = string.Empty;

        private readonly Stack<string> _commentHistory = new();

        public TestResultEntryViewModel(
            ITestResultEntryService entryService,
            IAutoCalculationService autoCalcService,
            IDialogService dialogService,
            IValidator<TestResult> validator,
            ICurrentUserService currentUserService,
            INormalRangeService normalRangeService,
            Func<CalculationConstantsViewModel> constantsVmFactory)
        {
            _entryService = entryService;
            _autoCalcService = autoCalcService;
            _dialogService = dialogService;
            _validator = validator;
            _currentUserService = currentUserService;
            _normalRangeService = normalRangeService;
            _constantsVmFactory = constantsVmFactory;
        }

        public async Task LoadForPatientTestAsync(int patientTestId)
        {
            _patientTestId = patientTestId;
            var data = await _entryService.GetPatientTestWithProfileAsync(patientTestId);
            _patient = data.patientTest.Visit.Patient;
            _parentTest = data.labTest;

            PatientName = _patient.FullName;
            PatientLabId = _patient.LabId ?? "";
            PatientGender = _patient.Gender.ToString();
            PatientAge = $"{_patient.AgeValue} {_patient.AgeUnit}";
            VisitDate = data.patientTest.Visit.VisitDate;
            TestTitle = _parentTest.TestName;

            ResultRows.Clear();
            foreach (var element in data.elements)
            {
                var existing = data.existingResults.FirstOrDefault(r => r.LabTestElementId == element.Id);
                var range = await _normalRangeService.GetMatchingRangeAsync(element.Id, _patient!);
                var rangeText = range != null ? $"{range.LowLimit} - {range.HighLimit} {range.TestUnit}" : "";
                var row = new TestResultRow(element.Id, element.Name)
                {
                    Value = existing?.Value,
                    Unit = existing?.Unit ?? "",
                    FlagText = existing?.FlagText,
                    CellColor = existing?.IsCritical == true ? "#FFF44336" :
                                existing?.IsAbnormal == true ? "#FFFF9800" : "Transparent",
                    NormalRangeText = rangeText,
                    IsReviewed = existing?.IsReviewed == true,
                    IsPrinted = existing?.IsPrinted == true
                };
                row.ValueChangedCallback = async _ => await OnElementValueChangedAsync(row);
                ResultRows.Add(row);
            }

            Comment = data.existingResults.FirstOrDefault()?.Comment;
            IsReviewed = data.patientTest.IsReviewed;

            var resultCount = data.existingResults.Count;
            HistoryButtonLabel = resultCount > 0 ? $"تاريخ مرضي ({resultCount})" : "تاريخ مرضي";

            var savedComments = await _entryService.GetSavedCommentsAsync(_parentTest.Id);
            SavedComments.Clear();
            foreach (var sc in savedComments)
                SavedComments.Add(sc);

            var constants = await _autoCalcService.GetConstantsAsync();
            Constants.Clear();
            foreach (var c in constants)
                Constants.Add(c);
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            var results = BuildResults();
            var validationResults = results.Select(r => _validator.Validate(r));
            var errors = validationResults.SelectMany(v => v.Errors).ToList();
            if (errors.Any())
            {
                _dialogService.ShowMessage("خطأ في التحقق", string.Join("\n", errors.Select(e => e.ErrorMessage)));
                return;
            }

            try
            {
                await _entryService.SaveResultsAsync(_patientTestId, results, Comment);
                _dialogService.ShowMessage("نجاح", "تم حفظ النتائج بنجاح");
                Application.Current.Windows.OfType<Views.Windows.TestResultEntryView>().FirstOrDefault()?.Close();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", ex.Message);
            }
        }

        [RelayCommand]
        private async Task PrintAsync()
        {
            try
            {
                var pdf = await _entryService.PrintReportAsync(_patientTestId);
                var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"Report_{_patientTestId}.pdf");
                await System.IO.File.WriteAllBytesAsync(tempPath, pdf);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true
                });
                _dialogService.ShowMessage("نجاح", "تم طباعة التقرير");
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", ex.Message);
            }
        }

        [RelayCommand]
        private async Task PreviewAsync()
        {
            try
            {
                var pdf = await _entryService.PreviewReportAsync(_patientTestId);
                var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"Preview_{_patientTestId}.pdf");
                await System.IO.File.WriteAllBytesAsync(tempPath, pdf);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", ex.Message);
            }
        }

        [RelayCommand]
        private async Task ToggleReviewedAsync()
        {
            try
            {
                var newState = !IsReviewed;
                await _entryService.MarkReviewedAsync(_patientTestId, newState);
                IsReviewed = newState;
                _dialogService.ShowMessage("نجاح", newState ? "تم وضع علامة مراجعة" : "تم إزالة علامة المراجعة");
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", ex.Message);
            }
        }

        [RelayCommand]
        private void OpenHistory()
        {
            _dialogService.ShowMessage("Info", "تاريخ مرضي مخصص غير متاح حالياً");
        }

        [RelayCommand]
        private void Back()
        {
            Application.Current.Windows.OfType<Views.Windows.TestResultEntryView>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private void MainMenu()
        {
            Application.Current.Windows.OfType<Views.Windows.TestResultEntryView>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private void EditConstants()
        {
            var vm = _constantsVmFactory();
            var window = new Views.Windows.CalculationConstantsView
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();
        }

        [RelayCommand]
        private void PickSavedComment(SavedComment? savedComment)
        {
            if (savedComment != null)
            {
                _commentHistory.Push(Comment ?? string.Empty);
                Comment = string.IsNullOrEmpty(Comment)
                    ? savedComment.CommentText
                    : Comment + "\n" + savedComment.CommentText;
            }
        }

        [RelayCommand]
        private async Task PickCommentFromNormalRangeAsync()
        {
            if (SelectedResultRow == null || string.IsNullOrWhiteSpace(SelectedResultRow.Value)) return;
            if (!decimal.TryParse(SelectedResultRow.Value, out var numericValue)) return;
            if (_patient == null) return;

            var range = await _normalRangeService.GetMatchingRangeAsync(SelectedResultRow.LabTestElementId, _patient);
            if (range == null) return;

            var eval = await _normalRangeService.EvaluateValueAsync(range, numericValue);
            var commentToAdd = eval.Category switch
            {
                "CriticalLow" or "CriticalHigh" => range.CriticalComment,
                "AbnormalLow" or "AbnormalHigh" => eval.Category.Contains("Low") ? range.LowComment : range.HighComment,
                _ => null
            };

            if (!string.IsNullOrWhiteSpace(commentToAdd))
            {
                _commentHistory.Push(Comment ?? string.Empty);
                Comment = string.IsNullOrEmpty(Comment)
                    ? commentToAdd
                    : Comment + "\n" + commentToAdd;
            }
        }

        [RelayCommand]
        private void UndoLastComment()
        {
            if (_commentHistory.Count > 0)
                Comment = _commentHistory.Pop();
        }

        private List<TestResult> BuildResults()
        {
            var results = new List<TestResult>();
            foreach (var row in ResultRows)
            {
                if (string.IsNullOrWhiteSpace(row.Value)) continue;

                results.Add(new TestResult
                {
                    PatientTestId = _patientTestId,
                    LabTestElementId = row.LabTestElementId,
                    Value = row.Value,
                    Unit = row.Unit,
                    IsAbnormal = row.CellColor != "Transparent",
                    IsReviewed = row.IsReviewed,
                    IsPrinted = row.IsPrinted,
                    FlagText = row.FlagText,
                    Comment = Comment
                });
            }
            return results;
        }

        private async Task OnElementValueChangedAsync(TestResultRow changedRow)
        {
            if (string.IsNullOrWhiteSpace(changedRow.Value)) return;
            if (!decimal.TryParse(changedRow.Value, out var numericValue)) return;

            try
            {
                // Real-time normal range evaluation
                if (_patient != null)
                {
                    var range = await _normalRangeService.GetMatchingRangeAsync(changedRow.LabTestElementId, _patient);
                    if (range != null)
                    {
                        var eval = await _normalRangeService.EvaluateValueAsync(range, numericValue);
                        changedRow.FlagText = eval.Flag;
                        changedRow.CellColor = eval.Category switch
                        {
                            "CriticalLow" or "CriticalHigh" => "#FFF44336",
                            "AbnormalLow" or "AbnormalHigh" => "#FFF9800",
                            _ => "Transparent"
                        };
                    }
                    else
                    {
                        changedRow.FlagText = null;
                        changedRow.CellColor = "Transparent";
                    }
                }

                // Auto-calculation
                switch (changedRow.Name)
                {
                    case "Hgb":
                        var hctRow = ResultRows.FirstOrDefault(r => r.Name == "Hct");
                        if (hctRow != null)
                        {
                            var hct = await _autoCalcService.CalculateHctAsync(numericValue);
                            hctRow.Value = hct.ToString("F1");
                        }
                        break;

                    case "PT":
                        var inrRow = ResultRows.FirstOrDefault(r => r.Name == "INR");
                        if (inrRow != null)
                        {
                            var inr = await _autoCalcService.CalculateINRAsync(numericValue);
                            inrRow.Value = inr.ToString("F2");
                        }
                        break;

                    case "PTT":
                        var pttRatioRow = ResultRows.FirstOrDefault(r => r.Name == "PTT Ratio");
                        if (pttRatioRow != null)
                        {
                            var pttRatio = await _autoCalcService.CalculatePTTRatioAsync(numericValue);
                            pttRatioRow.Value = pttRatio.ToString("F2");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ في الحساب التلقائي", ex.Message);
            }
        }
    }

    public partial class TestResultRow : ObservableObject
    {
        public int LabTestElementId { get; }
        public string Name { get; }
        public Action<string?>? ValueChangedCallback { get; set; }

        [ObservableProperty] private string? value;
        [ObservableProperty] private string? unit;
        [ObservableProperty] private string? flagText;
        [ObservableProperty] private string cellColor = "Transparent";
        [ObservableProperty] private string? normalRangeText;
        [ObservableProperty] private bool isEnabled = true;
        [ObservableProperty] private bool isReviewed;
        [ObservableProperty] private bool isPrinted;

        public TestResultRow(int labTestElementId, string name)
        {
            LabTestElementId = labTestElementId;
            Name = name;
        }

        partial void OnValueChanged(string? value)
        {
            ValueChangedCallback?.Invoke(value);
        }
    }
}
