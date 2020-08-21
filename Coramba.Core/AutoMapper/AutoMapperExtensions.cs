using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using Coramba.Common;
using Coramba.Common.Reflection;

namespace Coramba.Core.AutoMapper
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression, params Expression<Func<TDestination, object>>[] members)
        {
            members.ForEach(m =>
            {
                expression = expression.ForMember(MemberHelper.GetName(m), opt => opt.Ignore());
            });
            return expression;
        }

        public static IMapperConfigurationExpression AddByAnnotations(
            this IMapperConfigurationExpression expression,
            params Assembly[] assembliesToScan)
        {
            var allTypes = assembliesToScan.Where(a => !a.IsDynamic).SelectMany(a => a.GetTypes()).ToArray();

            foreach (var type in allTypes)
                foreach (var attribute in type.GetCustomAttributes().OfType<IAutoMapTypeConfigurator>())
                    attribute.Configure(expression, type);

            return expression;
        }
    }
}
