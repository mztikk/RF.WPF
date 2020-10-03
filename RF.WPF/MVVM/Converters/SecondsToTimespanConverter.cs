using System;
using System.Globalization;
using System.Windows.Data;

namespace RF.WPF.MVVM.Converters
{
    public class SecondsToTimespanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timespan)
            {
                return timespan.TotalSeconds;
            }

            throw new ArgumentException(value?.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && double.TryParse(str, out double seconds))
            {
                return TimeSpan.FromSeconds(seconds);
            }

            throw new ArgumentException(value?.ToString());
        }
    }

}
