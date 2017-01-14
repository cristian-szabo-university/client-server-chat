using ChatClient.ViewModel.ChatViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ChatClient
{
    public partial class ChatPage : Page
    {
        public ChatPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new ChatPresenter();

            ChatPresenter context = DataContext as ChatPresenter;
            context.PageNavigated += DataContext_PageNavigation;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            ChatPresenter context = DataContext as ChatPresenter;
            context.PageNavigated -= DataContext_PageNavigation;

            context.Dispose();
        }

        private void DataContext_PageNavigation(object sender, Page e)
        {
            NavigationService service = NavigationService.GetNavigationService(this);
            service.Navigate(e);
        }
    }
}
