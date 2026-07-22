using System.Windows.Controls;
using NewLab.Models.Domain.Enums;

namespace NewLab.Views.Pages
{
    public partial class PatientEntryView : UserControl
    {
        public PatientEntryView()
        {
            InitializeComponent();

            GenderCombo.ItemsSource = new[] { Gender.Male, Gender.Female };
            GenderCombo.SelectedItem = Gender.Male;

            AgeUnitCombo.ItemsSource = new[] { AgeUnit.Day, AgeUnit.Month, AgeUnit.Year };
            AgeUnitCombo.SelectedItem = AgeUnit.Year;

            BillingCombo.ItemsSource = new[] { BillingSystem.Individual, BillingSystem.LabToLab, BillingSystem.Free };
            BillingCombo.SelectedItem = BillingSystem.Individual;
        }
    }
}
