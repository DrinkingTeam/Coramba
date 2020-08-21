using System;
using System.Collections.Generic;
using System.Linq;

namespace Coramba.Common
{
    public class Dfs<TNode>
    {
        public Action<TNode> OnEnter { get; set; }
        public Action<TNode> OnLeave { get; set; }

        private void Walk(TNode node, Func<TNode, IEnumerable<TNode>> getChildren, HashSet<TNode> marks)
        {
            OnEnter?.Invoke(node);

            if (marks.Contains(node))
                return;

            marks.Add(node);

            var children = getChildren(node);
            if (children != null)
            {
                foreach (var child in children)
                {
                    Walk(child, getChildren, marks);
                }
            }

            OnLeave?.Invoke(node);
        }

        private void Walk(IEnumerable<TNode> startNodes, Func<TNode, IEnumerable<TNode>> getChildren, HashSet<TNode> marks)
        {
            foreach (var node in startNodes)
                if (!marks.Contains(node))
                    Walk(node, getChildren, marks);
        }

        public IEnumerable<TNode> Walk(IEnumerable<TNode> startNodes, Func<TNode, IEnumerable<TNode>> getChildren)
        {
            if (startNodes == null) throw new ArgumentNullException(nameof(startNodes));
            if (getChildren == null) throw new ArgumentNullException(nameof(getChildren));

            var marks = new HashSet<TNode>();

            Walk(startNodes, getChildren, marks);

            return marks;
        }

        public static IEnumerable<TNode> TopoSort(IEnumerable<TNode> startNodes, Func<TNode, IEnumerable<TNode>> getChildren)
        {
            var weights = new Dictionary<TNode, int>();

            var count = 0;
            var dfs = new Dfs<TNode>
            {
                OnEnter = n =>
                {
                    if (weights.TryGetValue(n, out var weight) && weight < 0)
                        throw new Exception("Cycle detected");

                    weights[n] = -1;
                },
                OnLeave = n => weights[n] = ++count
            };

            dfs.Walk(startNodes, getChildren);

            return weights.OrderByDescending(x => x.Value).Select(x => x.Key);
        }

        public static IEnumerable<TNode> Reachable(IEnumerable<TNode> startNodes, Func<TNode, IEnumerable<TNode>> getChildren)
        {
            return new Dfs<TNode>().Walk(startNodes, getChildren);
        }
    }
}
