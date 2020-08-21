using System;

namespace Coramba.DependencyInjection.Modules
{
    public abstract class ModuleBuilder<TModule, TComponent> : BaseModule<TComponent>
        where TModule: ModuleBuilder<TModule, TComponent>
        where TComponent: new()
    {
        protected new TModule WithComponent(Action<TComponent> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            action(Component);

            return (TModule)this;
        }
    }
}
