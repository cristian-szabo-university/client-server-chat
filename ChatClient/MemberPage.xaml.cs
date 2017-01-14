using ChatClient.ViewModel.MemberViewModel;
using System.Net;
using System.Windows.Controls;

namespace ChatClient
{
    public partial class MemberPage : Page
    {
        public MemberPage(IPEndPoint address)
        {
            InitializeComponent();

            DataContext = new MemberPresenter(address);
        }
    }
}
