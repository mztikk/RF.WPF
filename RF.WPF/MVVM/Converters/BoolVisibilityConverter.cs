using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RF.WPF.MVVM.Converters
{
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (value is null)
            {
                return Visibility.Collapsed;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
            {
                return v switch
                {
                    Visibility.Visible => true,
                    Visibility.Hidden => false,
                    Visibility.Collapsed => false,
                    _ => false,
                };
            }
            else if (value is null)
            {
                return false;
            }

            throw new ArgumentException();
        }
    }

}
