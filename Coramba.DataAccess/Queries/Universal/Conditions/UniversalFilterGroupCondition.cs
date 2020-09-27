using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coramba.DataAccess.Queries.Universal.Conditions
{
    public class UniversalFilterGroupCondition : IUniversalFilterCondition
    {
        public List<IUniversalFilterCondition> Conditions { get; } = new List<IUniversalFilterCondition>();
        public UniversalFilterGroupConditionOperator Operator { get; set; }

        public UniversalFilterWhereClause GetWhereClause(int parameterIndex)
        {
            if (Conditions == null)
                return null;

            var whereBuilder = new StringBuilder();
            var parameters = new List<object>();

            foreach (var condition in Conditions.Where(c => c != null))
            {
                var conditionResult = condition.GetWhereClause(parameters.Count);
                if (conditionResult?.Text == null)
                    continue;

                if (whereBuilder.Length > 0)
                {
                    switch (Operator)
                    {
                        case UniversalFilterGroupConditionOperator.And:
                            whereBuilder.Append(" && ");
                            break;
                        case UniversalFilterGroupConditionOperator.Or:
                            whereBuilder.Append(" || ");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                whereBuilder.Append("(");
                whereBuilder.Append(conditionResult.Text);
                whereBuilder.Append(")");

                parameters.AddRange(conditionResult.Parameters);
            }

            if (whereBuilder.Length == 0)
                return null;

            return new UniversalFilterWhereClause
            {
                Text = whereBuilder.ToString(),
                Parameters = parameters
            };
        }
    }
}