using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using NewLab.Models.Domain.Enums;

namespace NewLab.Converters
{
    public class TestStatusToIconConverter : IValueConverter
    {
        public static readonly TestStatusToIconConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TestStatus status)
            {
                return status switch
                {
                    TestStatus.New => "Circle",
                    TestStatus.Entered => "FileDocument",
                    TestStatus.Reviewed => "ArrowLeftRight",
                    TestStatus.Printed => "Printer",
                    TestStatus.Delivered => "Cart",
                    TestStatus.AccountIssue => "CurrencyUsd",
                    TestStatus.Completed => "Star",
                    _ => "Circle"
                };
            }
            return "Circle";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TestStatusToColorConverter : IValueConverter
    {
        public static readonly TestStatusToColorConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TestStatus status)
            {
                return status switch
                {
                    TestStatus.New => new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                    TestStatus.Entered => new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                    TestStatus.Reviewed => new SolidColorBrush(Color.FromRgb(255, 152, 0)),
                    TestStatus.Printed => new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                    TestStatus.Delivered => new SolidColorBrush(Color.FromRgb(156, 39, 176)),
                    TestStatus.AccountIssue => new SolidColorBrush(Color.FromRgb(255, 235, 59)),
                    TestStatus.Completed => new SolidColorBrush(Color.FromRgb(0, 150, 136)),
                    _ => new SolidColorBrush(Color.FromRgb(158, 158, 158))
                };
            }
            return new SolidColorBrush(Color.FromRgb(158, 158, 158));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
