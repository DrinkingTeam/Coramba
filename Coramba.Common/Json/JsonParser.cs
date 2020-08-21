using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coramba.Common.Json
{
    public class JsonParser: IDisposable
    {
        protected JsonTextReader JsonReader { get; }
        protected Dictionary<JsonToken, List<Func<JsonContext, Task>>> OnTokenActions { get; }
            = new Dictionary<JsonToken, List<Func<JsonContext, Task>>>();

        protected List<(string[] Pattern, Func<JsonContext, Task> Action)> OnObjectStarted { get; }
            = new List<(string[] Pattern, Func<JsonContext, Task> Action)>();
        protected List<(string[] Pattern, Func<JsonContext, Task> Action)> OnObjectParsed { get; }
            = new List<(string[] Pattern, Func<JsonContext, Task> Action)>();

        public JsonParser(TextReader textReader)
        {
            if (textReader == null) throw new ArgumentNullException(nameof(textReader));
            JsonReader = new JsonTextReader(textReader);
        }

        internal static string GetJsonPath(Stack<JsonPathItem> stack)
        {
            if (stack == null) throw new ArgumentNullException(nameof(stack));
            return stack
                .Reverse()
                .Select(x => x.GetTokenPathTerm())
                .ModifyFirst(x => "@")
                .Flatten();
        }

        internal static string GetLogInformation(Stack<JsonPathItem> stack, JsonTextReader jsonReader)
        {
            var result = GetJsonPath(stack);
            if (jsonReader.TokenType == JsonToken.PropertyName)
                result += "->" + JsonPathItem.GetTokenPathTerm(false, jsonReader.Value?.ToString());
            else if (jsonReader.TokenType.NotIn(JsonToken.StartObject, JsonToken.StartArray,
                JsonToken.EndObject, JsonToken.EndArray))
            {
                stack.TryPeek(out var top);
                result += $"->{JsonPathItem.GetTokenPathTerm(top?.IsArray ?? false, top?.CurrentPropertyName)} = '{jsonReader.Value}'";
            }

            return result + $": {jsonReader.TokenType}";
        }

        internal static bool IsJsonPathEquals(Stack<JsonPathItem> stack, string[] pattern)
        {
            if ((pattern?.Length ?? 0) != stack.Count - 1)
                return false;
            if (stack.Count == 1)
                return true;

            var index = 0;
            foreach (var item in stack.Reverse().Skip(1))
            {
                var term = pattern[index];
                if (term == null)
                {
                    index++;
                    continue;
                }

                if (item.PropertyName != term)
                    return false;
                index++;
            }

            return true;
        }

        private List<Func<JsonContext, Task>> GetOnTokenActions(JsonToken jsonToken)
        {
            if (!OnTokenActions.TryGetValue(jsonToken, out var actions))
            {
                actions = new List<Func<JsonContext, Task>>();
                OnTokenActions[jsonToken] = actions;
            }

            return actions;
        }

        public JsonParser OnTokenAsync(JsonToken token, Func<JsonContext, Task> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            GetOnTokenActions(token).Add(action);
            return this;
        }

        public JsonParser OnTokenAsync(Func<JsonContext, Task> action)
        {
            return OnTokenAsync(JsonToken.None, action);
        }

        public JsonParser OnObjectStartedAsync(string[] jsonPathPattern, Func<JsonContext, Task> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            OnObjectStarted.Add((jsonPathPattern ?? new string[0], action));

            return this;
        }

        public JsonParser SaveNested(params string[] pattern)
            => OnObjectStartedAsync(pattern, c =>
            {
                c.SaveNestedValues = true;
                return Task.CompletedTask;
            });

        public JsonParser OnObjectParsedAsync(string[] jsonPathPattern, Func<JsonContext, Task> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            OnObjectParsed.Add((jsonPathPattern ?? new string[0], action));

            return this;
        }

        private Task InvokeOnObjectStarted(Stack<JsonPathItem> stack)
        {
            var context = new JsonContext(JsonReader, stack);
            return OnObjectStarted
                .ForEachAsync(async x =>
                {
                    if (!x.Pattern.Any() || context.IsJsonPathEquals(x.Pattern))
                        await x.Action(context);
                });
        }

        private Task InvokeOnObjectParsed(Stack<JsonPathItem> stack)
        {
            var context = new JsonContext(JsonReader, stack);
            return OnObjectParsed
                .ForEachAsync(async x =>
                {
                    if (!x.Pattern.Any() || context.IsJsonPathEquals(x.Pattern))
                        await x.Action(context);
                });
        }

        private JsonPathItem Push(Stack<JsonPathItem> stack, bool isArray)
        {
            stack.TryPeek(out var top);

            var item = new JsonPathItem(top, isArray);

            stack.Push(item);

            return item;
        }

        private void Pop(Stack<JsonPathItem> stack)
        {
            if (!stack.TryPop(out var top))
                throw new Exception($"Unexpected eof: pop");

            if (stack.TryPeek(out var parent))
            {
                if (top.IsPrimitive)
                    parent.Values[top.PropertyName] = top.Value;
                else if (parent.SaveNestedValues)
                    parent.Values[top.PropertyName] = top.Values;
            }
        }

        public async Task ParseAsync()
        {
            var stack = new Stack<JsonPathItem>();

            var isFirst = true;
            while (await JsonReader.ReadAsync())
            {
                if (!stack.TryPeek(out var top))
                {
                    if (!isFirst)
                        throw new Exception($"Invalid json file: unexpected eof");
                    if (JsonReader.TokenType != JsonToken.StartArray
                        && JsonReader.TokenType != JsonToken.StartObject)
                        throw new Exception("Json file must starts with '{' or '[' symbols");
                }

                switch (JsonReader.TokenType)
                {
                    case JsonToken.StartObject:
                    case JsonToken.StartArray:
                    {
                        Push(stack, JsonReader.TokenType == JsonToken.StartArray);

                        await InvokeOnObjectStarted(stack);

                        break;
                    }
                    case JsonToken.PropertyName:
                    {
                        top.CurrentPropertyName = JsonReader.Value?.ToString();
                        if (top.CurrentPropertyName == null)
                            throw new Exception($"Property name is null: {GetLogInformation(stack, JsonReader)}");

                        break;
                    }
                    case JsonToken.EndObject:
                    case JsonToken.EndArray:
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                    {
                        var isPrimitive = JsonReader.TokenType.NotIn(JsonToken.EndObject, JsonToken.EndArray);
                        if (isPrimitive)
                        {
                            var item = Push(stack, false);

                            item.IsPrimitive = true;
                            item.Value = JsonReader.Value;
                        }

                        await InvokeOnObjectParsed(stack);

                        Pop(stack);

                        break;
                    }
                    case JsonToken.Comment:
                        break;
                    default:
                        throw new NotSupportedException($"Json token type is not supported {JsonReader.TokenType}: {GetLogInformation(stack, JsonReader)}");
                }

                var context = new JsonContext(JsonReader, stack);

                GetOnTokenActions(JsonToken.None)
                    .ForEach(async a =>
                    {
                        await a(context);
                    });

                GetOnTokenActions(JsonReader.TokenType)
                    .ForEach(async a =>
                    {
                        await a(context);
                    });

                isFirst = false;
            }
        }

        public void Dispose()
        {
            ((IDisposable) JsonReader)?.Dispose();
        }
    }
}
