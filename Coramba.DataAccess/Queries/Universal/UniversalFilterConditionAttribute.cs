using System;

namespace Coramba.DataAccess.Queries.Universal
{
    /// <summary>
    /// UniversalFilterConditionAttribute
    /// </summary>
    /// <remarks>
    ///  If property is string, then it's value must be preceded by conditional operator.
    ///  These operators are:
    ///    == - equals,
    ///    != - not equals,
    ///    &gt; - greater than,
    ///    &lt; - less than,
    ///    &gt;= - greater than or equals,
    ///    &lt;= -  less than or equals,
    ///    % - ends with,
    ///    if '%' (percentage symbol) is at the end of the string, then the conditional operator is 'starts with'
    ///    if '%' (percentage symbol) is at the end and also begins with it, then the conditional operator is 'contains'
    /// </remarks>
    public class UniversalFilterConditionAttribute: Attribute
    {
        public string Column { get; set; }
        public bool AllowOrderBy { get; set; }
    }
}
