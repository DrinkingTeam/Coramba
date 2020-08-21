using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coramba.Common
{
    public static class Graph
    {
        public static async Task<T> DfsAsync<T, TId>(
            T start,
            Func<T, Task<IEnumerable<T>>> getAdj,
            Func<T, TId> getId,
            HashSet<TId> marked,
            Func<T, Task<bool>> preAction = null,
            Func<T, Task<bool>> postAction = null
        )
        {
            if (getAdj == null) throw new ArgumentNullException(nameof(getAdj));
            if (getId == null) throw new ArgumentNullException(nameof(getId));

            var stack = new Stack<T>();
            marked ??= new HashSet<TId>();

            stack.Push(start);
            marked.Add(getId(start));
            var entered = new HashSet<TId>();
            while (stack.TryPeek(out var curNode))
            {
                if (!entered.Add(getId(curNode)))
                {
                    if (postAction != null && await postAction(curNode))
                        return curNode;

                    stack.Pop();
                    continue;
                }

                if (preAction != null && await preAction(curNode))
                    return curNode;

                var adjNodes = (await getAdj(curNode))
                    ?.Where(n => !marked.Contains(getId(n)))
                    .Reverse();
                if (adjNodes != null)
                {
                    foreach (var adjNode in adjNodes)
                    {
                        stack.Push(adjNode);
                        marked.Add(getId(curNode));
                    }
                }
            }

            return default(T);
        }

        public static async Task<List<T>> TopoSortAsync<T, TId>(
            IEnumerable<T> nodes,
            Func<T, Task<IEnumerable<T>>> getAdj,
            Func<T, TId> getId
        )
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));
            if (getAdj == null) throw new ArgumentNullException(nameof(getAdj));
            if (getId == null) throw new ArgumentNullException(nameof(getId));
            
            var result = new List<T>();
            var marked = new HashSet<TId>();

            foreach (var node in nodes)
            {
                if (marked.Contains(getId(node)))
                    continue;

                await DfsAsync(node, getAdj, getId, marked, null, n =>
                {
                    result.Add(n);
                    return Task.FromResult(false);
                });
            }

            result.Reverse();

            return result;
        }

        public static Task<List<T>> TopoSortAsync<T>(
            IEnumerable<T> nodes,
            Func<T, Task<IEnumerable<T>>> getAdj
        ) => TopoSortAsync(nodes, getAdj, n => n);
    }
}
