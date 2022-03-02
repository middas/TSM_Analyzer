using System;
using System.Windows.Input;

namespace TSM_Analyzer.Mvvm
{
    public class RelayCommand : ICommand
    {
        private readonly Action action;

        public RelayCommand(Action action)
        {
            this.action = action;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            action?.Invoke();
        }
    }

    public class RelayCommand<T> : ICommand where T : class
    {
        private readonly Action<T> action;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<T> action)
        {
            this.action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            action?.Invoke(parameter as T);
        }
    }
}