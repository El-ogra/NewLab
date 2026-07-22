using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NewLab.Models.Domain;
using NewLab.ViewModels.Pages;

namespace NewLab.Views.Windows
{
    public partial class BarcodeView : Window
    {
        private Point _dragStartPoint;

        public BarcodeView()
        {
            InitializeComponent();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            var border = sender as Border;
            if (border != null)
            {
                var label = border.DataContext as BarcodeLabel;
                if (label != null)
                {
                    DragDrop.DoDragDrop(border, label, DragDropEffects.Move);
                }
            }
        }

        private void Label_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void Label_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(BarcodeLabel)))
            {
                var draggedLabel = e.Data.GetData(typeof(BarcodeLabel)) as BarcodeLabel;
                var targetBorder = sender as Border;

                if (draggedLabel != null && targetBorder != null)
                {
                    var targetLabel = targetBorder.DataContext as BarcodeLabel;
                    var vm = DataContext as BarcodeViewModel;

                    if (targetLabel != null && vm != null && draggedLabel != targetLabel)
                    {
                        var draggedIndex = vm.Labels.IndexOf(draggedLabel);
                        var targetIndex = vm.Labels.IndexOf(targetLabel);

                        if (draggedIndex >= 0 && targetIndex >= 0)
                        {
                            vm.Labels.Move(draggedIndex, targetIndex);
                        }
                    }
                }
            }
        }
    }
}
