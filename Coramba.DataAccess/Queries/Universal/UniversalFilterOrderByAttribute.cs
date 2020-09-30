using System;

namespace Coramba.DataAccess.Queries.Universal
{
    public class UniversalFilterOrderByAttribute: Attribute
    {
        public string DtoName { get; }
        public string Column { get; }

        public UniversalFilterOrderByAttribute(string dtoName, string column)
        {
            DtoName = dtoName ?? throw new ArgumentNullException(nameof(dtoName));
            Column = column ?? throw new ArgumentNullException(nameof(column));
        }
    }
}
