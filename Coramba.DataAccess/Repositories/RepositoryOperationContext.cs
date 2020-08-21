using System;
using System.Collections.Generic;

namespace Coramba.DataAccess.Repositories
{
    public class RepositoryOperationContext
    {
        private Dictionary<string, object> _parameters;
        private HashSet<Type> _disableConventions;

        public DateTime? UtcNow { get; set; }

        public object GetParameter(string name)
            => _parameters?.GetValueOrDefault(name);

        public T GetParameter<T>(string name, T defaultValue = default)
            => (T)(GetParameter(name) ?? defaultValue);

        public void SetParameter(string name, object value)
        {
            _parameters ??= new Dictionary<string, object>();
            _parameters[name] = value;
        }

        public bool IsConventionEnabled(Type type)
            => _disableConventions == null || !_disableConventions.Contains(type);
        public void EnableConvention(Type type, bool enabled)
        {
            _disableConventions ??= new HashSet<Type>();

            if (!enabled)
                _disableConventions.Add(type);
            else
                _disableConventions.Remove(type);
        }

        public void EnableConvention<T>(bool enabled)
            => EnableConvention(typeof(T), enabled);
    }
}