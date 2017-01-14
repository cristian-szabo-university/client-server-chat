using ChatLibrary;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ChatClient.Service
{
    [ServiceContract]
    public interface IMemberContractAsync : IMemberContract
    {
        [OperationContract(
            Action = "http://videogamelab.co.uk/MemberContract/RegisterMember",
            ReplyAction = "http://videogamelab.co.uk/MemberContract/RegisterMemberResponse")]
        Task RegisterMemberAsync(String username, String password, Member.Gender orientation, DateTime birthday);
    }
}
