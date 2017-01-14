using ChatClient.Common;
using ChatClient.Service;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatClient.ViewModel.MemberViewModel
{
    public class MemberSignInViewModel : ValidationViewModelBase
    {
        private MemberPresenter _Parent;

        public override string Name
        {
            get
            {
                return "Sign In";
            }
        }

        public Boolean SignInActive
        {
            get { return Get(() => SignInActive); }
            set { Set(() => SignInActive, value); }
        }
        public ICommand SignInCommand
        {
            get { return Get(() => SignInCommand); }
            set { Set(() => SignInCommand, value, true); }
        }

        public String Username
        {
            get { return Get(() => Username); }
            set { Set(() => Username, value); }
        }
        public String Password
        {
            get { return Get(() => Password); }
            set { Set(() => Password, value, true); }
        }
        
        public MemberSignInViewModel(MemberPresenter parent)
        {
            _Parent = parent;

            SignInActive = false;
            SignInCommand = new RelayCommand<PasswordBox>(OnSignIn, CanSignIn);

            AddRule(() => Username, ValidateUsername, "Invalid username.");            
            AddRule(() => Password, ValidatePassword, "Invalid password.");
        }

        private bool ValidateUsername()
        {
            if (String.IsNullOrEmpty(Username))
            {
                throw new ArgumentNullException("Field can't be empty.");
            }

            if (Username.Length < 6)
            {
                throw new ApplicationException("Username should have at least 6 characters.");
            }

            if (Username.Length > 20)
            {
                throw new ApplicationException("Username should not contain more than 20 characters.");
            }

            return true;
        }

        private bool ValidatePassword()
        {
            if (String.IsNullOrEmpty(Password))
            {
                throw new Exception("Field can't be empty.");
            }

            if (Password.Length < 8)
            {
                throw new Exception("Password should have at least 8 characters.");
            }

            if (Password.Length > 16)
            {
                throw new Exception("Password should not contain more than 16 characters.");
            }

            return true;
        }

        private bool CanSignIn(PasswordBox arg)
        {
            if (SignInActive)
            {
                return false;
            }

            Password = arg.Password;

            if (HasErrors)
            {
                return false;
            }

            return true;
        }

        private async void OnSignIn(PasswordBox arg)
        {
            SignInActive = true;

            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.TransportWithMessageCredential, true);
            tcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            InstanceContext context = new InstanceContext(new ChatServiceCallback());

            EndpointAddress endpoint = new EndpointAddress(
                new Uri("net.tcp://" + _Parent.Address.ToString() + "/ChatService"), 
                EndpointIdentity.CreateDnsIdentity("Server"));

            GlobalData.Service = new ChatServiceClient(context, tcpBinding, endpoint);

            GlobalData.Service.ClientCredentials.ServiceCertificate.SetDefaultCertificate(
                StoreLocation.CurrentUser, 
                StoreName.My, 
                X509FindType.FindBySubjectName, 
                "Server");

            GlobalData.Service.ClientCredentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindBySubjectName,
                "Client");

            GlobalData.Service.ClientCredentials.UserName.UserName = Username;
            GlobalData.Service.ClientCredentials.UserName.Password = _Parent.ComputeMD5Hash(Password);

            try
            {
                GlobalData.Client = await GlobalData.Service.ConnectAsync();

                if (GlobalData.Client == null)
                {
                    throw new Exception("Account couldn't be found!");
                }

                OnPageNavigated(new ChatPage());
            }
            catch(Exception ex)
            {
                MessageDialogViewModel viewModel = new MessageDialogViewModel();
                viewModel.Title = "SignIn Error";
                viewModel.ButtonList.Add("OK");

                if (ex.InnerException != null)
                {
                    viewModel.Message = ex.InnerException.Message;
                }
                else
                {
                    viewModel.Message = ex.Message;
                }
                
                MessageDialog dialog = new MessageDialog(viewModel);
                dialog.ShowDialog();

                SignInActive = false;
            }
        }
    }
}
