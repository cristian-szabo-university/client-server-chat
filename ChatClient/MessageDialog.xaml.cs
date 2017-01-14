using ChatClient.Common;
using ChatClient.ViewModel;
using System;
using System.Timers;
using System.Windows;

namespace ChatClient
{
    public partial class MessageDialog : Window
    {
        private Timer CloseTimer;

        public MessageDialog(ViewModelBase viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            CloseTimer = new Timer();
            CloseTimer.Elapsed += Window_Timeout;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application app = Application.Current;
            Window wnd = app.MainWindow;
            Left = wnd.Left + (wnd.Width - ActualWidth) / 2;
            Top = wnd.Top + (wnd.Height - ActualHeight) / 2;

            MessageDialogViewModel viewModel = DataContext as MessageDialogViewModel;
            CloseTimer.Interval = viewModel.Timeout * 1000;

            CloseTimer.Start();
        }

        private void Window_Timeout(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (IsActive)
                {
                    Close();
                }
            }));
        }
    }
}
