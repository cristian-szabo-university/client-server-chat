using ChatClient.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ChatClient
{
    public partial class App : Application
    {
        public App()
        {
            AppContext.SetSwitch("Switch.System.IdentityModel.DisableMultipleDNSEntriesInSANCertificate", true);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (GlobalData.Service != null)
            {
                GlobalData.Service.Disconnect();
            }
        }
    }
}
