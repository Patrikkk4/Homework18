using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bank.Model
{
    /// <summary>
    /// Класс принимает не типизрованный входной параметр и наследует интерфейс определения команд
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class RelayCommand<T> : ICommand
    {
        /// <summary>
        /// Поле выполнение логики команды
        /// </summary>
        private readonly Action<T> execute;

        /// <summary>
        /// Поле Определение, может ли команда быть выполнена
        /// </summary>
        private readonly Func<T, bool> canExecute;

        // Событие, вызываемое при изменении условий
        public event EventHandler CanExecuteChanged
        {
            // Определение, может ли условие повлиять на выполнение команды
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Конструктор принимает папараметры команды и определяет может ли она быть выполнена
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Метод определяет возможно ли выполнение команды
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.CanExecute(parameter);
        }

        /// <summary>
        /// Метод выполнения команды
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            execute((T)parameter);
        }
    }
}
