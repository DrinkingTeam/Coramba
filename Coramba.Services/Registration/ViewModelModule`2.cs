using Coramba.DependencyInjection;

namespace Coramba.Services.Registration
{
    public class ModelDtoModule<TModelDto, TId> : BaseModelDtoModule<TModelDto, TId, ModelDtoModule<TModelDto, TId>>
    {
        public ModelDtoModule<TModelDto, TModel, TId> Model<TModel>()
            => Services.Module<ModelDtoModule<TModelDto, TModel, TId>>().SetupComponent(Component);
    }
}
