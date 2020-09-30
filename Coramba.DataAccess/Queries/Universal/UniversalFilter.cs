using Coramba.DataAccess.Queries.Universal.Conditions;

namespace Coramba.DataAccess.Queries.Universal
{
    public class UniversalFilter
    {
        public object[] Ids { get; set; }
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public IUniversalFilterCondition Condition { get; set; }
        public UniversalFilterOrderByItem[] OrderBy { get; set; }
    }
}
