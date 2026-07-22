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
        [ObservableProperty] private string patientName = string.Empty;
        [ObservableProperty] private string patientLabId = string.Empty;
        [ObservableProperty] private string testTitle = string.Empty;

        public TestResultEntryViewModel(
            ITestResultEntryService entryService,
            IAutoCalculationService autoCalcService,
            IDialogService dialogService,
            IValidator<TestResult> validator,
            ICurrentUserService currentUserService,
            Func<CalculationConstantsViewModel> constantsVmFactory)
        {
            _entryService = entryService;
            _autoCalcService = autoCalcService;
            _dialogService = dialogService;
            _validator = validator;
            _currentUserService = currentUserService;
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
            TestTitle = _parentTest.TestName;

            ResultRows.Clear();
            foreach (var element in data.elements)
            {
                var existing = data.existingResults.FirstOrDefault(r => r.LabTestElementId == element.Id);
                var row = new TestResultRow(element.Id, element.Name)
                {
                    Value = existing?.Value,
                    Unit = existing?.Unit ?? "",
                    FlagText = existing?.FlagText,
                    CellColor = existing?.IsCritical == true ? "#FFF44336" :
                                existing?.IsAbnormal == true ? "#FFFF9800" : "Transparent"
                };
                ResultRows.Add(row);
            }

            Comment = data.existingResults.FirstOrDefault()?.Comment;
            IsReviewed = data.patientTest.IsReviewed;

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
                await _entryService.MarkReviewedAsync(_patientTestId);
                IsReviewed = true;
                _dialogService.ShowMessage("نجاح", "تم وضع علامة مراجعة");
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
                Comment = string.IsNullOrEmpty(Comment)
                    ? savedComment.CommentText
                    : Comment + "\n" + savedComment.CommentText;
            }
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
                    FlagText = row.FlagText,
                    Comment = Comment
                });
            }
            return results;
        }
    }

    public partial class TestResultRow : ObservableObject
    {
        public int LabTestElementId { get; }
        public string Name { get; }

        [ObservableProperty] private string? value;
        [ObservableProperty] private string? unit;
        [ObservableProperty] private string? flagText;
        [ObservableProperty] private string cellColor = "Transparent";

        public TestResultRow(int labTestElementId, string name)
        {
            LabTestElementId = labTestElementId;
            Name = name;
        }
    }
}
