using System;
using Coramba.DataAccess.Conventions;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Repositories
{
    public abstract class RepositoryContext<T>
    {
        public IRepositoryConventions<T> Conventions { get; }
        public ILogger<T> Logger { get; }

        protected RepositoryContext(IRepositoryConventions<T> conventions, ILogger<T> logger)
        {
            Conventions = conventions ?? throw new ArgumentNullException(nameof(conventions));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
