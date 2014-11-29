using System;
using System.Globalization;
using System.Windows.Data;

namespace JsConsole.Converters
{
    //[ValueConversion(typeof(bool), typeof(string))]
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (!(value is bool) || !(parameter is string))
            //{
            //    throw new ArgumentException("Invalid argument/return type");
            //}
            var chaine = parameter.ToString();
            var res = chaine.Split(new[] {'-', '_', ',', ';', ':'});
            if (res.Length > 1)
            {
                if ((bool)value)
                {
                    return res[0];
                }
                else
                {
                    return res[1];
                }
            }
            return "Error";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
