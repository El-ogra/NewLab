using System.Windows.Controls;
using System.Windows.Input;
using NewLab.Models.Domain.Enums;

namespace NewLab.Views.Pages
{
    public partial class PatientEntryView : UserControl
    {
        private readonly Helpers.BarcodeScannerListener _barcodeListener = new();

        public PatientEntryView()
        {
            InitializeComponent();

            GenderCombo.ItemsSource = new[] { Gender.Male, Gender.Female };
            GenderCombo.SelectedItem = Gender.Male;

            AgeUnitCombo.ItemsSource = new[] { AgeUnit.Day, AgeUnit.Month, AgeUnit.Year };
            AgeUnitCombo.SelectedItem = AgeUnit.Year;

            BillingCombo.ItemsSource = new[] { BillingSystem.Individual, BillingSystem.LabToLab, BillingSystem.Free };
            BillingCombo.SelectedItem = BillingSystem.Individual;

            _barcodeListener.BarcodeScanned += OnBarcodeScanned;
        }

        private void OnBarcodeScanned(string code)
        {
            if (DataContext is ViewModels.Pages.PatientEntryViewModel vm)
            {
                vm.LabId = code;
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _barcodeListener.OnPreviewKeyDown(e);
        }
    }
}
