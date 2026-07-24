using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;

namespace NewLab.ViewModels.Windows
{
    public partial class AuditLogViewModel : ObservableObject
    {
        private readonly ITestResultsListService _testResultsListService;

        [ObservableProperty] private ObservableCollection<AuditLog> patientAudits = new();
        [ObservableProperty] private ObservableCollection<AuditLog> testAudits = new();
        [ObservableProperty] private string patientName = string.Empty;
        [ObservableProperty] private string testName = string.Empty;

        public AuditLogViewModel(ITestResultsListService testResultsListService)
        {
            _testResultsListService = testResultsListService;
        }

        public async Task LoadAsync(int patientId, int patientTestId, string patientName, string testName)
        {
            PatientName = patientName;
            TestName = testName;

            var patientAuditList = await _testResultsListService.GetAuditForPatientAsync(patientId);
            PatientAudits.Clear();
            foreach (var audit in patientAuditList)
                PatientAudits.Add(audit);

            var testAuditList = await _testResultsListService.GetAuditForTestAsync(patientTestId);
            TestAudits.Clear();
            foreach (var audit in testAuditList)
                TestAudits.Add(audit);
        }
    }
}
