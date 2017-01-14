using ChatClient.ViewModel;
using ChatLibrary;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ChatClient
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _ActivityTimer;
        private Point _LastMousePos = new Point(0, 0);

        public MainWindow()
        {
            InitializeComponent();

            InputManager.Current.PreProcessInput += OnActivity;

            _ActivityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5),
                IsEnabled = true
            };

            _ActivityTimer.Tick += OnInactivity;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new StartPage());
        }

        async void OnInactivity(object sender, EventArgs e)
        {
            _LastMousePos = Mouse.GetPosition(MainFrame);

            if (GlobalData.Client != null && GlobalData.Client.Available == Member.Status.Active)
            {
                await GlobalData.Service.ChangeStatusAsync(Member.Status.Away);
            }
        }

        async void OnActivity(object sender, PreProcessInputEventArgs e)
        {
            InputEventArgs inputEvent = e.StagingItem.Input;

            if (inputEvent is MouseEventArgs || inputEvent is KeyboardEventArgs)
            {
                if (e.StagingItem.Input is MouseEventArgs)
                {
                    MouseEventArgs mouseEvent = (MouseEventArgs)e.StagingItem.Input;

                    if (mouseEvent.LeftButton == MouseButtonState.Released &&
                        mouseEvent.RightButton == MouseButtonState.Released &&
                        mouseEvent.MiddleButton == MouseButtonState.Released &&
                        _LastMousePos == mouseEvent.GetPosition(MainFrame))
                        return;
                }

                if (GlobalData.Client != null && GlobalData.Client.Available == Member.Status.Away)
                {
                    await GlobalData.Service.ChangeStatusAsync(Member.Status.Active);
                }

                _ActivityTimer.Stop();
                _ActivityTimer.Start();
            }
        }
    }
}
