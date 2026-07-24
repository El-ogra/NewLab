using System.Windows;

namespace NewLab.Views.Windows
{
    public partial class NormalRangeSixWizardDialog : Window
    {
        public string Unit => UnitBox.Text;
        public string NormalRangeText => RangeTextBox.Text;
        public decimal LowLimit => decimal.TryParse(LowLimitBox.Text, out var v) ? v : 0;
        public decimal HighLimit => decimal.TryParse(HighLimitBox.Text, out var v) ? v : 0;
        public string? LowFlag => LowFlagBox.Text;
        public string? HighFlag => HighFlagBox.Text;
        public string? LowComment => LowCommentBox.Text;
        public string? HighComment => HighCommentBox.Text;

        public NormalRangeSixWizardDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
