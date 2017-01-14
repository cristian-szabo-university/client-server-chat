using ChatClient.Service;
using ChatClient.ViewModel;
using ChatLibrary;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChatClient.Converter
{
    public class MemberCurrentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                Member m = value as Member;

                if (m.Equals(GlobalData.Client))
                    return FontWeights.Bold;
                else
                    return FontWeights.Normal;
            }

            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
