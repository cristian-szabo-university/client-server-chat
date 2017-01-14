using ChatClient.ViewModel;
using ChatLibrary;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChatClient.Converter
{
    public class ChatVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Chat chat = value as Chat;

            if (chat == null)
            {
                return Visibility.Collapsed;
            }

            if (chat.Admin.Equals(GlobalData.Client))
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
