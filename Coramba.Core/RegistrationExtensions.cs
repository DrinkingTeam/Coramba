using Coramba.Core.Converters;
using Coramba.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.Core
{
    public static class RegistrationExtensions
    {
        public static RegistrationModule CoreModule(this IServiceCollection services)
            => services.Module<RegistrationModule>();

        public static void AddCoreModule(this IServiceCollection services)
            => services.AddModule<RegistrationModule>();

        public static void AddConverter<TFrom, TTo, TConverter>(this IServiceCollection services)
            where TConverter: class, IObjectConverter<TFrom, TTo>
            => services.AddScoped<IObjectConverter<TFrom, TTo>, TConverter>();
    }
}
