using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace ChatClient.Common
{
    public abstract class ViewModelBase : NotifycationObject, INotifyPropertyChanged, IDisposable
    {
        public virtual string Name
        {
            get
            {
                return "View Model Unknown";
            }
        }

        public event EventHandler<Page> PageNavigated;
        
        protected void OnPageNavigated(Page page)
        {
            PageNavigated?.Invoke(this, page);
        }

        public virtual void Dispose()
        {

        }
    }
}
