using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RF.WPF.MVVM
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _func;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="func">Action to be executed by the command</param>
        /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
        public AsyncCommand(Func<Task> func, Func<bool> canExecute)
        {
            _func = func;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="action">Action to be executed by the command</param>
        /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
        public AsyncCommand(Func<Task> func) : this(func, FuncCache.True) { }

        /// <summary>
        /// Wires CanExecuteChanged event 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) => _canExecute.Invoke();

        public async Task ExecuteAsync(object parameter) => await _func();
        public void Execute(object parameter) => ExecuteAsync(parameter);
    }
}
