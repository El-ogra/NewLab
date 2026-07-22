using System.Windows;

namespace NewLab.Views.Windows
{
    public partial class CalculationConstantsView : Window
    {
        public CalculationConstantsView()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
