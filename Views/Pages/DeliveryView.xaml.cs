using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NewLab.Helpers;
using NewLab.ViewModels.Pages;

namespace NewLab.Views.Pages
{
    public partial class DeliveryView : UserControl
    {
        private readonly BarcodeScannerListener _listener = new();

        public DeliveryView()
        {
            InitializeComponent();
            _listener.BarcodeScanned += raw =>
            {
                if (DataContext is DeliveryViewModel vm)
                    vm.ScanBarcodeCommand.Execute(raw);
            };
            PreviewKeyDown += (s, e) => _listener.OnPreviewKeyDown(e);
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
