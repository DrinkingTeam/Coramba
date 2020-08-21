using System;
using System.Threading.Tasks;
using Coramba.DataAccess.Ef.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coramba.DataAccess.Ef.Transactions
{
    class EfTransactionFactory<TDbContext> : IEfTransactionFactory<TDbContext>
        where TDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly IDbContextGetter<TDbContext> _dbContextGetter;

        public EfTransactionFactory(IDbContextGetter<TDbContext> dbContextGetter)
        {
            _dbContextGetter = dbContextGetter ?? throw new ArgumentNullException(nameof(dbContextGetter));
        }

        public Task<IDbContextTransaction> CreateAsync()
            => _dbContextGetter.Get().Database.BeginTransactionAsync();

        public IDbContextTransaction Current
            => _dbContextGetter.Get().Database.CurrentTransaction;
    }
}