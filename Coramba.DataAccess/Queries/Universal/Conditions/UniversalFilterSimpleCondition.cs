using System;
using System.Collections.Generic;
using System.Text;
using Coramba.Common;

namespace Coramba.DataAccess.Queries.Universal.Conditions
{
    public class UniversalFilterSimpleCondition : IUniversalFilterCondition
    {
        public string LeftField { get; set; }
        public object LeftValue { get; set; }
        public string RightField { get; set; }
        public object RightValue { get; set; }
        public UniversalFilterSimpleConditionOperator Operator { get; set; }

        public UniversalFilterWhereClause GetWhereClause(int parameterIndex)
        {
            var whereBuilder = new StringBuilder();
            var parameters = new List<object>();

            if (LeftField != null)
                whereBuilder.Append(LeftField);
            else
            {
                whereBuilder.Append($"@{parameterIndex}");
                parameters.Add(LeftValue);
                parameterIndex++;
            }

            switch (Operator)
            {
                case UniversalFilterSimpleConditionOperator.Equals:
                    whereBuilder.Append(" == ");
                    break;
                case UniversalFilterSimpleConditionOperator.NotEquals:
                    whereBuilder.Append(" != ");
                    break;
                case UniversalFilterSimpleConditionOperator.GreaterThan:
                    whereBuilder.Append(" > ");
                    break;
                case UniversalFilterSimpleConditionOperator.LessThan:
                    whereBuilder.Append(" < ");
                    break;
                case UniversalFilterSimpleConditionOperator.GreaterThanOrEquals:
                    whereBuilder.Append(" >= ");
                    break;
                case UniversalFilterSimpleConditionOperator.LessThanOrEquals:
                    whereBuilder.Append(" <= ");
                    break;
                case UniversalFilterSimpleConditionOperator.StartsWith:
                    whereBuilder.Append(".ToLower().StartsWith(");
                    break;
                case UniversalFilterSimpleConditionOperator.EndsWith:
                    whereBuilder.Append(".ToLower().EndsWith(");
                    break;
                case UniversalFilterSimpleConditionOperator.Contains:
                    whereBuilder.Append(".ToLower().Contains(");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (RightField != null)
                whereBuilder.Append(RightField);
            else
            {
                whereBuilder.Append($"@{parameterIndex}");
                parameters.Add(RightValue);
            }

            if (Operator.In(
                UniversalFilterSimpleConditionOperator.StartsWith,
                UniversalFilterSimpleConditionOperator.EndsWith,
                UniversalFilterSimpleConditionOperator.Contains))
                whereBuilder.Append(")");

            return new UniversalFilterWhereClause
            {
                Text = whereBuilder.ToString(),
                Parameters = parameters
            };
        }
    }
}