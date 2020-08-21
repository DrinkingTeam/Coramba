using Coramba.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DataAccess.Forge
{
    public static class RegistrationExtensions
    {
        public static RegistrationModule ForgeModule(this IServiceCollection services)
            => services.Module<RegistrationModule>();

        public static void AddForgeModule(this IServiceCollection services)
            => services.AddModule<RegistrationModule>();
    }
}
