using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using ChatLibrary;
using System;

namespace ChatClient.Service
{
    public partial class ChatServiceClient : DuplexClientBase<IChatContractAsync>, IChatContractAsync
    {
        public ChatServiceClient(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        { }

        public void ChangeStatus(Member.Status available)
        {
            Channel.ChangeStatus(available);
        }

        public Task ChangeStatusAsync(Member.Status available)
        {
            return Channel.ChangeStatusAsync(available);
        }

        public Member Connect()
        {
            return Channel.Connect();
        }

        public Task<Member> ConnectAsync()
        {
            return Channel.ConnectAsync();
        }

        public GroupChat CreateGroupChat(string chatName, int minAge, int maxSize)
        {
            return Channel.CreateGroupChat(chatName, minAge, maxSize);
        }

        public Task<GroupChat> CreateGroupChatAsync(string chatName, int minAge, int maxSize)
        {
            return Channel.CreateGroupChatAsync(chatName, minAge, maxSize);
        }

        public void Disconnect()
        {
            Channel.Disconnect();
        }

        public Task DisconnectAsync()
        {
            return Channel.DisconnectAsync();
        }

        public void UpdateGroupChatList()
        {
            Channel.UpdateGroupChatList();
        }

        public Task UpdateGroupChatListAsync()
        {
            return Channel.UpdateGroupChatListAsync();
        }

        public void UpdateMemberList()
        {
            Channel.UpdateMemberList();
        }

        public Task UpdateMemberListAsync()
        {
            return Channel.UpdateMemberListAsync();
        }

        public void UpdateMessageListFrom(DateTime deliverTime)
        {
            Channel.UpdateMessageListFrom(deliverTime);
        }

        public Task UpdateMessageListFromAsync(DateTime deliverTime)
        {
            return Channel.UpdateMessageListFromAsync(deliverTime);
        }

        public PrivateChat InvitePrivateChat(string personName)
        {
            return Channel.InvitePrivateChat(personName);
        }

        public Task<PrivateChat> InvitePrivateChatAsync(string personName)
        {
            return Channel.InvitePrivateChatAsync(personName);
        }

        public void JoinGroupChat(string chatName)
        {
            Channel.JoinGroupChat(chatName);
        }

        public Task JoinGroupChatAsync(string chatName)
        {
            return Channel.JoinGroupChatAsync(chatName);
        }

        public void KickOutMember(string memberName, string chatName)
        {
            Channel.KickOutMember(memberName, chatName);
        }

        public Task KickOutMemberAsync(string memberName, string chatName)
        {
            return Channel.KickOutMemberAsync(memberName, chatName);
        }

        public void LeaveGroupChat(string chatName)
        {
            Channel.LeaveGroupChat(chatName);
        }

        public Task LeaveGroupChatAsync(string chatName)
        {
            return Channel.LeaveGroupChatAsync(chatName);
        }

        public void LeavePrivateChat(string chatName)
        {
            Channel.LeavePrivateChat(chatName);
        }

        public Task LeavePrivateChatAsync(string chatName)
        {
            return Channel.LeavePrivateChatAsync(chatName);
        }

        public void MessageChat(string chatName, string text)
        {
            Channel.MessageChat(chatName, text);
        }

        public Task MessageChatAsync(string chatName, string text)
        {
            return Channel.MessageChatAsync(chatName, text);
        }
    }
}
