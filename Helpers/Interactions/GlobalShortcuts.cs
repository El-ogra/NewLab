using System.Windows;
using System.Windows.Input;

namespace NewLab.Helpers.Interactions
{
    public static class GlobalShortcuts
    {
        public static readonly DependencyProperty RegisterOnProperty =
            DependencyProperty.RegisterAttached(
                "RegisterOn",
                typeof(bool),
                typeof(GlobalShortcuts),
                new PropertyMetadata(false, OnRegisterOnChanged));

        public static bool GetRegisterOn(DependencyObject obj) => (bool)obj.GetValue(RegisterOnProperty);
        public static void SetRegisterOn(DependencyObject obj, bool value) => obj.SetValue(RegisterOnProperty, value);

        private static void OnRegisterOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element) return;

            if ((bool)e.NewValue)
            {
                element.Loaded += OnElementLoaded;
                element.Unloaded += OnElementUnloaded;
            }
            else
            {
                element.Loaded -= OnElementLoaded;
                element.Unloaded -= OnElementUnloaded;
            }
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement element) return;

            var window = Window.GetWindow(element);
            if (window == null) return;

            var bindings = new[]
            {
                CreateBinding(Key.F2, "OpenPatientDataCommand"),
                CreateBinding(Key.F3, "OpenSearchCommand"),
                CreateBinding(Key.F4, "OpenTestResultsListCommand"),
                CreateBinding(Key.F5, "RefreshCommand"),
                CreateBinding(Key.F6, "OpenDeliveryCommand"),
                CreateBinding(Key.F7, "OpenExternalSpecimensCommand"),
                CreateBinding(Key.F8, "ToggleReviewedCommand"),
                CreateBinding(Key.F9, "TogglePrintedCommand"),
                CreateBinding(Key.F12, "ToggleEnteredCommand"),
                CreateBinding(Key.Escape, "CloseCommand")
            };

            foreach (var binding in bindings)
            {
                window.InputBindings.Add(binding);
            }
        }

        private static void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement element) return;

            var window = Window.GetWindow(element);
            if (window == null) return;

            var toRemove = new System.Collections.Generic.List<KeyBinding>();
            foreach (KeyBinding kb in window.InputBindings)
            {
                if (kb.Command is RoutedCommand || kb.Command is ICommand)
                {
                    toRemove.Add(kb);
                }
            }
            foreach (var kb in toRemove)
            {
                window.InputBindings.Remove(kb);
            }
        }

        private static KeyBinding CreateBinding(Key key, string commandName)
        {
            var binding = new KeyBinding
            {
                Key = key,
                Command = new RelayCommand(() =>
                {
                    var window = Application.Current.MainWindow;
                    if (window?.DataContext is System.ComponentModel.INotifyPropertyChanged vm)
                    {
                        var prop = vm.GetType().GetProperty(commandName);
                        if (prop?.GetValue(vm) is ICommand cmd && cmd.CanExecute(null))
                        {
                            cmd.Execute(null);
                        }
                    }
                })
            };
            return binding;
        }

        private class RelayCommand : ICommand
        {
            private readonly System.Action _execute;
#pragma warning disable CS0067
            public event System.EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

            public RelayCommand(System.Action execute) => _execute = execute;
            public bool CanExecute(object? parameter) => true;
            public void Execute(object? parameter) => _execute();
        }
    }
}
