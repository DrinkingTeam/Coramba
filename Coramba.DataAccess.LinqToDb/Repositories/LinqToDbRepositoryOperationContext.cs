using Coramba.DataAccess.Repositories;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.Repositories
{
    public class LinqToDbRepositoryOperationContext: RepositoryOperationContext
    {
        public DataConnection DataConnection { get; }

        public LinqToDbRepositoryOperationContext(DataConnection dataConnection)
        {
            DataConnection = dataConnection;
        }
    }
}
