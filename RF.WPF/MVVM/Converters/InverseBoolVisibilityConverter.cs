using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RF.WPF.MVVM.Converters
{
    public class InverseBoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? Visibility.Collapsed : Visibility.Visible;
            }
            else if (value is null)
            {
                return Visibility.Visible;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
            {
                return v switch
                {
                    Visibility.Visible => false,
                    Visibility.Hidden => true,
                    Visibility.Collapsed => true,
                    _ => true,
                };
            }
            else if (value is null)
            {
                return true;
            }

            throw new ArgumentException();
        }
    }

}
