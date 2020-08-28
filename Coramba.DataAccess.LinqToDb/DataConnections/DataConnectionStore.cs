using System;
using System.Collections.Generic;
using System.Linq;
using Coramba.Common;
using LinqToDB.Data;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    class DataConnectionStore<TDataConnection> : IDataConnectionStore<TDataConnection>
        where TDataConnection: DataConnection
    {
        private readonly ILogger<DataConnectionStore<TDataConnection>> _logger;
        private readonly IDataConnectionFactory<TDataConnection>[] _dataConnectionFactories;
        private string _currentName;

        private Dictionary<string, TDataConnection> _dataConnections;

        private Dictionary<string, TDataConnection> DataConnections => _dataConnections ??= CreateDataConnections();

        public DataConnectionStore(IEnumerable<IDataConnectionFactory<TDataConnection>> dataConnectionFactories,
            ILogger<DataConnectionStore<TDataConnection>> logger)
        {
            _logger = logger;
            _dataConnectionFactories = dataConnectionFactories?.ToArray() ?? new IDataConnectionFactory<TDataConnection>[0];
            _currentName = _dataConnectionFactories.FirstOrDefault()?.Name;
        }

        private Dictionary<string, TDataConnection> CreateDataConnections()
        {
            var result = new Dictionary<string, TDataConnection>();
            _dataConnectionFactories
                .ForEach(f =>
                {
                    result.Add(f.Name, f.Create());
                });
            return result;
        }

        public TDataConnection Get(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (!DataConnections.TryGetValue(name, out var dbContext))
                throw new Exception($"DataConnection with name {name} is not registered");
            return dbContext;
        }

        public string GetCurrent() => _currentName;

        public void SetCurrent(string name)
        {
            _logger.LogInformation($"Set DataConnection for {typeof(TDataConnection).Name}: {name}");

            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!DataConnections.ContainsKey(name))
                throw new Exception($"DataConnection with name {name} is not registered");

            _currentName = name;
        }

        public void Add(string name, TDataConnection dbContext)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            DataConnections.Add(name, dbContext);
        }

        public void Remove(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!DataConnections.ContainsKey(name))
                throw new Exception($"DataConnection with name {name} is not registered");

            DataConnections.Remove(name);
            if (_currentName == name)
                _currentName = null;
        }
    }
}