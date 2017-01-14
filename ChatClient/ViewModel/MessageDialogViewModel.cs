using ChatClient.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ChatClient.ViewModel
{
    public class MessageDialogViewModel : ViewModelBase
    {
        public ICommand ClickCommand
        {
            get { return Get(() => ClickCommand); }
            set { Set(() => ClickCommand, value); }
        }

        public ObservableCollection<String> ButtonList
        {
            get { return Get(() => ButtonList); }
            set { Set(() => ButtonList, value); }
        }

        public String Title
        {
            get { return Get(() => Title); }
            set { Set(() => Title, value); }
        }

        public String Message
        {
            get { return Get(() => Message); }
            set { Set(() => Message, value); }
        }
        
        public Double Timeout
        {
            get { return Get(() => Timeout); }
            set { Set(() => Timeout, value); }
        }

        public String Result
        {
            get { return Get(() => Result); }
            set { Set(() => Result, value); }
        }

        public MessageDialogViewModel()
        {
            ButtonList = new ObservableCollection<String>();

            ClickCommand = new RelayCommand<List<Object>>(OnClick, CanClick);

            Timeout = 3600;
            Result = "Timeout";
        }

        private bool CanClick(List<Object> arg)
        {
            return true;
        }

        private void OnClick(List<Object> arg)
        {
            Result = arg[1] as String;

            Window wnd = arg[0] as Window;
            wnd.Close();
        }
    }
}
