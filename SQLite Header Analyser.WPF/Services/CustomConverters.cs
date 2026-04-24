using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace SQLite_Header_Analyser.WPF.Services
{
    internal class CustomConverters
    {
    }

    /// <summary>
    /// Use to convert a byte to a hex representation as string 
    /// </summary>
    public class DecimalToHexConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte byteValue)
            {
                return byteValue.ToString("X2");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
