using ChatClient.Common;
using ChatLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ChatClient.ViewModel
{
    public class GroupDialogViewModel : ValidationViewModelBase
    {
        public ICommand ClickCommand
        {
            get { return Get(() => ClickCommand); }
            set { Set(() => ClickCommand, value); }
        }
        public ICommand CreateCommand
        {
            get { return Get(() => CreateCommand); }
            set { Set(() => CreateCommand, value); }
        }

        public String GroupName
        {
            get { return Get(() => GroupName); }
            set { Set(() => GroupName, value); }
        }
        public String MinAge
        {
            get { return Get(() => MinAge); }
            set { Set(() => MinAge, value); }
        }
        public String MaxSize
        {
            get { return Get(() => MaxSize); }
            set { Set(() => MaxSize, value); }
        }

        public String Result
        {
            get { return Get(() => Result); }
            set { Set(() => Result, value); }
        }

        public GroupDialogViewModel()
        {
            ClickCommand = new RelayCommand<List<Object>>(OnClick, (o) => true);
            CreateCommand = new RelayCommand<List<Object>>(OnClick, CanCreate);

            AddRule(() => GroupName, ValidateGroupName, "Invalid group name.");
            AddRule(() => MinAge, ValidateMinAge, "Invalid min age.");
            AddRule(() => MaxSize, ValidateMaxSize, "Invalid max size.");

            MinAge = "0";
            MaxSize = GroupChat.GROUP_MIN_SIZE.ToString();
            Result = "Unknown";
        }

        private bool ValidateMaxSize()
        {
            Int32 result;

            if (String.IsNullOrEmpty(MaxSize))
            {
                throw new Exception("Field can't be empty.");
            }

            if (!Int32.TryParse(MaxSize, out result))
            {
                throw new Exception("Only numeric characters are accepted.");
            }

            if (result < GroupChat.GROUP_MIN_SIZE || result > GroupChat.GROUP_MAX_SIZE)
            {
                throw new Exception(String.Format("Port has to be between {0} and {1}.", GroupChat.GROUP_MIN_SIZE, GroupChat.GROUP_MAX_SIZE));
            }

            return true;
        }

        private bool ValidateMinAge()
        {
            Int32 result;

            if (String.IsNullOrEmpty(MinAge))
            {
                throw new Exception("Field can't be empty.");
            }

            if (!Int32.TryParse(MinAge, out result))
            {
                throw new Exception("Only numeric characters are accepted.");
            }

            Int32 age = (DateTime.Today.Year - GlobalData.Client.Birthday.Year);
            Int32 maxAge = age + age % 8;

            if (result < 0 || result > maxAge)
            {
                throw new Exception(String.Format("Port has to be between {0} and {1}.", 0, maxAge));
            }

            return true;
        }

        private bool ValidateGroupName()
        {
            if (String.IsNullOrEmpty(GroupName))
            {
                throw new ArgumentNullException("Field can't be empty.");
            }

            if (GroupName.Length < 0)
            {
                throw new ApplicationException("Username should have at least 1 character.");
            }

            if (GroupName.Length > 5)
            {
                throw new ApplicationException("Username should not contain more than 6 characters.");
            }

            return true;
        }

        private bool CanCreate(List<Object> arg)
        {
            if (HasErrors)
            {
                return false;
            }

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
