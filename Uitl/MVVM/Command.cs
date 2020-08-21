using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoffeeLPT.Uitl.MVVM
{
    public class Command : ICommand

    {
        private readonly WeakEventManager _weakEventManager = new WeakEventManager();
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;
        public Command(Action<object> execute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            this._execute = execute;
        }

        public Command(Action execute)
          : this((Action<object>)(o => execute()))
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
        }

        public Command(Action<object> execute, Func<object, bool> canExecute)
          : this(execute)
        {
            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));
            this._canExecute = canExecute;
        }

        public Command(Action execute, Func<bool> canExecute)
          : this((Action<object>)(o => execute()), (Func<object, bool>)(o => canExecute()))
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));
        }

        /// <param name="parameter">An <see cref="T:System.Object" /> used as parameter to determine if the Command can be executed.</param>
        /// <summary>Returns a <see cref="T:System.Boolean" /> indicating if the Command can be exectued with the given parameter.</summary>
        /// <returns>
        /// <see langword="true" /> if the Command can be executed, <see langword="false" /> otherwise.</returns>
        /// <remarks>
        ///     <para>If no canExecute parameter was passed to the Command constructor, this method always returns <see langword="true" />.</para>
        ///     <para>If the Command was created with non-generic execute parameter, the parameter of this method is ignored.</para>
        /// </remarks>
        public bool CanExecute(object parameter)
        {
            if (this._canExecute != null)
                return this._canExecute(parameter);
            return true;
        }

        /// <summary>Occurs when the target of the Command should reevaluate whether or not the Command can be executed.</summary>
        /// <remarks>To be added.</remarks>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                this._weakEventManager.AddEventHandler(value, "CanExecuteChanged");
            }
            remove
            {
                this._weakEventManager.RemoveEventHandler(value, "CanExecuteChanged");
            }
        }

        /// <param name="parameter">An <see cref="T:System.Object" /> used as parameter for the execute Action.</param>
        /// <summary>Invokes the execute Action</summary>
        /// <remarks>
        ///     <para>If the Command was created with non-generic execute parameter, the parameter of this method is ignored.</para>
        /// </remarks>
        public void Execute(object parameter)
        {
            this._execute(parameter);
        }

        /// <summary>Send a <see cref="E:System.Windows.Input.ICommand.CanExecuteChanged" /></summary>
        /// <remarks>To be added.</remarks>
        public void ChangeCanExecute()
        {
            this._weakEventManager.HandleEvent((object)this, (object)EventArgs.Empty, "CanExecuteChanged");
        }
    }
}
