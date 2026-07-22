using System.Windows.Controls;
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
    }
}
