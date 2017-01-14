using ChatClient.ViewModel.MemberViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ChatClient.View.MemberView
{
    public partial class SignInView : UserControl
    {
        public SignInView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MemberSignInViewModel context = DataContext as MemberSignInViewModel;
            context.PageNavigated += DataContext_PageNavigation;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            MemberSignInViewModel context = DataContext as MemberSignInViewModel;
            context.PageNavigated -= DataContext_PageNavigation;
        }

        private void DataContext_PageNavigation(object sender, Page e)
        {
            NavigationService service = NavigationService.GetNavigationService(this);
            service.Navigate(e);
        }
    }
}
