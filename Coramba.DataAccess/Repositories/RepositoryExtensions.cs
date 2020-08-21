using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coramba.Common;

namespace Coramba.DataAccess.Repositories
{
    public static class RepositoryExtensions
    {
        public static Task<IQueryable<T>> QueryAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> whereClause)
        {
            return repository.QueryAsync().Where(whereClause);
        }
    }
}
