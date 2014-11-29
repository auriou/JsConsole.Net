using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace JsConsole.Converters
{
    public class BoolToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool) || !(targetType == typeof(double)))
            {
                throw new ArgumentException(
                    "Invalid argument/return type. Expected argument: bool and return type: Visibility");
            }
            if (!((bool)value))
            {
                return 0d;
            }
            if ((parameter != null) && (parameter is Visibility))
            {
                return parameter;
            }
            return double.NaN;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility) || !(targetType == typeof(bool)))
            {
                throw new ArgumentException(
                    "Invalid argument/return type. Expected argument: Visibility and return type: bool");
            }
            if (((Visibility)value) == Visibility.Visible)
            {
                return double.NaN;
            }
            return 0d;
        }
    }
}