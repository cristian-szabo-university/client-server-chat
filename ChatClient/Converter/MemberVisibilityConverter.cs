using ChatLibrary;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChatClient.Converter
{
    class MemberVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Member client = value as Member;

            if (client == null)
            {
                return Visibility.Collapsed;
            }

            if (client.Admin == true)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
