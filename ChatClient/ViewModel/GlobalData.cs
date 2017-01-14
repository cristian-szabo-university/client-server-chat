using ChatClient.Service;
using ChatLibrary;

namespace ChatClient.ViewModel
{
    public static class GlobalData
    {
        public static ChatServiceClient Service { get; set; }

        public static Member Client { get; set; }
    }
}
