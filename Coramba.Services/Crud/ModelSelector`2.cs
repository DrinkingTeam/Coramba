using System.Linq;
using System.Threading.Tasks;
using Coramba.DataAccess.Base;

namespace Coramba.Services.Crud
{
    public class ModelSelector<TModel, TModelDto>: ModelSelector<TModel, TModelDto, object>
    {
        protected IModelFinder<TModel> ModelFinder { get; }

        public ModelSelector(ModelSelectorContext<TModel, TModelDto> context)
            : base(context)
        {
            ModelFinder = context.ModelFinder;
        }

        protected override Task<IQueryable<TModel>> GetModelById(IQueryable<TModel> queryable, object id)
            => ModelFinder.FindAsync(queryable, id);
    }
}
