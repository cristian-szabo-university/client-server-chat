using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace ChatClient
{
    [DataContract]
    public abstract class NotifycationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        private Dictionary<string, object> _PropertyMap = new Dictionary<string, object>();

        private void InternalPropertyUpdate(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> path)
        {
            string propertyName = GetPropertyName(path);

            InternalPropertyUpdate(propertyName);
        }

        protected object Get(String propertyName)
        {
            object result = null;

            if (_PropertyMap.ContainsKey(propertyName))
            {
                result = _PropertyMap[propertyName];
            }

            return result;
        }

        protected T Get<T>(Expression<Func<T>> path)
        {
            return Get(path, default(T));
        }

        protected virtual T Get<T>(Expression<Func<T>> path, T defaultValue)
        {
            var propertyName = GetPropertyName(path);

            if (_PropertyMap.ContainsKey(propertyName))
            {
                return (T)_PropertyMap[propertyName];
            }
            else
            {
                _PropertyMap.Add(propertyName, defaultValue);
                return defaultValue;
            }
        }

        protected void Set<T>(Expression<Func<T>> path, T value)
        {
            Set(path, value, false);
        }

        protected virtual void Set<T>(Expression<Func<T>> path, T value, bool forceUpdate)
        {
            var oldValue = Get(path);
            var propertyName = GetPropertyName(path);

            if (!object.Equals(value, oldValue) || forceUpdate)
            {
                _PropertyMap[propertyName] = value;
                OnPropertyChanged(path);
            }
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            Expression body = expression.Body;
            MemberExpression memberExpression = body as MemberExpression;

            if (memberExpression == null)
            {
                memberExpression = (MemberExpression)((UnaryExpression)body).Operand;
            }

            return memberExpression.Member.Name;
        }
    }
}
