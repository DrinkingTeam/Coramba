using System;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DependencyInjection.Modules
{
    public abstract class BaseModule : IModule
    {
        protected IServiceCollection Services { get; private set; }

        #region IModule
        void IModule.Init(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        IServiceCollection IModule.Services => Services;

        void IModule.AddRegistration(bool includeOncePart)
        {
            RegisterCore(includeOncePart);
        }
        #endregion

        private void RegisterCore(bool includeOncePart)
        {
            if (Services == null)
                throw new Exception($"{nameof(IModule)}.{nameof(IModule.Init)}() method is not invoked");

            Register();

            var hasMarker = !includeOncePart || Services.HasMarker(GetType());
            if (!hasMarker)
            {
                Services.SetMarker(GetType());

                RegisterOnce();
            }
        }

        protected virtual void Register()
        {
        }

        protected virtual void RegisterOnce()
        {
        }

        public void Add(bool includeOncePart = false)
        {
            RegistrationExtensions.AddModule(this, includeOncePart);
        }
    }
}
