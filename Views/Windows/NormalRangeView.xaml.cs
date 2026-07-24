using System.Windows;

namespace NewLab.Views.Windows
{
    public partial class NormalRangeView : Window
    {
        public NormalRangeView()
        {
            InitializeComponent();
        }

        private void BackToTests_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
