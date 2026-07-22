using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NewLab.Views.Controls
{
    public partial class LatinSymbolsPad : UserControl
    {
        public static readonly DependencyProperty TargetTextBoxProperty =
            DependencyProperty.Register(nameof(TargetTextBox), typeof(TextBox), typeof(LatinSymbolsPad), new PropertyMetadata(null));

        public static readonly DependencyProperty SymbolsProperty =
            DependencyProperty.Register(nameof(Symbols), typeof(IEnumerable<string>), typeof(LatinSymbolsPad),
                new PropertyMetadata(new[] { "α", "β", "γ", "μ", "±", "≤", "≥", "°" }));

        public TextBox? TargetTextBox
        {
            get => (TextBox?)GetValue(TargetTextBoxProperty);
            set => SetValue(TargetTextBoxProperty, value);
        }

        public IEnumerable<string> Symbols
        {
            get => (IEnumerable<string>)GetValue(SymbolsProperty);
            set => SetValue(SymbolsProperty, value);
        }

        public LatinSymbolsPad()
        {
            InitializeComponent();
        }

        private void SymbolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string symbol && TargetTextBox != null)
            {
                TargetTextBox.SelectedText = symbol;
                TargetTextBox.Focus();
            }
        }
    }
}
