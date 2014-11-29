using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace JsConsole.Converters
{
    public class GridResizeDirectionToOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gridOrientation = value is GridResizeDirection ? (GridResizeDirection) value : GridResizeDirection.Auto;
            if (gridOrientation == GridResizeDirection.Rows)
            {
                return Orientation.Horizontal;
            }
            return Orientation.Vertical;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}