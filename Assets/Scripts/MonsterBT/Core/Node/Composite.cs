using System;
using System.Linq;
using UnityEngine;

namespace MonsterBT
{
    public abstract class Composite : BehaviorTreeNode, IHasChildren
    {
        private BehaviorTreeNode[] children;

        protected override void OnInitialize()
        {
            OnStateChanged += state =>
            {
                if (state == NodeState.Error)
                {
                    Debug.LogError($"[MonsterBT] {this.GetType()} occurred Error...");
                }
            };

            foreach (var child in children)
            {

                child?.Initalize(Tree, Exec, GameObject);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var child in children)
            {
                child.Dispose();
            }
        }

        public virtual BehaviorTreeNode[] GetChildren() => children ?? Array.Empty<BehaviorTreeNode>();

        public virtual int GetChildrenCount() => children?.Count() ?? 0;

        public virtual void SetChild(int index, BehaviorTreeNode node)
        {
            if (index < 0)
            {
                Debug.LogError($"[MonsterBT] Node index of {index} is InValid");
                return;
            }

            if (index > children.Count())
            {
                Debug.LogError($"[MonsterBT] Node index of {index} is InValid");
                return;
            }

            children[index] = node;
        }

        public virtual void SetChildren(BehaviorTreeNode[] nodes)
        {
            int indBound = children.Count(); ;
            // if out of boundary,
            // we will only cast the valid node in nodes into children array.
            if (nodes.Count() > children.Count())
            {
                Debug.LogWarning(
                    $"[MonsterBT] Node Array is too big to fit for children array\n" +
                    $"So we'd cast it into smaller size");
            }
            else if (nodes.Count() < children.Count())
            {
                Debug.LogWarning(
                    $"[MonsterBT] Node Array is too small to fit for the whole children array\n" +
                    $"So we'd only set sequentially");
                indBound = nodes.Count();
            }

            for (int i = 0; i < indBound; i++)
            {
                children[i] = nodes[i];
            }
        }
    }
}