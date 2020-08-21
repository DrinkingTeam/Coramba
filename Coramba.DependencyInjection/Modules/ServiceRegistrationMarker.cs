using System;

namespace Coramba.DependencyInjection.Modules
{
    internal class ServiceRegistrationMarker<TService>
    {
        public ServiceRegistrationMarker()
            => throw new InvalidOperationException($"This type is not intended for construction and only needed to service registration procedure");
    }
}
