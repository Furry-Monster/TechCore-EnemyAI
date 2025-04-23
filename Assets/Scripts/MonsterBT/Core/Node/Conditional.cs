using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{
    public abstract class Conditional : BehaviorTreeNode
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
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            base.Dispose();

            child.Dispose();
        }
    }
}
