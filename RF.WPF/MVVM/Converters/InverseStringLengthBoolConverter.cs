using System;
using System.Globalization;
using System.Windows.Data;

namespace RF.WPF.MVVM.Converters
{
    public class InverseStringLengthBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
        {
            string str => str.Length == 0,
            null => true,
            _ => throw new ArgumentException(),
        };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

}
