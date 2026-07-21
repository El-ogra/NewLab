using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace NewLab.ViewModels.Pages
{
    public class IconNameToKindConverter : IValueConverter
    {
        public static readonly IconNameToKindConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string iconName)
            {
                return iconName switch
                {
                    "Flask" => PackIconKind.Flask,
                    "AccountEdit" => PackIconKind.AccountEdit,
                    "Magnify" => PackIconKind.Magnify,
                    "FileCheck" => PackIconKind.FileCheck,
                    "Wrench" => PackIconKind.Wrench,
                    "FileDocument" => PackIconKind.FileDocument,
                    "CreditCard" => PackIconKind.CreditCard,
                    "ChartBar" => PackIconKind.ChartBar,
                    "AccountGroup" => PackIconKind.AccountGroup,
                    "Database" => PackIconKind.Database,
                    "Cog" => PackIconKind.Cog,
                    "HelpCircle" => PackIconKind.HelpCircle,
                    "Information" => PackIconKind.Information,
                    "ExitToApp" => PackIconKind.ExitToApp,
                    _ => PackIconKind.HelpCircle
                };
            }
            return PackIconKind.HelpCircle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}