using System.Windows;

namespace NewLab.Helpers
{
    public static class LatinSymbolsPadAttach
    {
        public static readonly DependencyProperty AutoAttachProperty =
            DependencyProperty.RegisterAttached(
                "AutoAttach",
                typeof(bool),
                typeof(LatinSymbolsPadAttach),
                new PropertyMetadata(false));

        public static bool GetAutoAttach(DependencyObject obj) => (bool)obj.GetValue(AutoAttachProperty);
        public static void SetAutoAttach(DependencyObject obj, bool value) => obj.SetValue(AutoAttachProperty, value);
    }
}
