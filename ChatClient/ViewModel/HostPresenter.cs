using ChatClient.Common;

namespace ChatClient.ViewModel
{
    class HostPresenter : ViewModelBase
    {
        public HostPresenter()
        {
            CurrentView = new HostViewModel();
        }

        public ViewModelBase CurrentView
        {
            get { return Get(() => CurrentView); }
            set { Set(() => CurrentView, value); }
        }
    }
}
