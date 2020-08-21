using System;
using System.Threading.Tasks;
using Coramba.DataAccess.Conventions;

namespace Coramba.DataAccess.Repositories
{
    public abstract class CrudRepository<T> : Repository<T>, ICrudRepository<T>
    {
        protected CrudRepository(CrudRepositoryContext<T> context)
            : base(context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
        }

        protected virtual Task<T> NewCoreAsync(RepositoryOperationContext context)
        {
            var model = (T)Activator.CreateInstance(typeof(T));

            return Task.FromResult(model);
        }

        private async Task ApplyModelConventions<TConvention>(T model, RepositoryOperationContext context)
            where TConvention: IRepositoryModelConvention<T>
        {
            await GetConventions<TConvention>().ApplyAsync(model, context);
        }

        protected virtual async Task InitModelAsync(T model, RepositoryOperationContext context)
        {
            await ApplyModelConventions<IRepositoryNewConvention<T>>(model, context);
        }

        public async Task<T> NewAsync()
        {
            var context = CreateOperationContext();
            var model = await NewCoreAsync(context);

            await InitModelAsync(model, context);

            return model;
        }

        protected virtual async Task BeforeInsertAsync(T model, RepositoryOperationContext context)
        {
            await ApplyModelConventions<IRepositoryBeforeInsertConvention<T>>(model, context);
        }

        protected virtual async Task AfterInsertAsync(T model, RepositoryOperationContext context)
        {
            await ApplyModelConventions<IRepositoryAfterInsertConvention<T>>(model, context);
        }

        protected abstract Task InsertCoreAsync(T model, RepositoryOperationContext context);

        public async Task InsertAsync(T model)
        {
            var context = CreateOperationContext();

            await BeforeInsertAsync(model, context);

            await InsertCoreAsync(model, context);

            await AfterInsertAsync(model, context);
        }

        protected virtual async Task BeforeUpdateAsync(T model, RepositoryOperationContext context)
        {
            await ApplyModelConventions<IRepositoryBeforeUpdateConvention<T>>(model, context);
        }

        protected virtual async Task AfterUpdateAsync(T model, RepositoryOperationContext context)
        {
            await ApplyModelConventions<IRepositoryAfterUpdateConvention<T>>(model, context);
        }

        protected abstract Task UpdateCoreAsync(T model, RepositoryOperationContext context);

        public async Task UpdateAsync(T model)
        {
            var context = CreateOperationContext();

            await BeforeUpdateAsync(model, context);

            await UpdateCoreAsync(model, context);

            await AfterUpdateAsync(model, context);
        }

        protected virtual async Task BeforeDeleteAsync(T model, RepositoryOperationContext context)
        {
            await ApplyModelConventions<IRepositoryBeforeDeleteConvention<T>>(model, context);
        }

        protected virtual async Task AfterDeleteAsync(T model, RepositoryOperationContext context)
        {
            await ApplyModelConventions<IRepositoryAfterDeleteConvention<T>>(model, context);
        }

        protected abstract Task DeleteCoreAsync(T model, RepositoryOperationContext context);

        public async Task DeleteAsync(T model)
        {
            var context = CreateOperationContext();

            await BeforeDeleteAsync(model, context);

            await DeleteCoreAsync(model, context);

            await AfterDeleteAsync(model, context);
        }
    }
}
