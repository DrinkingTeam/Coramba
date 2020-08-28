using System;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    public class DataConnectionGetter<TDataConnection> : IDataConnectionGetter<TDataConnection>
        where TDataConnection : DataConnection
    {
        private readonly IDataConnectionStore<TDataConnection> _store;

        public DataConnectionGetter(IDataConnectionStore<TDataConnection> store)
        {
            _store = store;
        }

        public TDataConnection Get()
        {
            var name = _store.GetCurrent();
            if (name == null)
                throw new Exception($"Current DataConnection is not set for {typeof(TDataConnection).Name}");
            return _store.Get(name);
        }
    }
}