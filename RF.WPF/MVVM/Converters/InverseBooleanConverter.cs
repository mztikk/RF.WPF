﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace RF.WPF.MVVM.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
         => !(bool?)value ?? true;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
         => !(value as bool?);
    }

}
