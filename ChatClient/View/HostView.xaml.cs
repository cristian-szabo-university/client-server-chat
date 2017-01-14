using ChatClient.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ChatClient.View
{
    public partial class HostView : UserControl
    {
        public HostView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            HostViewModel context = DataContext as HostViewModel;
            context.PageNavigated += DataContext_PageNavigation;
        }

        private void DataContext_PageNavigation(object sender, Page e)
        {
            NavigationService service = NavigationService.GetNavigationService(this);
            service.Navigate(e);
        }
    }
}
