using System;
using System.ServiceModel;

namespace ChatLibrary
{
    public interface IChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyMemberStatus(String username, Member.Status available);

        [OperationContract(IsOneWay = true)]
        void NotifyMessageChat(String chatName, Message message);
        
        [OperationContract(IsOneWay = true)]
        void NotifyMemberJoinChat(String chatName, Member member);

        [OperationContract(IsOneWay = true)]
        void NotifyMemberLeaveChat(String chatName, Member member);

        [OperationContract(IsOneWay = true)]
        void NotifyGroupChatOpen(GroupChat chat);

        [OperationContract]
        bool JoinGroupChatRequest(Member member);

        [OperationContract(IsOneWay = true)]
        void JoinGroupChatResponse(GroupChat chat);

        [OperationContract]
        bool InvitePrivateChatRequest(Member member);

        [OperationContract(IsOneWay = true)]
        void InvitePrivateChatResponse(PrivateChat chat);
    }
}
