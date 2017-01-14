using ChatLibrary;
using System.Windows;
using System.Windows.Controls;

namespace ChatClient.Common
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LeftTemplate { get; set; }
        public DataTemplate RightTemplate { get; set; }
        public DataTemplate CenterTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && item is Message)
            {
                Message msg = item as Message;

                if (msg.Position == Message.Alignment.Left)
                {
                    return LeftTemplate;
                }
                else if (msg.Position == Message.Alignment.Right)
                {
                    return RightTemplate;
                }
                else
                {
                    return CenterTemplate;
                }
            }

            return null;
        }
    }
}
