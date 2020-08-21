using Coramba.DataAccess.Repositories;

namespace Coramba.Services.Common
{
    public abstract class BaseModelService<TModel>
    {
        protected ICrudRepository<TModel> Repository { get; }

        protected BaseModelService(BaseModelServiceContext<TModel> context)
        {
            Repository = context.Repository;
        }
    }
}
