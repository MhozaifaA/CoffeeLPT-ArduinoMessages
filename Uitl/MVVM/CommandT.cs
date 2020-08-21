using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeLPT.Uitl.MVVM
{
    public sealed class CommandT<T> : Command
    {

        public CommandT(Action<T> execute, bool keepTargetAlive = false)
            : this(execute, (Func<T, bool>)null)
        {
        }

        public CommandT(Action<T> execute)
            : base((Action<object>)(o =>
            {
                if (!CommandT<T>.IsValidParameter(o))
                    return;
                execute((T)o);
            }))
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
        }

        public CommandT(Action<T> execute, Func<T, bool> canExecute)
            : base((Action<object>)(o =>
            {
                if (!CommandT<T>.IsValidParameter(o))
                    return;
                execute((T)o);
            }), (Func<object, bool>)(o =>
            {
                if (CommandT<T>.IsValidParameter(o))
                    return canExecute((T)o);
                return false;
            }))
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));
        }

        private static bool IsValidParameter(object o)
        {
            if (o != null)
                return o is T;
            Type type = typeof(T);
            if (Nullable.GetUnderlyingType(type) != (Type)null)
                return true;
            return !type.GetTypeInfo().IsValueType;
        }
    }
}
