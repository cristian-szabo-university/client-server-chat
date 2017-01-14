using ChatClient.ViewModel.ChatViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ChatClient.View.ChatView
{
    public partial class AccountChatView : UserControl
    {
        public AccountChatView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ChatAccountViewModel context = DataContext as ChatAccountViewModel;
            context.PageNavigated += DataContext_PageNavigation;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ChatAccountViewModel context = DataContext as ChatAccountViewModel;
            context.PageNavigated -= DataContext_PageNavigation;
        }

        private void DataContext_PageNavigation(object sender, Page e)
        {
            NavigationService service = NavigationService.GetNavigationService(this);
            service.Navigate(e);
        }
    }
}
