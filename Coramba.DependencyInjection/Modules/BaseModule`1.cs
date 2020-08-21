using System;

namespace Coramba.DependencyInjection.Modules
{
    public abstract class BaseModule<TComponent> : BaseModule
        where TComponent: new()
    {
        protected TComponent Component { get; set; } = new TComponent();

        protected void WithComponent(Action<TComponent> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            action(Component);
        }

        protected void IsNotNull(Func<TComponent, object> valueGetter, string methodName)
        {
            if (valueGetter == null) throw new ArgumentNullException(nameof(valueGetter));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            if (valueGetter(Component) == null)
                throw new InvalidOperationException($"Value is null. You can use {methodName} to set it.");
        }
    }
}
