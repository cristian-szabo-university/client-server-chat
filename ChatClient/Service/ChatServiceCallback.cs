using ChatLibrary;
using System;
using System.ServiceModel;

namespace ChatClient.Service
{
    public class MessageChatEventArgs : EventArgs
    {
        public String ChatName;

        public Message Message;
    }

    public class MemberChatEventArgs : EventArgs
    {
        public String ChatName;

        public Member Member;
    }

    public class GroupChatOpenEventArgs : EventArgs
    {
        public GroupChat Chat;
    }

    public class JoinGroupChatEventArgs : EventArgs
    {
        public GroupChat Chat;

        public Member Member;
    }

    public class InvitePrivateChatEventArgs : EventArgs
    {
        public PrivateChat Chat;

        public Member Member;
    }

    public class MemberStatusEventArgs : EventArgs
    {
        public String Username;

        public Member.Status Available;
    }

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChatServiceCallback : IChatCallback
    {
        public event Action<MemberStatusEventArgs> MemberStatus;
        public event Action<MessageChatEventArgs> MessageChat;

        public event Action<MemberChatEventArgs> MemberJoinChat;
        public event Action<MemberChatEventArgs> MemberLeaveChat;

        public event Action<GroupChatOpenEventArgs> GroupChatOpen;

        public event Predicate<JoinGroupChatEventArgs> GroupChatJoinRequest;
        public event Predicate<InvitePrivateChatEventArgs> PrivateChatInviteRequest;

        public event Action<JoinGroupChatEventArgs> GroupChatJoinResponse;
        public event Action<InvitePrivateChatEventArgs> PrivateChatInviteResponse;

        public void NotifyMemberJoinChat(String chatName, Member member)
        {
            if (MemberJoinChat != null)
            {
                MemberJoinChat(new MemberChatEventArgs() { ChatName = chatName, Member = member });
            }
        }

        public void NotifyMemberLeaveChat(String chatName, Member member)
        {
            if (MemberLeaveChat != null)
            {
                MemberLeaveChat(new MemberChatEventArgs() { ChatName = chatName, Member = member });
            }
        }

        public void NotifyGroupChatOpen(GroupChat chat)
        {
            if (GroupChatOpen != null)
            {
                GroupChatOpen(new GroupChatOpenEventArgs() { Chat = chat });
            }
        }
        
        public bool JoinGroupChatRequest(Member member)
        {
            if (GroupChatJoinRequest != null)
            {
                return GroupChatJoinRequest(new JoinGroupChatEventArgs() { Member = member, Chat = null });
            }

            return false;
        }

        public void JoinGroupChatResponse(GroupChat chat)
        {
            if (GroupChatJoinResponse != null)
            {
                GroupChatJoinResponse(new JoinGroupChatEventArgs() { Chat = chat, Member = null });
            }
        }

        public bool InvitePrivateChatRequest(Member member)
        {
            if (PrivateChatInviteRequest != null)
            {
                return PrivateChatInviteRequest(new InvitePrivateChatEventArgs() { Member = member, Chat = null });
            }

            return false;
        }

        public void InvitePrivateChatResponse(PrivateChat chat)
        {
            if (PrivateChatInviteResponse != null)
            {
                PrivateChatInviteResponse(new InvitePrivateChatEventArgs() { Chat = chat, Member = null });
            }
        }

        public void NotifyMessageChat(string chatName, Message message)
        {
            if (MessageChat != null)
            {
                MessageChat(new MessageChatEventArgs() { ChatName = chatName, Message = message });
            }
        }

        public void NotifyMemberStatus(string username, Member.Status available)
        {
            if (MemberStatus != null)
            {
                MemberStatus(new MemberStatusEventArgs() { Username = username, Available = available });
            }
        }
    }
}
