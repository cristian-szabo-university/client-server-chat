using ChatLibrary;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace ChatServer
{
    internal class ChatApplication : Application
    {
        private ServiceEndpoint _Endpoint;

        public override Uri Address
        {
            get
            {
                return _Endpoint.ListenUri;
            }
        }

        public ChatApplication(IPAddress address, Int32 port) : base (address, port)
        { }

        public void Open(Database db)
        {
            if (IsReady())
            {
                throw new InvalidOperationException("The application can't be re-initialised because it was initialised before.");
            }

            ChatService service = new ChatService(db);
            _Host = new ServiceHost(service, _Address);

            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.TransportWithMessageCredential, true);
            tcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            _Host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            _Host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new MemberValidator(db);

            _Host.Credentials.ServiceCertificate.SetCertificate(
                StoreLocation.CurrentUser, StoreName.My,
                X509FindType.FindBySubjectName, "Server");

            _Host.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser, StoreName.My,
                X509FindType.FindBySubjectName, "Client");

            _Endpoint = _Host.AddServiceEndpoint(typeof(IChatContract), tcpBinding, "ChatService");

            base.Open();
        }
    }
}
