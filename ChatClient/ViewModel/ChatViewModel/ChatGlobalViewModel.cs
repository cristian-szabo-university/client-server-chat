using ChatClient.Common;
using ChatClient.Service;
using ChatLibrary;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace ChatClient.ViewModel.ChatViewModel
{
    class ChatGlobalViewModel : ViewModelBase
    {
        private ChatPresenter _Parent;

        public override string Name
        {
            get
            {
                return "Global";
            }
        }

        public ICommand SendMessageCommand
        {
            get { return Get(() => SendMessageCommand); }
            set { Set(() => SendMessageCommand, value); }
        }
        public ICommand CreateGroupChatCommand
        {
            get { return Get(() => CreateGroupChatCommand); }
            set { Set(() => CreateGroupChatCommand, value); }
        }
        public ICommand JoinGroupChatCommand
        {
            get { return Get(() => JoinGroupChatCommand); }
            set { Set(() => JoinGroupChatCommand, value); }
        }
        public ICommand InvitePrivateChatCommand
        {
            get { return Get(() => InvitePrivateChatCommand); }
            set { Set(() => InvitePrivateChatCommand, value); }
        }
        
        public GlobalChat GlobalChat
        {
            get { return Get(() => GlobalChat); }
            set { Set(() => GlobalChat, value); }
        }
        public Member CurrentMember
        {
            get { return Get(() => CurrentMember); }
            set { Set(() => CurrentMember, value); }
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
        public ICommand KickChatCommand
        {
            get { return Get(() => KickChatCommand); }
            set { Set(() => KickChatCommand, value); }
        }

        public ChatGlobalViewModel(ViewModelBase parent)
        {
            _Parent = parent as ChatPresenter;

            GlobalChat = new GlobalChat()
            {
                Admin = GlobalData.Client
            };

            GroupChatList = new ObservableCollection<GroupChat>();

            SendMessageCommand = new RelayCommand<String>(OnSendMessage, CanSendMessage);
            CreateGroupChatCommand = new RelayCommand(OnCreateGroupChat, CanCreateGroupChat);
            JoinGroupChatCommand = new RelayCommand(OnJoinGroupChat, CanJoinGroupChat);
            InvitePrivateChatCommand = new RelayCommand(OnInvitePrivateChat, CanInvitePrivateChat);
            KickChatCommand = new RelayCommand(OnKickChat, CanKickChat);

            InstanceContext context = GlobalData.Service.InnerDuplexChannel.CallbackInstance;
            ChatServiceCallback callback = context.GetServiceInstance() as ChatServiceCallback;
            callback.MemberJoinChat += Callback_MemberJoinChat;
            callback.MemberLeaveChat += Callback_MemberLeaveChat;
            callback.GroupChatOpen += Callback_GroupChatOpen;
            callback.MessageChat += Callback_MessageChat;
            callback.MemberStatus += Callback_MemberStatus;

            var updateData = new Action(async () =>
            {
                await GlobalData.Service.UpdateMemberListAsync();
                await GlobalData.Service.UpdateGroupChatListAsync();
            });

            Dispatcher.CurrentDispatcher.BeginInvoke(updateData);
        }

        private bool CanKickChat()
        {
            if (CurrentMember == null)
            {
                return false;
            }

            if (CurrentMember.Equals(GlobalData.Client))
            {
                return false;
            }

            return true;
        }

        private async void OnKickChat()
        {
            await GlobalData.Service.KickOutMemberAsync(CurrentMember.Username, GlobalChat.Name);
        }

        private bool CanSendMessage(String arg)
        {
            if (GlobalChat == null)
            {
                return false;
            }

            if (GlobalChat.MemberList.Count == 1)
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

            await GlobalData.Service.MessageChatAsync(GlobalChat.Name, arg);
        }

        private bool CanInvitePrivateChat()
        {
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

        private async void OnInvitePrivateChat()
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
                    viewModel.CurrentChat = chat;
                    _Parent.CurrentView = viewModel;
                }
            }
        }

        private bool CanJoinGroupChat()
        {
            if (CurrentChat == null)
            {
                return false;
            }

            if (CurrentChat.MemberList.Count == CurrentChat.MaxSize)
            {
                return false;
            }

            if ((DateTime.Today.Year - GlobalData.Client.Birthday.Year) < CurrentChat.MinAge)
            {
                return false;
            }

            if (CurrentChat.Admin.Equals(GlobalData.Client))
            {
                return false;
            }

            if (CurrentChat.Admin.Available == Member.Status.Busy)
            {
                return false;
            }

            if (CurrentChat.HasMember(GlobalData.Client))
            {
                return false;
            }

            return true;
        }

        private async void OnJoinGroupChat()
        {
            if (GlobalData.Client.Available == Member.Status.Busy)
            {
                await GlobalData.Service.ChangeStatusAsync(Member.Status.Active);
            }

            await GlobalData.Service.JoinGroupChatAsync(CurrentChat.Name);
        }

        private bool CanCreateGroupChat()
        {
            Int32 count = 0;

            if (GlobalChat == null)
            {
                return false;
            }

            GroupChatList.ToList().ForEach(
                g => 
                {
                    if (g.Admin.Equals(GlobalData.Client))
                    {
                        count++;
                    }
                });

            return (count < Chat.MEMBER_MAX_GROUPS ? true : false);
        }

        private async void OnCreateGroupChat()
        {
            if (GlobalData.Client.Available == Member.Status.Busy)
            {
                await GlobalData.Service.ChangeStatusAsync(Member.Status.Active);
            }

            GroupDialogViewModel context = new GroupDialogViewModel();

            var dialog = new GroupDialog(context);
            dialog.ShowDialog();

            if (!context.Result.Equals("Create"))
            {
                return;
            }

            GroupChat chat = await GlobalData.Service.CreateGroupChatAsync(
                context.GroupName, 
                Int32.Parse(context.MinAge), 
                Int32.Parse(context.MaxSize));

            if (chat != null)
            {
                ChatGroupViewModel viewModel = _Parent.FindView<ChatGroupViewModel>();

                if (viewModel != null)
                {
                    viewModel.GroupChatList.Add(chat);
                    viewModel.CurrentChat = chat;
                    _Parent.CurrentView = viewModel;
                }
            }
        }

        private void Callback_MemberLeaveChat(MemberChatEventArgs args)
        {
            if (GlobalChat.Name.Equals(args.ChatName))
            {
                if (!args.Member.Equals(GlobalData.Client))
                {
                    GlobalChat.MemberList.Remove(args.Member);
                }
                else
                {
                    if (_Parent.CurrentView == this)
                    {
                        _Parent.NavigateBack();
                    }                  
                }
            }
            else
            {
                GroupChat chat = GroupChatList.ToList().Find(g => g.Name.Equals(args.ChatName));

                if (chat != null)
                {
                    chat.MemberList.Remove(args.Member);

                    if (chat.Admin.Equals(args.Member))
                    {
                        GroupChatList.Remove(chat);
                    }
                }
            }
        }

        private void Callback_MemberJoinChat(MemberChatEventArgs args)
        {
            if (GlobalChat.Name.Equals(args.ChatName))
            {
                GlobalChat.MemberList.Add(args.Member);
            }
            else
            {
                GroupChat chat = GroupChatList.ToList().Find(g => g.Name.Equals(args.ChatName));

                if (chat != null)
                {
                    chat.AddMember(args.Member);
                }
            }
        }

        private void Callback_GroupChatOpen(GroupChatOpenEventArgs args)
        {
            GroupChatList.Add(args.Chat);
        }

        private void Callback_MessageChat(MessageChatEventArgs args)
        {
            if (GlobalChat.Name.Equals(args.ChatName))
            {
                GlobalChat.MessageList.Add(args.Message);
            }
        }

        private void Callback_MemberStatus(MemberStatusEventArgs args)
        {
            Member m = GlobalChat.FindMember(args.Username);

            if (m != null)
            {
                m.Available = args.Available;
            }

            GroupChatList.ToList().ForEach(
                chat =>
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
                });
        }

        public override void Dispose()
        {
            base.Dispose();

            GlobalChat = null;
            CurrentChat = null;
            CurrentMember = null;
        }
    }
}
