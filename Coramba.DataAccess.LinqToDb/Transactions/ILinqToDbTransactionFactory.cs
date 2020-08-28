using System.Threading.Tasks;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.Transactions
{
    public interface ILinqToDbTransactionFactory<TDataConnection>
        where TDataConnection: DataConnection
    {
        Task<LinqToDbTransaction> CreateAsync();
        LinqToDbTransaction Current { get; }
    }
}
