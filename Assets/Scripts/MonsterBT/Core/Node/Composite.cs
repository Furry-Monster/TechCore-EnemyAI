using System;
using System.Linq;
using UnityEngine;

namespace MonsterBT
{
    public abstract class Composite : BehaviorTreeNode, IHasChildren
    {
        private readonly BehaviorTreeNode[] children;

        public Composite(int childNum = 0)
        {
            children = new BehaviorTreeNode[childNum];
        }

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
                child?.Dispose();
            }

            base.Dispose();
        }

        public virtual BehaviorTreeNode[] GetChildren()
        {
            if (children == null || children.Count() == 0)
                return Array.Empty<BehaviorTreeNode>();
            return children;
        }

        public int GetChildrenCount() => children.Count();

        public virtual void SetChild(int index, BehaviorTreeNode node)
        {
            if (index < 0 || index >= children.Count())
            {
                Debug.LogError($"[MonsterBT] Node index of {index} is InValid");
                return;
            }
            children[index] = node;
        }
    }
}