using ChatLibrary;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ChatClient.Converter
{
    public class MemberStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color fillColor = Colors.Black;

            if (value == null)
            {
                return new SolidColorBrush(fillColor);
            }

            Member.Status available = (Member.Status)value;
            
            switch (available)
            {
                case Member.Status.Active:
                    fillColor = Colors.Green;
                    break;

                case Member.Status.Away:
                    fillColor = Colors.Yellow;
                    break;

                case Member.Status.Busy:
                    fillColor = Colors.Red;
                    break;
            }

            return new SolidColorBrush(fillColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
