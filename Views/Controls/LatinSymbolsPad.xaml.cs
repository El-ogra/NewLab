using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NewLab.Helpers;

namespace NewLab.Views.Controls
{
    public partial class LatinSymbolsPad : UserControl
    {
        public static readonly DependencyProperty TargetTextBoxProperty =
            DependencyProperty.Register(nameof(TargetTextBox), typeof(TextBox), typeof(LatinSymbolsPad), new PropertyMetadata(null));

        public static readonly DependencyProperty SymbolsProperty =
            DependencyProperty.Register(nameof(Symbols), typeof(IEnumerable<string>), typeof(LatinSymbolsPad),
                new PropertyMetadata(new[] { "¹", "²", "³", "⁴", "⁵", "⁶", "⁷", "⁸", "⁹", "⁰", "α", "β", "γ", "μ", "±", "≤", "≥", "°", "×", "÷" }));

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

        private TextBox? _lastFocusedTextBox;

        public LatinSymbolsPad()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (LatinSymbolsPadAttach.GetAutoAttach(this))
            {
                var parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    parentWindow.PreviewGotKeyboardFocus += OnParentWindowPreviewGotKeyboardFocus;
                }
            }
        }

        private void OnParentWindowPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus is TextBox textBox)
            {
                _lastFocusedTextBox = textBox;
            }
        }

        private void SymbolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string symbol)
            {
                var target = TargetTextBox ?? _lastFocusedTextBox;
                if (target != null)
                {
                    target.SelectedText = symbol;
                    target.Focus();
                }
            }
        }
    }
}
