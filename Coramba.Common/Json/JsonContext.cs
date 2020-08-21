using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Coramba.Common.Reflection;
using Newtonsoft.Json;

namespace Coramba.Common.Json
{
    public class JsonContext
    {
        public JsonTextReader JsonReader { get; }
        public Stack<JsonPathItem> Stack { get; }

        public bool SaveNestedValues
        {
            get => Stack.Peek().SaveNestedValues;
            set => Stack.Peek().SaveNestedValues = value;
        }

        public object Value => Stack.Peek().Value;
        public Dictionary<string, object> Values => Stack.Peek().Values;

        public JsonContext(JsonTextReader jsonReader, Stack<JsonPathItem> stack)
        {
            JsonReader = jsonReader ?? throw new ArgumentNullException(nameof(jsonReader));
            Stack = stack ?? throw new ArgumentNullException(nameof(stack));
        }

        public string GetJsonPath()
            => JsonParser.GetJsonPath(Stack);

        public string GetLogInformation()
            => JsonParser.GetLogInformation(Stack, JsonReader);

        public bool IsJsonPathEquals(params string[] pattern)
            => JsonParser.IsJsonPathEquals(Stack, pattern);

        public Dictionary<string, object> GetValue(int level = 0)
        {
            foreach (var item in Stack)
            {
                if (level == 0)
                    return item.Values;

                level--;
            }

            return null;
        }

        private T ConvertTo<T>(object value)
        {
            if (value == null && typeof(T).IsNullable())
                return default;
            var type = typeof(T).GetNullableUnderlyingType();

            return (T)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        public T GetValue<T>(string[] keys, int topLevel, bool isRequired)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (keys.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(keys));

            var values = GetValue(topLevel);
            if (values == null)
            {
                if (isRequired)
                    throw new Exception($"There are not top values in stack");
                return default;
            }

            object value = null;
            for (var index = 0; index < keys.Length; index++)
            {
                var key = keys[index];
                if (!values.TryGetValue(key, out value))
                {
                    if (isRequired)
                        throw new Exception($"Key {key} is not in the json dictionary");
                    return default;
                }

                if (index == keys.Length - 1)
                    break;

                if (!(value is Dictionary<string, object> valueDict))
                {
                    if (isRequired)
                        throw new Exception($"Value of key {key} should be a json dictionary");
                    return default;
                }

                values = valueDict;
            }

            return ConvertTo<T>(value);
        }

        public T GetValue<T>(string key, int topLevel = 0)
            => GetValue<T>(new[] {key}, topLevel, false);

        public T GetRequiredValue<T>(string key, int topLevel = 0)
            => GetValue<T>(new[] { key }, topLevel, true);

        public T GetValue<T>(params string[] keys)
        {
            if (keys == null || !keys.Any())
            {
                var value = ConvertTo<T>(Value);
                if (value == null)
                    return default;
            }

            return GetValue<T>(keys, 0, false);
        }

        public T GetRequiredValue<T>(params string[] keys)
        {
            if (keys == null || !keys.Any())
            {
                if (Value == null)
                    throw new Exception($"Value is empty");

                return ConvertTo<T>(Value);
            }

            return GetValue<T>(keys, 0, true);
        }
    }
}
