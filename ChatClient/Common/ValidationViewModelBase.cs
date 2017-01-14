using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace ChatClient.Common
{
    public class ValidationViewModelBase : ViewModelBase, IDataErrorInfo
    {
        private Dictionary<string, BinderBase> ruleMap = new Dictionary<string, BinderBase>();

        public void AddRule<T>(Expression<Func<T>> expression, Func<object, bool> ruleDelegate, string errorMessage)
        {
            AddRule(expression, new BinderBase(ruleDelegate, errorMessage));
        }

        public void AddRule<T>(Expression<Func<T>> expression, Func<bool> ruleDelegate, string errorMessage)
        {
            AddRule(expression, new Binder(ruleDelegate, errorMessage));
        }

        public void AddRule<T, U>(Expression<Func<T>> expression, Func<U, bool> ruleDelegate, string errorMessage)
        {
            AddRule(expression, new Binder<U>(ruleDelegate, errorMessage));
        }

        public void AddRule<T>(Expression<Func<T>> expression, BinderBase binder)
        {
            var name = GetPropertyName(expression);

            ruleMap.Add(name, binder);
        }

        protected override void Set<T>(Expression<Func<T>> expression, T value, bool forceUpdate)
        {
            var name = GetPropertyName(expression);
            
            if (ruleMap.ContainsKey(name))
            {
                ruleMap[name].IsDirty = true;
            }

            base.Set<T>(expression, value, forceUpdate);
        }

        public bool HasErrors
        {
            get
            {
                var values = ruleMap.ToList();
                values.ForEach(b => b.Value.Update(Get(b.Key)));

                return values.Any(b => b.Value.HasError);
            }
        }

        public string Error
        {
            get
            {
                var errors = from b in ruleMap.Values where b.HasError select b.Error;

                return string.Join("\n", errors);
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (ruleMap.ContainsKey(columnName))
                {
                    ruleMap[columnName].Update(Get(columnName));
                    return ruleMap[columnName].Error;
                }

                return null;
            }
        }
    }

    public class BinderBase
    {
        private readonly Func<object, bool> _RuleDelegate;
        private readonly String _Message;

        public BinderBase(Func<object, bool> ruleDelegate, string message)
        {
            _RuleDelegate = ruleDelegate;
            _Message = message;

            IsDirty = true;
        }

        public string Error { get; set; }
        public bool HasError { get; set; }
        public bool IsDirty { get; set; }

        public void Update(object value)
        {
            if (!IsDirty)
            {
                return;
            }

            Error = null;
            HasError = false;

            try
            {
                if (!_RuleDelegate(value))
                {
                    Error = _Message;
                    HasError = true;
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
                HasError = true;
            }
        }
    }

    public class Binder : BinderBase
    {
        public Binder(Func<bool> ruleDelegate, string message)
            : base(o => ruleDelegate(), message)
        {
            if (ruleDelegate == null)
                throw new ArgumentNullException("ruleDelegate", "Delegate method cannot be null.");
        }
    }

    public class Binder<T> : BinderBase
    {
        public Binder(Func<T, bool> ruleDelegate, string message)
            : base(o => ruleDelegate((T)o), message)
        {
            if (ruleDelegate == null)
                throw new ArgumentNullException("ruleDelegate", "Delegate method cannot be null.");

            Type type = typeof(T);

            if (type.IsValueType && (!type.IsGenericType || !typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
                throw new InvalidCastException("T for Binder<T> is not an object nor Nullable.");
        }
    }
}
