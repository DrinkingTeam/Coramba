using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coramba.Common
{
    public static class EnumHelper
    {
        public static TEnumOut ConvertByName<TEnumIn, TEnumOut>(TEnumIn value)
            => (TEnumOut)Enum.Parse(typeof(TEnumOut), Enum.GetName(typeof(TEnumIn), value));

        public static List<(TEnum Value, MemberInfo MemberInfo)> GetMemberInfo<TEnum>()
        {
            var members = typeof(TEnum).GetMembers();

            return Enum
                .GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => (e, members.Single(x => x.Name == e.ToString() && x.DeclaringType == typeof(TEnum))))
                .ToList();
        }

        public static List<(TEnum Value, TAttribute Attribute)> GetAttributes<TEnum, TAttribute>()
        {
            return GetMemberInfo<TEnum>()
                .Select(x => (x.Value, (TAttribute)(object)x.MemberInfo.GetCustomAttributes(typeof(TAttribute)).Single()))
                .ToList();
        }
    }
}
