using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NewLab.Converters
{
    public class BooleanToActiveColorConverter : IValueConverter
    {
        public static readonly BooleanToActiveColorConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive && isActive)
                return new SolidColorBrush(Colors.Green);
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
