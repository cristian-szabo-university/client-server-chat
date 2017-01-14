using ChatClient.Common;
using System.Windows;

namespace ChatClient
{
    public partial class GroupDialog : Window
    {
        public GroupDialog(ViewModelBase viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application app = Application.Current;
            Window wnd = app.MainWindow;
            Left = wnd.Left + (wnd.Width - ActualWidth) / 2;
            Top = wnd.Top + (wnd.Height - ActualHeight) / 2;
        }
    }
}
