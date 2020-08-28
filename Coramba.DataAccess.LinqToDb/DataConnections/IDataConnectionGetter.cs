using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    public interface IDataConnectionGetter<TDataConnection>
        where TDataConnection : DataConnection
    {
        TDataConnection Get();
    }
}
