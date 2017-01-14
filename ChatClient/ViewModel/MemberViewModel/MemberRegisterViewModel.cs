using ChatClient.Common;
using ChatClient.Service;
using ChatLibrary;
using System;
using System.ServiceModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatClient.ViewModel.MemberViewModel
{
    public class MemberRegisterViewModel : ValidationViewModelBase
    {
        private MemberPresenter _Parent;

        public override string Name
        {
            get
            {
                return "Register";
            }
        }

        public ICommand RegisterCommand
        {
            get { return Get(() => RegisterCommand); }
            set { Set(() => RegisterCommand, value); }
        }
        public Boolean RegisterActive
        {
            get { return Get(() => RegisterActive); }
            set { Set(() => RegisterActive, value); }
        }
        public String Username
        {
            get { return Get(() => Username); }
            set { Set(() => Username, value); }
        }
        public String Password
        {
            get { return Get(() => Password); }
            set { Set(() => Password, value); }
        }
        public DateTime Birthday
        {
            get { return Get(() => Birthday); }
            set { Set(() => Birthday, value); }
        }
        public Member.Gender Gender
        {
            get { return Get(() => Gender); }
            set { Set(() => Gender, value); }
        }

        public MemberRegisterViewModel(MemberPresenter parent)
        {
            _Parent = parent;

            RegisterActive = false;

            RegisterCommand = new RelayCommand<PasswordBox>(OnRegister, CanRegister);

            Birthday = new DateTime(DateTime.Now.Year - 12, DateTime.Now.Month, DateTime.Now.Day);

            AddRule(() => Username, ValidateUsername, "Invalid username.");
            AddRule(() => Password, ValidatePassword, "Invalid password.");
            AddRule(() => Birthday, ValidateBirthday, "Invalid birthday.");
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

        private bool ValidateBirthday()
        {
            if (String.IsNullOrEmpty(Birthday.ToString()))
            {
                throw new Exception("Field can't be empty.");
            }

            if (DateTime.Today.Year - Birthday.Year < 12)
            {
                throw new Exception("You need to be at least 12 years old.");
            }

            return true;
        }

        private bool CanRegister(PasswordBox arg)
        {
            if (RegisterActive)
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

        private async void OnRegister(PasswordBox arg)
        {
            RegisterActive = true;

            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);

            EndpointAddress endpoint = new EndpointAddress(
                new Uri("net.tcp://" + _Parent.Address.Address + ":" + (_Parent.Address.Port + 11) + "/MemberService"));

            MemberServiceClient service = new MemberServiceClient(tcpBinding, endpoint);

            try
            {
                await service.RegisterMemberAsync(Username, _Parent.ComputeMD5Hash(Password), Gender, Birthday);

                _Parent.CurrentView = _Parent.FindView<MemberSignInViewModel>();
            }
            catch (Exception ex)
            {
                MessageDialogViewModel viewModel = new MessageDialogViewModel();
                viewModel.Title = "Register Error";
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

                RegisterActive = false;
            }
        }
    }
}
