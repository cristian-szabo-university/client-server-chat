using System;
using System.Windows.Input;

namespace ChatClient.Common
{
    public abstract class RelayCommandBase : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        protected RelayCommandBase(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
    
    public class RelayCommand : RelayCommandBase
    {
        public RelayCommand(Action executeMethod)
            : this(executeMethod, () => true)
        {
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException("executeMethod", "Execute method cannot be null.");
        }

        public void Execute()
        {
            Execute(null);
        }

        public bool CanExecute()
        {
            return CanExecute(null);
        }
    }

    public class RelayCommand<T> : RelayCommandBase
    {
        public RelayCommand(Action<T> executeMethod)
            : this(executeMethod, o => true)
        {
        }

        public RelayCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base(o => executeMethod((T)o), o => canExecuteMethod((T)o))
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException("executeMethod", "Execute method cannot be null.");

            Type type = typeof(T);

            if (type.IsValueType && (!type.IsGenericType || !typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
                throw new InvalidCastException("T for RelayCommand<T> is not an object nor Nullable.");
        }

        public bool CanExecute(T parameter)
        {
            return CanExecute((object)parameter);
        }

        public void Execute(T parameter)
        {
            Execute((object)parameter);
        }
    }
}
