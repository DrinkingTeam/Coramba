using System;
using Coramba.Core.Converters;
using Coramba.Services.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.Services.Registration
{
    public abstract class BaseModelDtoModule<TModelDto, TId, TModule> : BaseModelDtoModule<TModelDto, TModule>
        where TModule : BaseModelDtoModule<TModelDto, TId, TModule>
    {
        public TModule IdGetter(Func<TModelDto, TId> idGetter)
        {
            if (idGetter == null) throw new ArgumentNullException(nameof(idGetter));
            return WithComponent(c => c.IdGetter = idGetter);
        }

        protected override void Register()
        {
            base.Register();

            if (Component.IdGetter != null)
                Services.AddScoped(typeof(IObjectConverter<TModelDto, TId>), sp =>
                    ActivatorUtilities.CreateInstance<DtoToIdConverter<TModelDto, TId>>(sp, Component.IdGetter));
        }
    }
}
