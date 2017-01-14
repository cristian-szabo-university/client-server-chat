using System;
using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceContract(
        Name = "MemberContract",
        Namespace = "http://videogamelab.co.uk/")]
    public interface IMemberContract
    {
        [OperationContract]
        void RegisterMember(String username, String password, Member.Gender orientation, DateTime birthday);
    }
}
