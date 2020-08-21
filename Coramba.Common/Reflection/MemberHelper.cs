using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Coramba.Common.Reflection
{
    public static class MemberHelper
    {
        private static MemberExpression GetMemberExpression<T, TMemberType>(Expression<Func<T, TMemberType>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var expressionBody = expression.Body;
            while (expressionBody.NodeType == ExpressionType.Convert)
                expressionBody = ((UnaryExpression)expressionBody).Operand;

            if (!(expressionBody is MemberExpression memberExpression))
                throw new Exception($"expression body must be of type MemberExpression");

            return memberExpression;
        }

        public static MemberInfo GetMemberInfo<T>(Expression<Func<T, object>> expression)
        {
            return GetMemberExpression(expression).Member;
        }

        public static string GetName<T, TMemberType>(Expression<Func<T, TMemberType>> expression)
        {
            return GetMemberExpression(expression).Member.Name;
        }

        public static Type GetType<T>(Expression<Func<T, object>> expression)
        {
            var memberInfo = GetMemberInfo(expression);
            return memberInfo switch
            {
                PropertyInfo x => x.PropertyType,
                FieldInfo x => x.FieldType,
                _ => throw new Exception($"Unexpected member type {memberInfo?.GetType()}")
            };
        }
    }
}
