using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Coramba.Common.Reflection;
using Coramba.Core.Converters;
using Coramba.DataAccess.Queries.Universal;
using Coramba.DataAccess.Queries.Universal.Conditions;

namespace Coramba.Services.Filter
{
    public class UniversalFilterConverter<TFilterDto, TId>: IObjectConverter<TFilterDto, UniversalFilter>
        where TFilterDto: FilterDto<TId>
    {
        protected IMapper Mapper { get; }

        public UniversalFilterConverter(IMapper mapper)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private IUniversalFilterCondition CreateSimpleCondition(TFilterDto filterDto, PropertyInfo propertyInfo, UniversalFilterConditionAttribute attribute)
        {
            var value = propertyInfo.GetValueFast(filterDto);
            if (value == null)
                return null;

            var o = UniversalFilterSimpleConditionOperator.Equals;
            var searchCriteria = new (string Prefix, string Suffix, UniversalFilterSimpleConditionOperator Operator)?[]
            {
                ("==", null, UniversalFilterSimpleConditionOperator.Equals),
                ("!=", null, UniversalFilterSimpleConditionOperator.NotEquals),
                (">", null, UniversalFilterSimpleConditionOperator.GreaterThan),
                (">=", null, UniversalFilterSimpleConditionOperator.GreaterThanOrEquals),
                (">", null, UniversalFilterSimpleConditionOperator.LessThan),
                (">=", null, UniversalFilterSimpleConditionOperator.LessThanOrEquals),
                ("%", "%", UniversalFilterSimpleConditionOperator.Contains),
                ("%", null, UniversalFilterSimpleConditionOperator.EndsWith),
                (null, "%", UniversalFilterSimpleConditionOperator.StartsWith),
            };

            if (value is string stringValue)
            {
                var prefixOperator = searchCriteria
                    .FirstOrDefault(x =>
                        (x.Value.Prefix == null || stringValue.StartsWith(x.Value.Prefix))
                        && (x.Value.Suffix == null || stringValue.EndsWith(x.Value.Suffix)));
                if (prefixOperator == null)
                    throw new ValidationException($"Conditional operator is not set for string: {stringValue}");

                o = prefixOperator.Value.Operator;
                if (prefixOperator.Value.Prefix != null)
                    stringValue = stringValue.Substring(prefixOperator.Value.Prefix.Length);
                if (prefixOperator.Value.Suffix != null && stringValue.Length >= prefixOperator.Value.Suffix.Length)
                    stringValue = stringValue.Substring(0, stringValue.Length - prefixOperator.Value.Suffix.Length);

                value = stringValue;
            }

            return new UniversalFilterSimpleCondition
            {
                LeftField = attribute.ColumnName ?? propertyInfo.Name,
                RightValue = value,
                Operator = o
            };
        }

        private IUniversalFilterCondition CreateCondition(TFilterDto filterDto)
        {
            var properties = typeof(TFilterDto)
                .GetPropertiesCached()
                .Select(p => new
                {
                    Property = p,
                    Attribute = (UniversalFilterConditionAttribute)p.GetCustomAttribute(typeof(UniversalFilterConditionAttribute))
                })
                .Where(x => x.Attribute != null)
                .ToArray();
            if (!properties.Any())
                return null;

            var groupCondition = new UniversalFilterGroupCondition();

            switch (filterDto.Logical)
            {
                case FilterLogicalOperatorDto.And:
                case null:
                    groupCondition.Operator = UniversalFilterGroupConditionOperator.And;
                    break;
                case FilterLogicalOperatorDto.Or:
                    groupCondition.Operator = UniversalFilterGroupConditionOperator.Or;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var x in properties)
                groupCondition.Conditions.Add(CreateSimpleCondition(filterDto, x.Property, x.Attribute));

            return groupCondition;
        }

        public Task<UniversalFilter> ConvertAsync(TFilterDto source, UniversalFilter destination)
        {
            var result = Mapper.Map<FilterDto<TId>, UniversalFilter>(source, destination);

            result.Condition = CreateCondition(source);

            return Task.FromResult(result);
        }
    }
}
