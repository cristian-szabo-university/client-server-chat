using ChatLibrary;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ChatServer
{
    internal class MemberApplication : Application
    {
        private ServiceEndpoint _Endpoint;

        public override Uri Address
        {
            get
            {
                return _Endpoint.ListenUri;
            }
        }

        public MemberApplication(IPAddress address, int port) : base(address, port)
        { }

        public void Open(Database db)
        {
            if (IsReady())
            {
                throw new InvalidOperationException("The application can't be re-initialised because it was initialised before.");
            }

            IMemberContract service = new MemberService(db);
            _Host = new ServiceHost(service, _Address);

            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);

            _Endpoint = _Host.AddServiceEndpoint(typeof(IMemberContract), tcpBinding, "MemberService");

            base.Open();
        }
    }
}