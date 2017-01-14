using ChatLibrary;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace ChatClient.Service
{
    class MemberServiceClient : ClientBase<IMemberContractAsync>, IMemberContractAsync
    {
        public MemberServiceClient(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }

        public void RegisterMember(string username, string password, Member.Gender orientation, DateTime birthday)
        {
            Channel.RegisterMemberAsync(username, password, orientation, birthday);
        }

        public Task RegisterMemberAsync(string username, string password, Member.Gender orientation, DateTime birthday)
        {
            return Channel.RegisterMemberAsync(username, password, orientation, birthday);
        }
    }
}
