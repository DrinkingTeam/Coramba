using Coramba.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.Api
{
    public static class RegistrationExtensions
    {
        public static RegistrationModule ApiModule(this IServiceCollection services)
            => services.Module<RegistrationModule>();

        public static void AddApiModule(this IServiceCollection services)
            => services.AddModule<RegistrationModule>();
    }
}
