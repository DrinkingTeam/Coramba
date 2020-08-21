using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coramba.DataAccess.Conventions;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Repositories
{
    public abstract class Repository<T> : IRepository<T>
    {
        private IRepositoryConventions<T> Conventions { get; }
        protected ILogger<T> Logger { get; }

        protected Repository(RepositoryContext<T> context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            Conventions = context.Conventions;
            Logger = context.Logger;
        }

        protected virtual RepositoryOperationContext CreateOperationContext()
            => new RepositoryOperationContext();

        protected IEnumerable<TConvention> GetConventions<TConvention>()
            where TConvention: IRepositoryConvention<T>
            => Conventions.GetConventions().OfType<TConvention>();

        protected abstract Task<IQueryable<T>> ListCoreAsync(RepositoryOperationContext context);

        protected virtual Task<IQueryable<T>> FilterAsync(IQueryable<T> query, RepositoryOperationContext context)
        {
            return GetConventions<IRepositoryFilterConvention<T>>()
                .ApplyAsync(query, (c, q) => c.ApplyAsync(q, context), context);
        }

        public async Task<IQueryable<T>> QueryAsync()
        {
            var context = CreateOperationContext();
            return await FilterAsync(await ListCoreAsync(context), context);
        }
    }
}
