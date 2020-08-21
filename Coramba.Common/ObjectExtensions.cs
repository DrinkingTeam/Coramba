using System;
using System.Linq;

namespace Coramba.Common
{
    public static class ObjectExtensions
    {
        public static TResult If<TIn, TResult>(this TIn obj, Func<TIn, bool> condition, Func<TIn, TResult> ifTrue, Func<TIn, TResult> ifFalse)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (ifTrue == null) throw new ArgumentNullException(nameof(ifTrue));
            if (ifFalse == null) throw new ArgumentNullException(nameof(ifFalse));

            return condition(obj) ? ifTrue(obj) : ifFalse(obj);
        }

        public static TResult If<TIn, TResult>(this TIn obj, bool condition, Func<TIn, TResult> ifTrue, Func<TIn, TResult> ifFalse)
            => obj.If(x => condition, ifTrue, ifFalse);

        public static TResult If<TIn, TResult>(this TIn obj, Func<TIn, bool> condition, Func<TIn, TResult> ifTrue)
            where TIn: TResult
            => obj.If(condition, ifTrue, x => obj);

        public static TResult If<TIn, TResult>(this TIn obj, bool condition, Func<TIn, TResult> ifTrue)
            where TIn : TResult
            => obj.If(condition, ifTrue, x => obj);

        public static bool In<T>(this T obj, params T[] values)
            => values != null && values.Contains(obj);

        public static bool NotIn<T>(this T obj, params T[] values)
            => !obj.In(values);
    }
}
