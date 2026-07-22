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
    public partial class TestResultsListViewModel : ObservableObject
    {
        private readonly ITestResultsListService _testResultsListService;
        private readonly IPatientService _patientService;
        private readonly ILabTestService _labTestService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly Func<TestResultEntryViewModel> _entryVmFactory;

        [ObservableProperty] private ObservableCollection<PatientListItem> patients = new();
        [ObservableProperty] private PatientListItem? selectedPatient;
        [ObservableProperty] private ObservableCollection<PatientTest> patientTests = new();
        [ObservableProperty] private PatientTest? selectedTest;
        [ObservableProperty] private string searchCode = string.Empty;
        [ObservableProperty] private int? searchAttendanceNumber;
        [ObservableProperty] private string filterMode = "All";
        [ObservableProperty] private DateTime selectedDate = DateTime.Today;
        [ObservableProperty] private string noteText = string.Empty;

        public bool IsAdmin => _currentUserService.IsAdmin;

        public TestResultsListViewModel(
            ITestResultsListService testResultsListService,
            IPatientService patientService,
            ILabTestService labTestService,
            IDialogService dialogService,
            INavigationService navigationService,
            ICurrentUserService currentUserService,
            Func<TestResultEntryViewModel> entryVmFactory)
        {
            _testResultsListService = testResultsListService;
            _patientService = patientService;
            _labTestService = labTestService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _currentUserService = currentUserService;
            _entryVmFactory = entryVmFactory;

            _ = LoadTodayPatientsAsync();
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadTodayPatientsAsync();
        }

        [RelayCommand]
        private void OpenPatientData()
        {
            _navigationService.NavigateTo<PatientEntryViewModel>();
        }

        [RelayCommand]
        private void OpenSearch()
        {
            _navigationService.NavigateTo<SearchViewModel>();
        }

        [RelayCommand]
        private async Task OpenTestEntryAsync()
        {
            if (SelectedTest == null)
            {
                _dialogService.ShowMessage("خطأ", "اختر تحليلاً أولاً");
                return;
            }
            var vm = _entryVmFactory();
            await vm.LoadForPatientTestAsync(SelectedTest.Id);
            var window = new Views.Windows.TestResultEntryView
            {
                DataContext = vm,
                Owner = System.Windows.Application.Current.MainWindow
            };
            window.ShowDialog();
            await LoadPatientTestsAsync(SelectedPatient?.PatientId ?? 0);
        }

        [RelayCommand]
        private void OpenDelivery()
        {
            _navigationService.NavigateTo<DeliveryViewModel>();
        }

        [RelayCommand]
        private async Task ToggleReviewedAsync()
        {
            if (SelectedTest == null) return;
            await _testResultsListService.ToggleReviewedAsync(SelectedTest.Id);
            await RefreshAsync();
        }

        [RelayCommand]
        private async Task ToggleEnteredAsync()
        {
            if (SelectedTest == null) return;
            await _testResultsListService.ToggleEnteredAsync(SelectedTest.Id);
            await RefreshAsync();
        }

        [RelayCommand]
        private async Task TogglePrintedAsync()
        {
            if (SelectedTest == null) return;
            await _testResultsListService.TogglePrintedAsync(SelectedTest.Id);
            await RefreshAsync();
        }

        [RelayCommand]
        private async Task UpdateNoteAsync()
        {
            if (SelectedPatient == null) return;
            await _testResultsListService.UpdatePatientNoteAsync(SelectedPatient.PatientId, NoteText);
            _dialogService.ShowMessage("نجاح", "تم حفظ الملاحظات");
        }

        [RelayCommand(CanExecute = nameof(CanShowAudit))]
        private void ShowAudit()
        {
            _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Phase 10");
        }

        private bool CanShowAudit() => IsAdmin;

        [RelayCommand(CanExecute = nameof(CanShowFinancial))]
        private void ShowFinancialTracking()
        {
            _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Phase 10");
        }

        private bool CanShowFinancial() => IsAdmin;

        [RelayCommand]
        private async Task SearchAsync()
        {
            if (SearchAttendanceNumber.HasValue)
            {
                var result = await _testResultsListService.SearchByAttendanceNumberAsync(SearchAttendanceNumber.Value, SelectedDate);
                if (result != null)
                {
                    Patients.Clear();
                    Patients.Add(result);
                    SelectedPatient = result;
                }
                else
                {
                    _dialogService.ShowMessage("خطأ", "لم يتم العثور على مريض بهذا الرقم");
                }
            }
            else if (!string.IsNullOrWhiteSpace(SearchCode))
            {
                var result = await _testResultsListService.SearchByCodeAsync(SearchCode);
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
        }

        [RelayCommand]
        private async Task PreviousDayAsync()
        {
            SelectedDate = SelectedDate.AddDays(-1);
            await RefreshAsync();
        }

        [RelayCommand]
        private async Task NextDayAsync()
        {
            SelectedDate = SelectedDate.AddDays(1);
            await RefreshAsync();
        }

        [RelayCommand]
        private void PrintAggregateReport()
        {
            _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 4");
        }

        [RelayCommand]
        private void PrintWorksheet()
        {
            _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 4");
        }

        [RelayCommand]
        private void PrintEnvelope()
        {
            _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 4");
        }

        [RelayCommand]
        private void PrintHistory()
        {
            _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 4");
        }

        [RelayCommand]
        private void PrintBlankReport()
        {
            _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 4");
        }

        partial void OnSelectedPatientChanged(PatientListItem? value)
        {
            if (value != null)
            {
                _ = LoadPatientTestsAsync(value.PatientId);
            }
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            _ = LoadTodayPatientsAsync();
        }

        partial void OnFilterModeChanged(string value)
        {
            _ = LoadTodayPatientsAsync();
        }

        private async Task LoadTodayPatientsAsync()
        {
            var items = await _testResultsListService.GetPatientsByFilterAsync(FilterMode, SelectedDate);
            Patients.Clear();
            foreach (var item in items)
                Patients.Add(item);
        }

        private async Task LoadPatientTestsAsync(int patientId)
        {
            var tests = await _testResultsListService.GetPatientTestsAsync(patientId, SelectedDate);
            PatientTests.Clear();
            foreach (var test in tests)
                PatientTests.Add(test);
        }
    }
}
