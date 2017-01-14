using ChatLibrary;
using System;
using System.Linq;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChatClient.View.ChatView
{
    public partial class MessageChatView : UserControl
    {
        public Chat Chat
        {
            get { return (Chat)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }

        public static readonly DependencyProperty ChatProperty =
            DependencyProperty.Register("Chat", typeof(Chat), typeof(MessageChatView), new PropertyMetadata(OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MessageChatView view = sender as MessageChatView;
            
            if (e.OldValue == null)
            {
                Chat chat = e.NewValue as Chat;

                chat.MessageList.CollectionChanged += view.MessageList_CollectionChanged;

                view.MessageList_CollectionChanged(null, null);
            }
            else if (e.NewValue == null)
            {
                Chat chat = e.OldValue as Chat;

                chat.MessageList.CollectionChanged -= view.MessageList_CollectionChanged;
            }
            else
            {
                Chat newChat = e.NewValue as Chat;

                newChat.MessageList.CollectionChanged += view.MessageList_CollectionChanged;

                Chat oldChat = e.OldValue as Chat;

                oldChat.MessageList.CollectionChanged -= view.MessageList_CollectionChanged;

                view.MessageList_CollectionChanged(null, null);
            }
        }

        public MessageChatView()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
        }

        private void MessageList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            listView.Dispatcher.BeginInvoke(new Action(() =>
            {
                listView.UpdateLayout();

                if (Chat.MessageList.Count > 0)
                {
                    listView.ScrollIntoView(Chat.MessageList.Last());
                }
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(async () =>
            {
                await Task.Delay(50);

                txtMessage.Clear();
                txtMessage.Focus();
            }));            
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ClearValue(ChatProperty);
        }
    }
}
