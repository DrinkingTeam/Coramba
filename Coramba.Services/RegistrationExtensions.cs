using System.Collections.Generic;
using System.Linq;
using Coramba.Core.Converters;
using Coramba.DependencyInjection;
using Coramba.Services.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.Services
{
    public static class RegistrationExtensions
    {
        public static RegistrationModule ServicesModule(this IServiceCollection services)
            => services.Module<RegistrationModule>();

        public static void AddServicesModule(this IServiceCollection services)
            => services.AddModule<RegistrationModule>();

        public static ModelDtoModule<TModelDto> ModelDto<TModelDto>(this IServiceCollection services)
            => services.Module<ModelDtoModule<TModelDto>>();

        public static void AddQueryConverter<T, TImpl>(this IServiceCollection services)
            where TImpl: class, IObjectConverter<IQueryable<T>, IEnumerable<T>>
            => services.AddScoped<IObjectConverter<IQueryable<T>, IEnumerable<T>>, TImpl>();
    }
}
