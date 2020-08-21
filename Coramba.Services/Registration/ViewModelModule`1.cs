using System;
using Coramba.DependencyInjection;

namespace Coramba.Services.Registration
{
    public class ModelDtoModule<TModelDto> : BaseModelDtoModule<TModelDto, ModelDtoModule<TModelDto>>
    {
        public ModelDtoModule<TModelDto, TId> Id<TId>(Func<TModelDto, TId> idGetter)
            => Services.Module<ModelDtoModule<TModelDto, TId>>().SetupComponent(Component).IdGetter(idGetter);

        public ModelDtoModule<TModelDto, TId> Id<TId>(Func<TModelDto, TId?> idGetter)
            where TId: struct
            => Services.Module<ModelDtoModule<TModelDto, TId>>().SetupComponent(Component).IdGetter(m => idGetter(m).GetValueOrDefault());
    }
}
