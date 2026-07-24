using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NewLab.Views.Pages
{
    public partial class TestResultsListView : UserControl
    {
        public TestResultsListView()
        {
            InitializeComponent();
        }

        private void LabId_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.Text != null)
            {
                Clipboard.SetText(tb.Text);
            }
        }

        private void FileCode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.Text != null)
            {
                Clipboard.SetText(tb.Text);
            }
        }

        private void VisitCode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.Text != null)
            {
                Clipboard.SetText(tb.Text);
            }
        }
    }
}
