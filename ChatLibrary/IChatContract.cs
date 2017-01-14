using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceContract(
        Name = "ChatContract",
        Namespace = "http://videogamelab.co.uk/",
        CallbackContract = typeof(IChatCallback), 
        SessionMode = SessionMode.Required)]
    public interface IChatContract
    {
        [OperationContract]
        Member Connect();

        [OperationContract(IsOneWay = true)]
        void Disconnect();

        [OperationContract(IsOneWay = true)]
        void UpdateMemberList();

        [OperationContract(IsOneWay = true)]
        void UpdateGroupChatList();

        [OperationContract(IsOneWay = true)]
        void UpdateMessageListFrom(DateTime deliverTime);

        [OperationContract(IsOneWay = true)]
        void ChangeStatus(Member.Status available);

        [OperationContract(IsOneWay = true)]
        void MessageChat(String chatName, String text);
        
        [OperationContract]
        GroupChat CreateGroupChat(String chatName, Int32 minAge, Int32 maxSize);

        [OperationContract(IsOneWay = true)]
        void JoinGroupChat(String chatName);

        [OperationContract(IsOneWay = true)]
        void KickOutMember(String memberName, String chatName);

        [OperationContract(IsOneWay = true)]
        void LeaveGroupChat(String chatName);

        [OperationContract]
        PrivateChat InvitePrivateChat(String personName);

        [OperationContract(IsOneWay = true)]
        void LeavePrivateChat(String chatName);
    }
}
