using ChatLibrary;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ChatClient.Converter
{
    public class ChatStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            Color fillColor = Colors.Black;
            Chat.Status status = (Chat.Status)value;

            switch (status)
            {
                case Chat.Status.Online:
                    fillColor = Colors.Green;
                    break;

                case Chat.Status.Offline:
                    fillColor = Colors.Gray;
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
