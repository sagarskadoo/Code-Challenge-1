using System;
using System.Globalization;
using Xamarin.Forms;

namespace PacmanSimulator.Converters
{
    /// <summary>
    /// Converter for string to int conversion and vice-versa.
    /// </summary>
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty((string)value))
                return -1;
            int number;
            if (int.TryParse((string)value, out number))
                return number;
            else
                return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return ((Int32)value).ToString();
        }
    }
}