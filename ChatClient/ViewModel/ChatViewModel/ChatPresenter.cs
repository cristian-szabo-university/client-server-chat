using ChatClient.Common;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using System.ServiceModel;
using ChatClient.Service;
using System.Net;

namespace ChatClient.ViewModel.ChatViewModel
{
    class ChatPresenter : ViewModelBase
    {
        public ObservableCollection<ViewModelBase> ViewList
        {
            get { return Get(() => ViewList); }
            set { Set(() => ViewList, value); }
        }
        public ViewModelBase CurrentView
        {
            get { return Get(() => CurrentView); }
            set { Set(() => CurrentView, value); }
        }

        public ChatPresenter()
        {
            InstanceContext context = GlobalData.Service.InnerDuplexChannel.CallbackInstance;
            ChatServiceCallback callback = context.GetServiceInstance() as ChatServiceCallback;
            callback.GroupChatJoinRequest += Callback_GroupChatJoinRequest;
            callback.PrivateChatInviteRequest += Callback_PrivateChatInviteRequest;

            ViewList = new ObservableCollection<ViewModelBase>();
            ViewList.Add(new ChatGlobalViewModel(this));
            ViewList.Add(new ChatGroupViewModel(this));
            ViewList.Add(new ChatPrivateViewModel(this));
            ViewList.Add(new ChatAccountViewModel(this));

            CurrentView = ViewList[0];
        }

        private bool Callback_PrivateChatInviteRequest(InvitePrivateChatEventArgs args)
        {
            MessageDialogViewModel viewModel = new MessageDialogViewModel();
            viewModel.Title = "Chat Invite";
            viewModel.Message = "Member " + args.Member.Username + " would like to start a private chat with you.";
            viewModel.ButtonList.Add("Accept");
            viewModel.ButtonList.Add("Decline");

            MessageDialog dialog = new MessageDialog(viewModel);
            dialog.ShowDialog();

            if (viewModel.Result.Equals("Accept"))
            {
                return true;
            }

            return false;
        }

        private bool Callback_GroupChatJoinRequest(JoinGroupChatEventArgs args)
        {
            MessageDialogViewModel viewModel = new MessageDialogViewModel();
            viewModel.Title = "Join Group";
            viewModel.Message = "Member " + args.Member.Username + " would like to join your group.";
            viewModel.ButtonList.Add("Accept");
            viewModel.ButtonList.Add("Decline");

            MessageDialog dialog = new MessageDialog(viewModel);
            dialog.ShowDialog();

            if (viewModel.Result.Equals("Accept"))
            {
                return true;
            }

            return false;
        }

        public T FindView<T>() where T : ViewModelBase
        {
            Type viewType = typeof(T);

            return (T)ViewList.ToList().Find(v => v.GetType().Equals(viewType));
        }

        public async void NavigateBack()
        {
            await GlobalData.Service.DisconnectAsync();

            GlobalData.Client = null;

            IPEndPoint address = new IPEndPoint(
                IPAddress.Parse(
                    GlobalData.Service.Endpoint.Address.Uri.Host),
                    GlobalData.Service.Endpoint.Address.Uri.Port);

            GlobalData.Service = null;

            OnPageNavigated(new MemberPage(address));
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach(ViewModelBase vm in ViewList)
            {
                vm.Dispose();
            }

            ViewList.Clear();
            CurrentView = null;
        }
    }
}
