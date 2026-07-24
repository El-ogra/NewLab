using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NewLab.Converters
{
    public class BoolToRedBrushConverter : IValueConverter
    {
        public static readonly BoolToRedBrushConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isImportant && isImportant)
                return Brushes.Red;

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BoolToRedBrushConverter does not support ConvertBack.");
        }
    }
}
