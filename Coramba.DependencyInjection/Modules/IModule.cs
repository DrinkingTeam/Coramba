using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DependencyInjection.Modules
{
    public interface IModule
    {
        void Init(IServiceCollection services);
        IServiceCollection Services { get; }
        void AddRegistration(bool includeOncePart);
    }
}
