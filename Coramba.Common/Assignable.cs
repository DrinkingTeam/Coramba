using System;
using System.Diagnostics;

namespace Coramba.Common
{
    [DebuggerDisplay("{!HasValue?\"NotAssigned\":Value?.ToString()}")]
    public struct Assignable<T>
    {
        private readonly T _value;

        public Assignable(T value)
        {
            _value = value;
            HasValue = true;
        }

        public bool HasValue { get; }

        public T Value => !HasValue ? throw new Exception("Value is not set") : _value;
        public static Assignable<T> NotAssigned => new Assignable<T>();

        public T GetValueOrDefault() => _value;

        public T GetValueOrDefault(T defaultValue) => HasValue ? _value : defaultValue;

        public override bool Equals(object other)
        {
            if (!HasValue) return other == null;
            if (other == null) return false;
            return _value.Equals(other);
        }

        public override int GetHashCode() => HasValue ? _value.GetHashCode() : 0;

        public override string ToString() => HasValue ? _value.ToString() : string.Empty;

        public static implicit operator Assignable<T>(T value) => new Assignable<T>(value);

        public static explicit operator T(Assignable<T> value) => value!.Value;
    }
}
