using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Coramba.Common.Reflection
{
    public static class MemberInfoExtensions
    {
        public static Type GetReturnType(this MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case PropertyInfo x:
                    return x.PropertyType;
                case FieldInfo x:
                    return x.FieldType;
                case MethodInfo x:
                    return x.ReturnType;
            }

            throw new NotSupportedException($"Invalid member type of {memberInfo}");
        }

        public static Expression<Func<T, object>> GetGetterExpression<T>(this MemberInfo memberInfo)
        {
            var targetType = memberInfo.DeclaringType;
            var exInstance = Expression.Parameter(typeof(T), "t");
            Expression convertedExInstance = exInstance;
            if (!targetType.IsAssignableFrom(typeof(T)))
                convertedExInstance = Expression.Convert(exInstance, targetType);

            var exMemberAccess = Expression.MakeMemberAccess(convertedExInstance, memberInfo);       // t.PropertyName
            var exConvertToObject = Expression.Convert(exMemberAccess, typeof(object));     // Convert(t.PropertyName, typeof(object))
            var lambda = Expression.Lambda<Func<T, object>>(exConvertToObject, exInstance);

            return lambda;
        }

        public static Expression<Action<T, object>> GetSetterExpression<T>(this MemberInfo memberInfo)
        {
            var targetType = memberInfo.DeclaringType;
            var exInstance = Expression.Parameter(typeof(T), "t");

            var exMemberAccess = Expression.MakeMemberAccess(Expression.Convert(exInstance, targetType), memberInfo);

            // t.PropertValue(Convert(p))
            var exValue = Expression.Parameter(typeof(object), "p");
            var exConvertedValue = Expression.Convert(exValue, GetUnderlyingType(memberInfo));
            var exBody = Expression.Assign(exMemberAccess, exConvertedValue);

            var lambda = Expression.Lambda<Action<T, object>>(exBody, exInstance, exValue);
            return lambda;
        }

        private static readonly ConcurrentCache<MemberInfo, Func<object, object>> GettersCache
            = new ConcurrentCache<MemberInfo, Func<object, object>>(mi => GetGetterExpression<object>(mi).Compile());

        public static Func<object, object> GetGetter(this MemberInfo memberInfo)
            => GettersCache.Get(memberInfo);

        private static readonly ConcurrentCache<MemberInfo, Action<object, object>> SettersCache
            = new ConcurrentCache<MemberInfo, Action<object, object>>(mi => GetSetterExpression<object>(mi).Compile());

        public static Action<object, object> GetSetter(this MemberInfo memberInfo)
            => SettersCache.Get(memberInfo);

        public static object GetValueFast(this MemberInfo memberInfo, object instance)
            => GetGetter(memberInfo)(instance);

        public static void SetValueFast(this MemberInfo memberInfo, object instance, object value)
            => GetSetter(memberInfo)(instance, value);

        private static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                     "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }
    }
}
