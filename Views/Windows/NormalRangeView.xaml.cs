using System.Windows;
using NewLab.Models.Domain.Enums;

namespace NewLab.Views.Windows
{
    public partial class NormalRangeView : Window
    {
        public NormalRangeView()
        {
            InitializeComponent();
            // Populate ComboBoxes (Technical Note 4: enum x:Array fails in XAML)
            GenderCombo.ItemsSource = new[] { Gender.Male, Gender.Female };
            AgeUnitCombo.ItemsSource = System.Enum.GetValues<AgeUnit>();
        }

        private void BackToTests_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
