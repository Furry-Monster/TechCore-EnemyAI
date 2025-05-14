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
            OnStateChanged -= state =>
            {
                if (state == NodeState.Error)
                {
                    Debug.LogError($"[MonsterBT] {this.GetType()} occurred Error...");
                }
            };

            foreach (var child in children)
            {
                child.Dispose();
            }

            base.Dispose();
        }

        public virtual BehaviorTreeNode[] GetChildren() => children ?? Array.Empty<BehaviorTreeNode>();

        public int GetChildrenCount() => children?.Count() ?? 0;

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
    }
}