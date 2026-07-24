using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace NewLab.Views.Windows
{
    public partial class TestResultEntryView : Window
    {
        public TestResultEntryView()
        {
            InitializeComponent();
            ResultDataGrid.PreviewKeyDown += ResultDataGrid_PreviewKeyDown;
        }

        private void ResultDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var dg = (DataGrid)sender;
            var cellInfo = dg.CurrentCell;
            if (cellInfo.Column == null) return;

            var rowIndex = dg.Items.IndexOf(cellInfo.Item);
            var colIndex = cellInfo.Column.DisplayIndex;
            var totalCols = dg.Columns.Count;
            var totalRows = dg.Items.Count;

            bool isLastCell = (rowIndex == totalRows - 1) && (colIndex == totalCols - 1);

            if (isLastCell)
            {
                // Last cell: save and close
                if (DataContext is ViewModels.Pages.TestResultEntryViewModel vm)
                {
                    vm.SaveCommand.Execute(null);
                }
                e.Handled = true;
            }
            else
            {
                // Move focus to next cell
                var direction = colIndex < totalCols - 1
                    ? FocusNavigationDirection.Right
                    : FocusNavigationDirection.Down;
                var request = new TraversalRequest(direction);
                var focused = FocusManager.GetFocusedElement(this) as FrameworkElement;
                focused?.MoveFocus(request);
                e.Handled = true;
            }
        }
    }
}
