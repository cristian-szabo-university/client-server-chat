using ChatClient.Common;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Input;

namespace ChatClient.ViewModel
{
    class HostViewModel : ValidationViewModelBase
    {
        public String AddressA
        {
            get { return Get(() => AddressA); }
            set { Set(() => AddressA, value); }
        }
        public String AddressB
        {
            get { return Get(() => AddressB); }
            set { Set(() => AddressB, value); }
        }
        public String AddressC
        {
            get { return Get(() => AddressC); }
            set { Set(() => AddressC, value); }
        }
        public String AddressD
        {
            get { return Get(() => AddressD); }
            set { Set(() => AddressD, value); }
        }
        public String Port
        {
            get { return Get(() => Port); }
            set { Set(() => Port, value); }
        }

        public Boolean ConnectActive
        {
            get { return Get(() => ConnectActive); }
            set { Set(() => ConnectActive, value); }
        }
        public ICommand ConnectCommand
        {
            get { return Get(() => ConnectCommand); }
            set { Set(() => ConnectCommand, value); }
        }

        public HostViewModel()
        {
            ConnectActive = false;

            ConnectCommand = new RelayCommand(OnConnect, CanConnect);

            AddressA = "127";
            AddressB = "0";
            AddressC = "0";
            AddressD = "1";
            Port = "7777";

            AddRule<String, String>(() => AddressA, ValidateAddress, "Value is invalid.");
            AddRule<String, String>(() => AddressB, ValidateAddress, "Value is invalid.");
            AddRule<String, String>(() => AddressC, ValidateAddress, "Value is invalid.");
            AddRule<String, String>(() => AddressD, ValidateAddress, "Value is invalid.");
            AddRule(() => Port, ValidatePort, "Value is invalid.");
        }

        private bool ValidateAddress(String address)
        {
            Int32 result;

            if (String.IsNullOrEmpty(address))
            {
                throw new Exception("Field can't be empty.");
            }

            if (!Int32.TryParse(address, out result))
            {
                throw new Exception("Only numeric characters are accepted.");
            }

            if (result < 0 || result > 255)
            {
                throw new Exception("Only values between 0 and 255.");
            }

            return true;
        }

        private bool ValidatePort()
        {
            Int32 result;

            if (String.IsNullOrEmpty(Port))
            {
                throw new Exception("Field can't be empty.");
            }

            if (!Int32.TryParse(Port, out result))
            {
                throw new Exception("Only numeric characters are accepted.");
            }

            if (result < IPEndPoint.MinPort || result > IPEndPoint.MaxPort)
            {
                throw new Exception(String.Format("Port has to be between {0} and {1}.", IPEndPoint.MinPort, IPEndPoint.MaxPort));
            }

            return true;
        }

        private bool CanConnect()
        {
            IPAddress result;

            if (HasErrors || ConnectActive)
            {
                return false;
            }

            String addressStr = AddressA + "." + AddressB + "." + AddressC + "." + AddressD;

            if (!IPAddress.TryParse(addressStr, out result))
            {
                return false;
            }

            if (result.Equals(IPAddress.Any) || 
                result.Equals(IPAddress.Broadcast))
            {
                return false;
            }
            
            return true;
        }

        private void OnConnect()
        {
            ConnectActive = true;

            Int32 hostPort = Int32.Parse(Port);
            IPAddress hostAddress = IPAddress.Parse(AddressA + "." + AddressB + "." + AddressC + "." + AddressD);

            Ping hostPing = new Ping();
            hostPing.PingCompleted += HostPing_PingCompleted;
            hostPing.SendAsync(hostAddress, 2000, new IPEndPoint(hostAddress, hostPort));
        }

        private void HostPing_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            PingReply reply = e.Reply;

            if (reply.Status == IPStatus.Success)
            {
                OnPageNavigated(new MemberPage(e.UserState as IPEndPoint));
            }
            else
            {
                MessageDialogViewModel viewModel = new MessageDialogViewModel();
                viewModel.Title = "Host Error";               
                viewModel.Message = "Couldn't reach host address.";
                viewModel.ButtonList.Add("OK");

                MessageDialog dialog = new MessageDialog(viewModel);
                dialog.ShowDialog();

                ConnectActive = false;
            }
        }
    }
}
