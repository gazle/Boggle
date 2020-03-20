using System;
using System.Windows.Input;

namespace Boggle
{
    public class DelegateCommand<T> : ICommand
    {
        readonly Action<T> _execute;
        readonly Predicate<T> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public void Execute(object parameter) => _execute((T)parameter);

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class DelegateCommand : ICommand
    {
        readonly Action _execute;
        readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public void Execute(object parameter) => _execute();

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}