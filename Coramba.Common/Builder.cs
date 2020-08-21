using System;

namespace Coramba.Common
{
    public class Builder<TBuilder, TComponent>
        where TBuilder: Builder<TBuilder, TComponent>
    {
        private TComponent _component;

        protected TComponent Component
        {
            get => _component ??= CreateComponent();
            set => _component = value;
        }

        protected virtual TComponent CreateComponent()
            => (TComponent) Activator.CreateInstance(typeof(TComponent));

        protected TBuilder WithComponent(Action<TComponent> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            action(Component);
            return (TBuilder) this;
        }
    }
}
