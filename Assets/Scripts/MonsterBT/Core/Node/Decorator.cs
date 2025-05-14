using System;
using System.Linq;
using UnityEngine;

namespace MonsterBT
{
    public abstract class Decorator : BehaviorTreeNode, IHasSingleChild
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
            OnStateChanged -= state =>
            {
                if (state == NodeState.Error)
                {
                    Debug.LogError($"[MonsterBT] {this.GetType()} occurred Error...");
                }
            };

            child.Dispose();

            base.Dispose();
        }

        public virtual BehaviorTreeNode GetChild()
        {
            throw new NotImplementedException();
        }

        public int GetChildrenCount() => child == null ? 0 : 1;

        public virtual void SetChild(BehaviorTreeNode node)
        {
            throw new NotImplementedException();
        }
    }
}