namespace Coramba.Services.Filter
{
    /// <summary>
    /// Basic filter.
    /// </summary>
    /// <typeparam name="TId">Id type.</typeparam>
    public class FilterDto<TId>
    {
        public TId[] Ids { get; set; }
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public FilterLogicalOperatorDto? Logical { get; set; }
    }
}