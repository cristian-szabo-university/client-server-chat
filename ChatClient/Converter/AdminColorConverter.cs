using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ChatClient.Converter
{
    public class AdminColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            Boolean admin = System.Convert.ToBoolean(value);

            if (admin)
            {
                return new SolidColorBrush(Colors.LightGray);
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
