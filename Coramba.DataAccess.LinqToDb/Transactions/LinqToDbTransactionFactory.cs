using System;
using System.Threading.Tasks;
using Coramba.DataAccess.LinqToDb.DataConnections;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.Transactions
{
    class LinqToDbTransactionFactory<TDataConnection> : ILinqToDbTransactionFactory<TDataConnection>
        where TDataConnection : DataConnection
    {
        private readonly IDataConnectionGetter<TDataConnection> _dataConnectionGetter;

        public LinqToDbTransactionFactory(IDataConnectionGetter<TDataConnection> dataConnectionGetter)
        {
            _dataConnectionGetter = dataConnectionGetter ?? throw new ArgumentNullException(nameof(dataConnectionGetter));
        }

        public async Task<LinqToDbTransaction> CreateAsync()
        {
            var transaction = await _dataConnectionGetter.Get().BeginTransactionAsync();
            return Current = new LinqToDbTransaction(transaction, () => Current = null);
        }

        public LinqToDbTransaction Current { get; private set; }
    }
}