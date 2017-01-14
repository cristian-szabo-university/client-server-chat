using ChatLibrary;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ChatClient.Service
{
    [ServiceContract(
        CallbackContract = typeof(IChatCallback),
        SessionMode = SessionMode.Required)]
    public interface IChatContractAsync : IChatContract
    {
        [OperationContract(
            Action = "http://videogamelab.co.uk/ChatContract/Connect",
            ReplyAction = "http://videogamelab.co.uk/ChatContract/ConnectResponse")]
        Task<Member> ConnectAsync();

        [OperationContract(IsOneWay = true,
            Action = "http://videogamelab.co.uk/ChatContract/Disconnect")]
        Task DisconnectAsync();

        [OperationContract(IsOneWay = true,
            Action = "http://videogamelab.co.uk/ChatContract/UpdateMemberList")]
        Task UpdateMemberListAsync();

        [OperationContract(IsOneWay = true,
            Action = "http://videogamelab.co.uk/ChatContract/UpdateGroupChatList")]
        Task UpdateGroupChatListAsync();

        [OperationContract(
            Action = "http://videogamelab.co.uk/ChatContract/CreateGroupChat",
            ReplyAction = "http://videogamelab.co.uk/ChatContract/CreateGroupChatResponse")]
        Task<GroupChat> CreateGroupChatAsync(String chatName, Int32 minAge, Int32 maxSize);

        [OperationContract(IsOneWay = true,
            Action = "http://videogamelab.co.uk/ChatContract/JoinGroupChat")]
        Task JoinGroupChatAsync(String chatName);

        [OperationContract(IsOneWay = true,
            Action = "http://videogamelab.co.uk/ChatContract/KickOutMember")]
        Task KickOutMemberAsync(String memberName, String chatName);

        [OperationContract(IsOneWay = true,
            Action = "http://videogamelab.co.uk/ChatContract/LeaveGroupChat")]
        Task LeaveGroupChatAsync(String chatName);

        [OperationContract(
            Action = "http://videogamelab.co.uk/ChatContract/InvitePrivateChat",
            ReplyAction = "http://videogamelab.co.uk/ChatContract/InvitePrivateChatResponse")]
        Task<PrivateChat> InvitePrivateChatAsync(String personName);

        [OperationContract(IsOneWay = true,
            Action = "http://videogamelab.co.uk/ChatContract/LeavePrivateChat")]
        Task LeavePrivateChatAsync(String chatName);

        [OperationContract(IsOneWay = true,
            Action = "http://videogamelab.co.uk/ChatContract/MessageChat")]
        Task MessageChatAsync(String chatName, String text);

        [OperationContract(IsOneWay = true,
            Action = "http://videogamelab.co.uk/ChatContract/ChangeStatus")]
        Task ChangeStatusAsync(Member.Status available);

        [OperationContract(IsOneWay = true,
            Action = "http://videogamelab.co.uk/ChatContract/UpdateMessageListFrom")]
        Task UpdateMessageListFromAsync(DateTime deliverTime);
    }
}
