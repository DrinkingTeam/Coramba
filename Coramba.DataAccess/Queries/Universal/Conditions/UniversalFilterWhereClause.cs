using System.Collections.Generic;

namespace Coramba.DataAccess.Queries.Universal.Conditions
{
    public class UniversalFilterWhereClause
    {
        public string Text { get; set; }
        public List<object> Parameters { get; set; }
    }
}
