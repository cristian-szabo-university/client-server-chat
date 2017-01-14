using ChatClient.Common;
using System.ServiceModel;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using ChatClient.Service;
using System.Windows.Input;
using ChatLibrary;
using System.Net;

namespace ChatClient.ViewModel.ChatViewModel
{
    class ChatPrivateViewModel : ViewModelBase
    {
        private ChatPresenter _Parent;

        public override string Name
        {
            get
            {
                return "Private";
            }
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
        public ObservableCollection<PrivateChat> PrivateChatList
        {
            get { return Get(() => PrivateChatList); }
            set { Set(() => PrivateChatList, value); }
        }
        public PrivateChat CurrentChat
        {
            get { return Get(() => CurrentChat); }
            set { Set(() => CurrentChat, value); }
        }

        public ChatPrivateViewModel(ViewModelBase parent)
        {
            _Parent = parent as ChatPresenter;

            PrivateChatList = new ObservableCollection<PrivateChat>();

            SendMessageCommand = new RelayCommand<String>(OnSendMessage, CanSendMessage);
            LeaveChatCommand = new RelayCommand(OnLeaveChat, CanLeaveChat);

            InstanceContext context = GlobalData.Service.InnerDuplexChannel.CallbackInstance;
            ChatServiceCallback callback = context.GetServiceInstance() as ChatServiceCallback;
            callback.MemberLeaveChat += Callback_MemberLeaveChat; ;
            callback.PrivateChatInviteResponse += Callback_PrivateChatInviteResponse;
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

            if (CurrentChat.Person.Available == Member.Status.Busy)
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
            if (CurrentChat.Active == Chat.Status.Offline)
            {
                PrivateChatList.Remove(CurrentChat);

                if (PrivateChatList.Count == 0)
                {
                    _Parent.CurrentView = _Parent.FindView<ChatGlobalViewModel>();
                }
            }
            else
            {
                if (GlobalData.Client.Available == Member.Status.Busy)
                {
                    await GlobalData.Service.ChangeStatusAsync(Member.Status.Active);
                }

                await GlobalData.Service.LeavePrivateChatAsync(CurrentChat.Name);
            }
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

            PrivateChat chat = PrivateChatList.ToList().Find(g => g.Name.Equals(args.ChatName));

            if (chat == null)
            {
                return;
            }

            if (args.Member.Equals(GlobalData.Client))
            {
                PrivateChatList.Remove(chat);

                if (PrivateChatList.Count == 0)
                {
                    _Parent.CurrentView = _Parent.FindView<ChatGlobalViewModel>();
                }
            }
            else
            {
                chat.Active = Chat.Status.Offline;
            }
        }

        private void Callback_PrivateChatInviteResponse(InvitePrivateChatEventArgs args)
        {
            if (!args.Chat.Admin.Equals(GlobalData.Client))
            {
                args.Chat.Person = args.Chat.Admin;
            }

            PrivateChatList.Add(args.Chat);
            
            _Parent.CurrentView = this;
        }

        private void Callback_MessageChat(MessageChatEventArgs args)
        {
            PrivateChat chat = PrivateChatList.ToList().Find(g => g.Name.Equals(args.ChatName));

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
            PrivateChatList.ToList().ForEach(
                chat =>
                {
                    if (chat.Active == Chat.Status.Online)
                    {
                        if (chat.Admin.Username.Equals(args.Username))
                        {
                            chat.Admin.Available = args.Available;
                        }
                        else if (chat.Person.Username.Equals(args.Username))
                        {
                            chat.Person.Available = args.Available;
                        }
                    }
                });
        }
    }
}
