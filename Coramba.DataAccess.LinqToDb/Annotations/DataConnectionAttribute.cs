using System;

namespace Coramba.DataAccess.LinqToDb.Annotations
{
    public class DataConnectionAttribute: Attribute
    {
        public Type DataConnectionType { get; }

        public DataConnectionAttribute(Type dataConnectionType)
        {
            DataConnectionType = dataConnectionType ?? throw new ArgumentNullException(nameof(dataConnectionType));
        }
    }
}
