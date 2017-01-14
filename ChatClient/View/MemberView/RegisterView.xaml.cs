using ChatClient.ViewModel.MemberViewModel;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ChatClient.View.MemberView
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MemberRegisterViewModel context = DataContext as MemberRegisterViewModel;
            context.PageNavigated += DataContext_PageNavigation;
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MemberRegisterViewModel context = DataContext as MemberRegisterViewModel;
            context.PageNavigated -= DataContext_PageNavigation;
        }

        private void DataContext_PageNavigation(object sender, Page e)
        {
            NavigationService service = NavigationService.GetNavigationService(this);
            service.Navigate(e);
        }
    }
}
