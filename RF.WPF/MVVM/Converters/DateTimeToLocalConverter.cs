using System;
using System.Globalization;
using System.Windows.Data;

namespace RF.WPF.MVVM.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToLocalTime().ToString();
            }

            throw new ArgumentException(value.ToString());
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();


    }
}
