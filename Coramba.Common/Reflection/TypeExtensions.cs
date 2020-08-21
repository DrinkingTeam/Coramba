using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Coramba.Common.Reflection
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentCache<(Type Type, BindingFlags Flags), PropertyInfo[]> PropertiesCache
            = new ConcurrentCache<(Type Type, BindingFlags Flags), PropertyInfo[]>(key => key.Type.GetProperties(key.Flags));

        private static readonly ConcurrentCache<(Type type, Type attributeType, bool inherit), Attribute[]> AttributesCache
            = new ConcurrentCache<(Type type, Type attributeType, bool inherit), Attribute[]>(
                key => key.type.GetCustomAttributes(key.attributeType, key.inherit).Cast<Attribute>().ToArray());

        private static readonly ConcurrentCache<(Type type, Type attributeType), PropertyDescriptor[]> PropertyDescriptorsCache
            = new ConcurrentCache<(Type type, Type attributeType), PropertyDescriptor[]>(
                key => TypeDescriptor.GetProperties(key.type).OfType<PropertyDescriptor>()
                    .Where(p => p.Attributes.Cast<Attribute>().Any(a => key.attributeType.IsInstanceOfType(a))).ToArray());

        public static bool IsImplements(this Type type, Type baseType)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (baseType == null) throw new ArgumentNullException(nameof(baseType));

            return GetImplementationTypes(type, baseType).Any();
        }

        public static bool IsImplements<T>(this Type type)
            => type.IsImplements(typeof(T));

        public static IEnumerable<Type> GetImplementationTypes(this Type type, Type baseType)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (baseType == null) throw new ArgumentNullException(nameof(baseType));

            if (!baseType.IsGenericTypeDefinition)
                return baseType.IsAssignableFrom(type) ? new [] { baseType } : new Type[0];

            if (baseType.IsInterface)
                return GetImplementationTypesForInterface();

            return GetImplementationTypesForClass();

            IEnumerable<Type> GetImplementationTypesForInterface()
            {
                return type
                    .GetInterfaces()
                    .Where(IsTypeImplementation);
            }

            IEnumerable<Type> GetImplementationTypesForClass()
            {
                do
                {
                    if (IsTypeImplementation(type))
                        yield return type;

                    type = type.BaseType;
                } while (type != typeof(object) && type != null);
            }

            bool IsTypeImplementation(Type t)
            {
                return t.IsGenericType && t.GetGenericTypeDefinition() == baseType;
            }
        }

        public static Type GetImplementationType(this Type type, Type baseType)
        {
            var types = type.GetImplementationTypes(baseType).Take(2).ToList();
            if (!type.GetImplementationTypes(baseType).TrySingleOrDefault(out var result))
                throw new Exception($"Type {baseType} implements multiple variations of {baseType} type");

            return result;
        }

        public static PropertyInfo[] GetPropertiesCached(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
            => PropertiesCache.Get((type, flags));

        public static PropertyInfo GetPropertyCached(this Type type, string name, BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
            => type.GetPropertiesCached(flags).FirstOrDefault(x => x.Name == name);

        public static TAttribute Attribute<TAttribute>(this Type type, bool inherit = true)
        {
            return Attributes<TAttribute>(type, inherit).FirstOrDefault();
        }

        public static IEnumerable<TAttribute> Attributes<TAttribute>(this Type type, bool inherit = true)
        {
            return Attributes(type, typeof(TAttribute), inherit).Cast<TAttribute>();
        }

        public static IEnumerable<Attribute> Attributes(this Type type, Type attributeType, bool inherit = true)
        {
            return AttributesCache.Get((type, attributeType, inherit));
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetNullableUnderlyingType(this Type type)
        {
            return type.IsNullable() ? Nullable.GetUnderlyingType(type) : type;
        }

        public static PropertyDescriptor[] GetPropertyDescriptorsCached<TAttribute>(this Type type)
            => PropertyDescriptorsCache.Get((type, typeof(TAttribute)));

        public static Type[] GetGenericTypeArguments(this Type type,
            Type @default, params Type[] arguments)
        {
            if (@default == null) throw new ArgumentNullException(nameof(@default));
            if (!type.IsGenericTypeDefinition)
                throw new ArgumentException($"Type {type} must be a generic type definition",
                    nameof(type));

            var genericArgumentsLength = type.GetGenericArguments().Length;
            if (arguments != null && arguments.Length > genericArgumentsLength)
                throw new ArgumentException($"Generic type {type} has {genericArgumentsLength} argument(s), passed {arguments.Length}",
                    nameof(type));

            var argumentsTypeLength = arguments?.Length ?? 0;
            var result = new Type[genericArgumentsLength];

            for (var index = 0; index < genericArgumentsLength; index++)
                result[index] = (index < argumentsTypeLength ? arguments[index] : null) ?? @default;

            return result;
        }

        public static Type[] GetGenericBaseTypeArguments(this Type @base, Type implementation, params Type[] baseArguments)
        {
            if (@base == null) throw new ArgumentNullException(nameof(@base));
            if (implementation == null) throw new ArgumentNullException(nameof(implementation));

            if (!@base.IsGenericTypeDefinition)
                throw new ArgumentException($"Type {@base} must be a generic type definition",
                    nameof(@base));

            var genericBaseArguments = @base.GetGenericArguments();
            if (baseArguments != null && baseArguments.Length > genericBaseArguments.Length)
                throw new ArgumentException($"Generic type {@base} has {genericBaseArguments.Length} argument(s), passed {baseArguments.Length}",
                    nameof(@base));

            if (implementation.IsGenericTypeDefinition)
                throw new ArgumentException($"Type {implementation} is generic type definition", nameof(implementation));

            Type[] implementationArguments = new Type[0];
            Type[] genericImplementationArguments = new Type[0];
            if (implementation.IsGenericType)
            {
                implementationArguments = implementation.GetGenericArguments();
                genericImplementationArguments = implementation.GetGenericTypeDefinition().GetGenericArguments();
            }

            var baseArgumentsLength = baseArguments?.Length ?? 0;

            var baseImplArguments = implementation.GetImplementationType(@base)?.GetGenericArguments();
            if (baseImplArguments == null)
                throw new Exception($"Implementation type for {@base} not found, type = {implementation}");

            var result = new Type[genericBaseArguments.Length];

            for (var i = 0; i < genericBaseArguments.Length; i++)
            {
                genericBaseArguments[i] = i < baseArgumentsLength && baseArguments[i] != null ? baseArguments[i] : baseImplArguments[i];

                if (genericBaseArguments[i] != null && genericBaseArguments[i].IsGenericParameter)
                {
                    var foundIndex = Array.IndexOf(genericImplementationArguments, genericBaseArguments[i]);
                    if (foundIndex >= 0)
                        genericBaseArguments[i] = implementationArguments[foundIndex];
                }
            }

            return result;

        }
    }
}
