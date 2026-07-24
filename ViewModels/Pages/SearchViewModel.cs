using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;
using NewLab.Services.Interfaces;

namespace NewLab.ViewModels.Pages
{
    public partial class SearchViewModel : ObservableObject
    {
        private readonly IPatientSearchService _searchService;
        private readonly IReferralService _referralService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ICurrentUserService _currentUserService;

        [ObservableProperty] private string? namePrefix;
        [ObservableProperty] private string? nameContains;
        [ObservableProperty] private string? phoneNumber;
        [ObservableProperty] private string? nationalId;
        [ObservableProperty] private string? visitCode;
        [ObservableProperty] private string? labCode;
        [ObservableProperty] private string? fileCode;
        [ObservableProperty] private DateTime? dateFrom;
        [ObservableProperty] private DateTime? dateTo;
        [ObservableProperty] private int? ageFrom;
        [ObservableProperty] private int? ageTo;
        [ObservableProperty] private AgeUnit? selectedAgeUnit;
        [ObservableProperty] private Gender? selectedGender;
        [ObservableProperty] private Referral? selectedReferral;
        [ObservableProperty] private SearchSource selectedSource = SearchSource.Main;

        [ObservableProperty] private ObservableCollection<PatientSearchRow> results = new();
        [ObservableProperty] private PatientSearchRow? selectedResult;
        [ObservableProperty] private ObservableCollection<PatientTest> selectedResultTests = new();
        [ObservableProperty] private PatientTestsSummary? summary;
        [ObservableProperty] private int openAccountsCount;

        public bool IsAdmin => _currentUserService.IsAdmin;
        public bool IsBackupSearchEnabled => false;
        public List<Referral> AvailableReferrals { get; private set; } = new();
        public Gender?[] AvailableGenders => new Gender?[] { null, Gender.Male, Gender.Female };
        public AgeUnit?[] AvailableAgeUnits => new AgeUnit?[] { null, AgeUnit.Day, AgeUnit.Month, AgeUnit.Year };
        public SearchSource[] AvailableSources => Enum.GetValues<SearchSource>();

        public SearchViewModel(
            IPatientSearchService searchService,
            IReferralService referralService,
            IDialogService dialogService,
            INavigationService navigationService,
            ICurrentUserService currentUserService)
        {
            _searchService = searchService;
            _referralService = referralService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _currentUserService = currentUserService;

            _ = LoadReferralsAsync();
            _ = LoadOpenAccountsCountAsync();
        }

        private async Task LoadOpenAccountsCountAsync()
        {
            OpenAccountsCount = await _searchService.GetOpenAccountsCountAsync();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            var criteria = new SearchCriteria(
                NamePrefix: NamePrefix,
                NameContains: NameContains,
                PhoneNumber: PhoneNumber,
                NationalId: NationalId,
                VisitCode: VisitCode,
                LabCode: LabCode,
                FileCode: FileCode,
                DateFrom: DateFrom,
                DateTo: DateTo,
                AgeFrom: AgeFrom,
                AgeTo: AgeTo,
                AgeUnit: SelectedAgeUnit,
                Gender: SelectedGender,
                ReferralId: SelectedReferral?.Id,
                Source: SelectedSource);

            try
            {
                var items = await _searchService.SearchAsync(criteria);
                Results.Clear();
                foreach (var item in items)
                    Results.Add(item);
            }
            catch (NotImplementedException ex)
            {
                _dialogService.ShowMessage("خطأ", ex.Message);
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", "حدث خطأ أثناء البحث: " + ex.Message);
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await SearchAsync();
        }

        [RelayCommand]
        private void OpenPatientData()
        {
            _navigationService.NavigateTo<PatientEntryViewModel>();
        }

        [RelayCommand]
        private void OpenResults()
        {
            _navigationService.NavigateTo<TestResultsListViewModel>();
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteAsync()
        {
            if (SelectedResult == null) return;

            var confirmed = _dialogService.ShowConfirmation("تأكيد الحذف", "هل أنت متأكد من حذف هذا المريض؟");
            if (!confirmed) return;

            try
            {
                await _searchService.DeletePatientAsync(SelectedResult.PatientId);
                _dialogService.ShowMessage("نجاح", "تم حذف المريض بنجاح");
                Results.Remove(SelectedResult);
                SelectedResult = null;
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

        [RelayCommand]
        private void PrintGroupResults()
        {
            _dialogService.ShowMessage("قريباً", "ستتوفر هذه الميزة قريباً");
        }

        [RelayCommand(CanExecute = nameof(CanBackup))]
        private void SwitchToBackup()
        {
        }

        [RelayCommand]
        private void Back()
        {
            _navigationService.GoBack();
        }

        private bool CanDelete() => IsAdmin && SelectedResult != null;
        private bool CanBackup() => IsBackupSearchEnabled;

        partial void OnSelectedResultChanged(PatientSearchRow? value)
        {
            if (value != null) _ = LoadSelectedPatientTestsAsync(value.PatientId);
            DeleteCommand.NotifyCanExecuteChanged();
        }

        private async Task LoadSelectedPatientTestsAsync(int patientId)
        {
            var tests = await _searchService.GetPatientTestsSummaryAsync(patientId);
            SelectedResultTests.Clear();
            foreach (var test in tests)
                SelectedResultTests.Add(test);

            Summary = await _searchService.GetSummaryAsync(patientId);
        }

        private async Task LoadReferralsAsync()
        {
            var referrals = await _referralService.SearchByNameAsync("");
            AvailableReferrals = referrals;
        }
    }
}
