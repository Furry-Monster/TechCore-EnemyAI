using System;
using UnityEngine;

namespace MonsterBT
{
    public abstract class Decorator : BehaviorTreeNode
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

            child?.Initalize(Tree, Exec);
        }

        protected override NodeState DoExecute()
        {
            var state = child.Execute();

            return state;
        }

        public override void Dispose()
        {
            base.Dispose();

            child.Dispose();
        }
    }
}