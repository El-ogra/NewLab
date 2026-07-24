using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NewLab.Helpers.Interactions
{
    public static class DoubleClickBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(DoubleClickBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static ICommand GetCommand(DependencyObject obj) => (ICommand)obj.GetValue(CommandProperty);
        public static void SetCommand(DependencyObject obj, ICommand value) => obj.SetValue(CommandProperty, value);

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(DoubleClickBehavior),
                new PropertyMetadata(null));

        public static object GetCommandParameter(DependencyObject obj) => obj.GetValue(CommandParameterProperty);
        public static void SetCommandParameter(DependencyObject obj, object value) => obj.SetValue(CommandParameterProperty, value);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                dataGrid.MouseDoubleClick -= OnMouseDoubleClick;
                if (e.NewValue != null)
                    dataGrid.MouseDoubleClick += OnMouseDoubleClick;
            }
            else if (d is ListBox listBox)
            {
                listBox.MouseDoubleClick -= OnMouseDoubleClick;
                if (e.NewValue != null)
                    listBox.MouseDoubleClick += OnMouseDoubleClick;
            }
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null) return;

            var command = GetCommand(element);
            var parameter = GetCommandParameter(element);
            if (command?.CanExecute(parameter) == true)
            {
                command.Execute(parameter);
            }
        }
    }
}
