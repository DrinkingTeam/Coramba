using System;
using System.Linq;
using System.Threading.Tasks;
using Coramba.DataAccess.LinqToDb.DataConnections;
using Coramba.DataAccess.Repositories;
using LinqToDB;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.Repositories
{
    public class LinqToDbCrudRepository<TDataConnection, T> : CrudRepository<T>
        where TDataConnection : DataConnection
        where T : class
    {
        protected IDataConnectionGetter<TDataConnection> DataConnectionGetter { get; }

        protected TDataConnection DataConnection
        {
            get
            {
                var dbContext = DataConnectionGetter.Get();
                if (dbContext == null)
                    throw new Exception($"DataConnection {typeof(TDataConnection)} not set");
                return dbContext;
            }
        }

        protected ITable<T> Table => DataConnection.GetTable<T>();

        public LinqToDbCrudRepository(LinqToDbCrudRepositoryContext<TDataConnection, T> context)
            : base(context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            DataConnectionGetter = context.DataConnectionGetter;
        }

        protected override RepositoryOperationContext CreateOperationContext()
        {
            return new LinqToDbRepositoryOperationContext(DataConnection);
        }

        protected override Task<IQueryable<T>> ListCoreAsync(RepositoryOperationContext context)
            => Task.FromResult((IQueryable<T>)Table);

        protected override async Task InsertCoreAsync(T entity, RepositoryOperationContext context)
        {
            await DataConnection.InsertAsync(entity);
        }

        protected override async Task UpdateCoreAsync(T entity, RepositoryOperationContext context)
        {
            await DataConnection.UpdateAsync(entity);
        }

        protected override async Task DeleteCoreAsync(T entity, RepositoryOperationContext context)
        {
            await DataConnection.DeleteAsync(entity);
        }
    }
}
