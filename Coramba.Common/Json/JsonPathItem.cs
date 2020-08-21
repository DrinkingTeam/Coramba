using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Coramba.Common.Json
{
    [DebuggerDisplay("{GetTokenPathTerm()}: IsArray = {IsArray}, IsArrayElement = {IsArrayElement}, IsPrimitive = {IsPrimitive}")]
    public class JsonPathItem
    {
        public JsonPathItem(JsonPathItem top, bool isArray)
        {
            if (top != null)
            {
                if (top.IsArray)
                {
                    PropertyName = (int.Parse(top.CurrentPropertyName, CultureInfo.InvariantCulture) + 1).ToString(CultureInfo.InvariantCulture);
                    top.CurrentPropertyName = PropertyName;
                    IsArrayElement = true;
                }
                else
                    PropertyName = top.CurrentPropertyName;

                SaveNestedValues = top.SaveNestedValues;
            }
            else
                PropertyName = "@";

            IsArray = isArray;
            if (isArray)
                CurrentPropertyName = "-1";
        }

        public string PropertyName { get; }
        public bool IsArrayElement { get; }

        public bool IsArray { get; set; }
        public bool IsPrimitive { get; set; }

        public string CurrentPropertyName { get; internal set; }

        public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();
        public object Value { get; set; }

        public bool SaveNestedValues { get; set; }

        internal static string GetTokenPathTerm(bool isArrayElement, string propertyName)
            => isArrayElement ? $"[{propertyName}]" : $"['{propertyName}']";

        public string GetTokenPathTerm()
            => GetTokenPathTerm(IsArrayElement, PropertyName);
    }
}
