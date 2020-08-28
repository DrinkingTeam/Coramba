using System;
using Coramba.DataAccess.Conventions;
using Coramba.DataAccess.LinqToDb.DataConnections;
using Coramba.DataAccess.Repositories;
using LinqToDB.Data;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.LinqToDb.Repositories
{
    public class LinqToDbCrudRepositoryContext<TDataConnection, T> : CrudRepositoryContext<T>
        where TDataConnection : DataConnection
    {
        public IDataConnectionGetter<TDataConnection> DataConnectionGetter { get; }

        public LinqToDbCrudRepositoryContext(IRepositoryConventions<T> conventions, IDataConnectionGetter<TDataConnection> dbContextGetter, ILogger<T> logger)
            : base(conventions, logger)
        {
            DataConnectionGetter = dbContextGetter ?? throw new ArgumentNullException(nameof(dbContextGetter));
        }
    }
}
