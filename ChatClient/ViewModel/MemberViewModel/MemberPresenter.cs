using ChatClient.Common;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ChatClient.ViewModel.MemberViewModel
{
    public class MemberPresenter : ViewModelBase
    {
        public ObservableCollection<ViewModelBase> ViewList
        {
            get { return Get(() => ViewList); }
            set { Set(() => ViewList, value); }
        }
        public ViewModelBase CurrentView
        {
            get { return Get(() => CurrentView); }
            set { Set(() => CurrentView, value); }
        }
        
        public IPEndPoint Address { get; }

        public MemberPresenter(IPEndPoint address)
        {
            Address = address;

            ViewList = new ObservableCollection<ViewModelBase>();
            ViewList.Add(new MemberSignInViewModel(this));
            ViewList.Add(new MemberRegisterViewModel(this));

            CurrentView = ViewList[0];
        }

        public string ComputeMD5Hash(string input)
        {
            StringBuilder builder = new StringBuilder();

            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
            }
            
            return builder.ToString();
        }

        public T FindView<T>() where T : ViewModelBase
        {
            Type viewType = typeof(T);

            return (T)ViewList.ToList().Find(v => v.GetType().Equals(viewType));
        }
    }
}
