using ChatClient.Common;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Linq;
using System;
using ChatClient.Service;
using System.Windows.Input;
using ChatLibrary;

namespace ChatClient.ViewModel.ChatViewModel
{
    class ChatGroupViewModel : ViewModelBase
    {
        private ChatPresenter _Parent;

        public override string Name
        {
            get
            {
                return "Group";
            }
        }

        public ObservableCollection<GroupChat> GroupChatList
        {
            get { return Get(() => GroupChatList); }
            set { Set(() => GroupChatList, value); }
        }
        public GroupChat CurrentChat
        {
            get { return Get(() => CurrentChat); }
            set { Set(() => CurrentChat, value); }
        }
        public Member CurrentMember
        {
            get { return Get(() => CurrentMember); }
            set { Set(() => CurrentMember, value); }
        }

        public ICommand SendMessageCommand
        {
            get { return Get(() => SendMessageCommand); }
            set { Set(() => SendMessageCommand, value); }
        }
        public ICommand LeaveChatCommand
        {
            get { return Get(() => LeaveChatCommand); }
            set { Set(() => LeaveChatCommand, value); }
        }
        public ICommand InviteChatCommand
        {
            get { return Get(() => InviteChatCommand); }
            set { Set(() => InviteChatCommand, value); }
        }
        public ICommand KickChatCommand
        {
            get { return Get(() => KickChatCommand); }
            set { Set(() => KickChatCommand, value); }
        }

        public ChatGroupViewModel(ViewModelBase parent)
        {
            _Parent = parent as ChatPresenter;

            GroupChatList = new ObservableCollection<GroupChat>();

            SendMessageCommand = new RelayCommand<String>(OnSendMessage, CanSendMessage);
            LeaveChatCommand = new RelayCommand(OnLeaveChat, CanLeaveChat);
            InviteChatCommand = new RelayCommand(OnInviteChat, CanInviteChat);
            KickChatCommand = new RelayCommand(OnKickChat, CanKickChat);

            InstanceContext context = GlobalData.Service.InnerDuplexChannel.CallbackInstance;
            ChatServiceCallback callback = context.GetServiceInstance() as ChatServiceCallback;
            callback.MemberLeaveChat += Callback_MemberLeaveChat;
            callback.MemberJoinChat += Callback_MemberJoinChat;
            callback.GroupChatJoinResponse += Callback_GroupChatJoinResponse;
            callback.MessageChat += Callback_MessageChat;
            callback.MemberStatus += Callback_MemberStatus;
        }

        private bool CanSendMessage(String arg)
        {
            if (CurrentChat == null)
            {
                return false;
            }

            if (CurrentChat.Active == Chat.Status.Offline)
            {
                return false;
            }

            if (CurrentChat.MemberList.Count == 1)
            {
                return false;
            }
            
            if (arg.Length == 0)
            {
                return false;
            }

            return true;
        }

        private async void OnSendMessage(String arg)
        {
            if (GlobalData.Client.Available == Member.Status.Busy)
            {
                await GlobalData.Service.ChangeStatusAsync(Member.Status.Active);
            }

            await GlobalData.Service.MessageChatAsync(CurrentChat.Name, arg);
        }

        private bool CanKickChat()
        {
            if (CurrentChat == null)
            {
                return false;
            }

            if (CurrentMember == null)
            {
                return false;
            }

            if (CurrentChat.Active == Chat.Status.Offline)
            {
                return false;
            }

            if (CurrentChat.Admin.Equals(CurrentMember))
            {
                return false;
            }

            if (!CurrentChat.Admin.Equals(GlobalData.Client))
            {
                return false;
            }

            return true;
        }

        private async void OnKickChat()
        {
            if (GlobalData.Client.Available == Member.Status.Busy)
            {
                await GlobalData.Service.ChangeStatusAsync(Member.Status.Active);
            }

            await GlobalData.Service.KickOutMemberAsync(CurrentMember.Username, CurrentChat.Name);
        }

        private bool CanInviteChat()
        {
            if (CurrentChat == null)
            {
                return false;
            }

            if (CurrentMember == null)
            {
                return false;
            }

            if (CurrentMember.Equals(GlobalData.Client))
            {
                return false;
            }

            if (CurrentMember.Available == Member.Status.Busy)
            {
                return false;
            }

            ChatPrivateViewModel viewModel = _Parent.FindView<ChatPrivateViewModel>();

            if (viewModel.PrivateChatList.ToList().Exists(
                c => c.Active == Chat.Status.Online &&
                (c.Person.Equals(CurrentMember) || c.Person.Equals(GlobalData.Client))))
            {
                return false;
            }

            return true;
        }

        private async void OnInviteChat()
        {
            if (GlobalData.Client.Available == Member.Status.Busy)
            {
                await GlobalData.Service.ChangeStatusAsync(Member.Status.Active);
            }

            PrivateChat chat = await GlobalData.Service.InvitePrivateChatAsync(CurrentMember.Username);

            if (chat != null)
            {
                ChatPrivateViewModel viewModel = _Parent.FindView<ChatPrivateViewModel>();

                if (viewModel != null)
                {
                    viewModel.PrivateChatList.Add(chat);
                    _Parent.CurrentView = viewModel;
                }
            }
        }

        private bool CanLeaveChat()
        {
            if (CurrentChat == null)
            {
                return false;
            }

            return true;
        }

        private async void OnLeaveChat()
        {
            if (GlobalData.Client.Available == Member.Status.Busy)
            {
                await GlobalData.Service.ChangeStatusAsync(Member.Status.Active);
            }

            if (CurrentChat.Active == Chat.Status.Online)
            {
                await GlobalData.Service.LeaveGroupChatAsync(CurrentChat.Name);
            }
            else
            {
                GroupChatList.Remove(CurrentChat);

                if (GroupChatList.Count == 0)
                {
                    _Parent.CurrentView = _Parent.FindView<ChatGlobalViewModel>();
                }
            }
        }

        private void Callback_GroupChatJoinResponse(JoinGroupChatEventArgs args)
        {
            GroupChatList.Add(args.Chat);

            _Parent.CurrentView = this;
        }

        private void Callback_MemberJoinChat(MemberChatEventArgs args)
        {
            GroupChat chat = GroupChatList.ToList().Find(c => c.Name.Equals(args.ChatName));

            if (chat == null)
            {
                return;
            }

            chat.AddMember(args.Member);
        }

        private void Callback_MemberLeaveChat(MemberChatEventArgs args)
        {
            GlobalChat globalChat = _Parent.FindView<ChatGlobalViewModel>().GlobalChat;

            if (globalChat.Name.Equals(args.ChatName))
            {
                if (_Parent.CurrentView == this)
                {
                    _Parent.NavigateBack();
                }
                
                return;
            }

            GroupChat chat = GroupChatList.ToList().Find(c => c.Name.Equals(args.ChatName));

            if (chat == null)
            {
                return;
            }

            if (args.Member.Equals(GlobalData.Client))
            {
                GroupChatList.Remove(chat);

                if (GroupChatList.Count == 0)
                {
                    _Parent.CurrentView = _Parent.FindView<ChatGlobalViewModel>();
                }
            }
            else
            {
                if (chat.Admin.Equals(args.Member))
                {
                    chat.Active = Chat.Status.Offline;
                }

                chat.MemberList.Remove(args.Member);
            }
        }

        private void Callback_MessageChat(MessageChatEventArgs args)
        {
            GroupChat chat = GroupChatList.ToList().Find(c => c.Name.Equals(args.ChatName));

            if (chat == null)
            {
                return;
            }

            chat.MessageList.Add(args.Message);

            if (chat.Active == Chat.Status.Offline)
            {
                chat.Name += " (" + args.Message.Deliver.ToLongTimeString() + ")";
            }
        }

        private void Callback_MemberStatus(MemberStatusEventArgs args)
        {
            GroupChatList.ToList().ForEach(
                chat =>
                {
                    if (chat.Active == Chat.Status.Online)
                    {
                        if (chat.Admin.Username.Equals(args.Username))
                        {
                            chat.Admin.Available = args.Available;
                        }

                        Member client = chat.FindMember(args.Username);

                        if (client != null)
                        {
                            client.Available = args.Available;
                        }
                    }
                });
        }

        public override void Dispose()
        {
            base.Dispose();

            GroupChatList.Clear();
            CurrentChat = null;
            CurrentMember = null;
        }
    }
}
