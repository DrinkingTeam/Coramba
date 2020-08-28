using System;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    public class DataConnectionFactory<TDataConnection> : IDataConnectionFactory<TDataConnection>
        where TDataConnection : DataConnection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Action<TDataConnection> _configurator;
        public string Name { get; }

        public DataConnectionFactory(string name, Action<TDataConnection> configurator, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _configurator = configurator;
        }

        public TDataConnection Create()
        {
            var dbContext = ActivatorUtilities.CreateInstance<TDataConnection>(_serviceProvider);

            _configurator?.Invoke(dbContext);

            return dbContext;
        }
    }
}