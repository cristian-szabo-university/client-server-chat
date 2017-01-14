using ChatClient.Common;
using ChatClient.Service;
using ChatLibrary;
using System;
using System.ComponentModel;
using System.Net;
using System.ServiceModel;
using System.Windows.Input;

namespace ChatClient.ViewModel.ChatViewModel
{
    class ChatAccountViewModel : ViewModelBase
    {
        private ChatPresenter _Parent;

        private DateTime _TimeSinceBusy;

        public override string Name
        {
            get
            {
                return "Account";
            }
        }

        public ICommand SingOutCommand
        {
            get { return Get(() => SingOutCommand); }
            set { Set(() => SingOutCommand, value); }
        }
        public String Available
        {
            get { return Get(() => Available); }
            set { Set(() => Available, value); }
        }

        public ChatAccountViewModel(ViewModelBase parent)
        {
            _Parent = parent as ChatPresenter;

            Available = GlobalData.Client.Available.ToString();

            SingOutCommand = new RelayCommand(OnSingOut, CanSignOut);

            PropertyChanged += ChatAccountViewModel_PropertyChanged;

            InstanceContext context = GlobalData.Service.InnerDuplexChannel.CallbackInstance;
            ChatServiceCallback callback = context.GetServiceInstance() as ChatServiceCallback;
            callback.MemberStatus += Callback_MemberStatus;
        }

        private async void Callback_MemberStatus(MemberStatusEventArgs args)
        {
            if (GlobalData.Client.Username.Equals(args.Username))
            {
                if (GlobalData.Client.Available == Member.Status.Busy &&
                    args.Available == Member.Status.Active)
                {
                    await GlobalData.Service.UpdateMessageListFromAsync(_TimeSinceBusy);
                }

                if (args.Available == Member.Status.Busy)
                {
                    _TimeSinceBusy = DateTime.Now;
                }

                GlobalData.Client.Available = args.Available;

                Available = args.Available.ToString();
            }
        }

        private async void ChatAccountViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("Available"))
            {
                return;
            }

            Member.Status available = (Member.Status)Enum.Parse(typeof(Member.Status), Available);

            if (GlobalData.Client.Available != available)
            {
                await GlobalData.Service.ChangeStatusAsync(available);
            }
        }

        private bool CanSignOut()
        {
            return true;
        }

        private void OnSingOut()
        {
            _Parent.NavigateBack();
        }
    }
}
