using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RF.WPF.MVVM.Converters
{
    public class StringLengthVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (value is null)
            {
                return Visibility.Collapsed;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

}
