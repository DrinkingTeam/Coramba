using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coramba.DataAccess.Ef.Transactions
{
    public interface IEfTransactionFactory<TDbContext>
        where TDbContext: DbContext
    {
        IDbContextTransaction Current { get; }
        Task<IDbContextTransaction> CreateAsync();
    }
}
