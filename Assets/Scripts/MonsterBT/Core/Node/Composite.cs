using System;
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

                child?.Initalize(Tree, Exec);
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

        public virtual BehaviorTreeNode[] GetChildren()
        {
            return children;
        }

        public virtual int GetChildrenCount()
        {
            throw new NotImplementedException();
        }

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