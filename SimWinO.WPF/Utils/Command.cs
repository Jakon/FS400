using System;
using System.Windows.Input;

namespace SimWinO.FlightSimulator
{
    public class Command : ICommand
    {
        private Action<object> CommandAction { get; }
        private Func<object, bool> CanExecuteFunc { get; }

        public Command(Action<object> action, Func<object, bool> canExecute = null)
        {
            CommandAction = action;
            CanExecuteFunc = canExecute;
        }

        public Command(Action action, Func<bool> canExecute = null)
        {
            CommandAction = _ => action();
            CanExecuteFunc = _ => canExecute?.Invoke() ?? true;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            CommandAction(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
