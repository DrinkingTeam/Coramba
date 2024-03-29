﻿using System;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    public interface IDataConnectionStore<TDataConnection>: IAsyncDisposable, IDisposable
        where TDataConnection : DataConnection
    {
        TDataConnection Get(string name);
        string GetCurrent();
        void SetCurrent(string name);
        void Add(string name, TDataConnection dbContext);
        void Remove(string name);
    }
}
