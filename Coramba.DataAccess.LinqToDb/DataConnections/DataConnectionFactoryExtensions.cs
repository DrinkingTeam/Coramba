using System;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    public static class DataConnectionFactoryExtensions
    {
        public static TDataConnection Create<TDataConnection>(this IDataConnectionFactory<TDataConnection> factory, Action<TDataConnection> configurator)
            where TDataConnection : DataConnection
        {
            var result = factory.Create();
            configurator?.Invoke(result);
            return result;
        }
    }
}
