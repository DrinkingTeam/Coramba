using System;
using System.Threading.Tasks;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.Transactions
{
    public class LinqToDbTransaction: IDisposable, IAsyncDisposable
    {
        private DataConnectionTransaction _connectionTransaction;
        private readonly Action _onDispose;

        public LinqToDbTransaction(DataConnectionTransaction connectionTransaction, Action onDispose)
        {
            _connectionTransaction = connectionTransaction;
            _onDispose = onDispose;
        }

        private bool _disposed;
        
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            // Cascade async dispose calls
            if (_connectionTransaction != null)
            {
                await _connectionTransaction.DisposeAsync();
                _connectionTransaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _connectionTransaction?.Dispose();
                // TODO: dispose managed state (managed objects).
            }

            _onDispose();

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            _disposed = true;
        }

        public void Commit() => _connectionTransaction.Commit();
        public void Rollback() => _connectionTransaction.Rollback();

        public Task CommitAsync() => _connectionTransaction.CommitAsync();
        public Task RollbackAsync() => _connectionTransaction.RollbackAsync();
    }
}
