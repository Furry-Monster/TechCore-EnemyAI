using System;
using UnityEngine;

namespace MonsterBT
{
    public abstract class Decorator : BehaviorTreeNode, IHasChildren
    {
        private BehaviorTreeNode child;

        protected override void OnInitialize()
        {
            OnStateChanged += state =>
            {
                if (state == NodeState.Error)
                {
                    Debug.LogError($"[MonsterBT] {this.GetType()} occurred Error...");
                }
            };

            child?.Initalize(Tree, Exec, GameObject);
        }

        public override void Dispose()
        {
            base.Dispose();

            child.Dispose();
        }

        public virtual BehaviorTreeNode[] GetChildren() =>
            child == null
            ? Array.Empty<BehaviorTreeNode>()
            : new[] { child };

        public virtual int GetChildrenCount() => child == null ? 0 : 1;

        public virtual void SetChild(int index, BehaviorTreeNode node)
        {
            throw new NotImplementedException();
        }

        public virtual void SetChildren(BehaviorTreeNode[] nodes)
        {
            throw new NotImplementedException();
        }
    }
}