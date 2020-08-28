using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    public interface IDataConnectionFactory<TDataConnection>
        where TDataConnection : DataConnection
    {
        string Name { get; }
        TDataConnection Create();
    }
}
